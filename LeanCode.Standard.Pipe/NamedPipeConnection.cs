using System;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Threading;

namespace LeanCode.Standard.Pipes
{
    public class NamedPipeConnection<TRead, TWrite>
            where TRead : class
            where TWrite : class
    {
        private bool _notifiedSucceeded;

        public readonly int Id;

        public string Name { get; }

        public bool IsConnected { get { return streamWrapper.IsConnected; } }

        public event ConnectionEventHandler<TRead, TWrite> Disconnected;

        public event ConnectionMessageEventHandler<TRead, TWrite> ReceiveMessage;

        public event ConnectionExceptionEventHandler<TRead, TWrite> Error;

        private readonly PipeStreamWrapper<TRead, TWrite> streamWrapper;

        private readonly AutoResetEvent writeSignal = new AutoResetEvent(false);

        private readonly BlockingCollection<TWrite> writeQueue = new BlockingCollection<TWrite>();

        internal NamedPipeConnection(int id, string name, PipeStream serverStream)
        {
            Id = id;
            Name = name;
            streamWrapper = new PipeStreamWrapper<TRead, TWrite>(serverStream);
        }

        public void Open()
        {
            var readWorker = new Worker();
            readWorker.Succeeded += OnSucceeded;
            readWorker.Error += OnError;
            readWorker.DoWork(ReadPipe);

            var writeWorker = new Worker();
            writeWorker.Succeeded += OnSucceeded;
            writeWorker.Error += OnError;
            writeWorker.DoWork(WritePipe);
        }

        public void PushMessage(TWrite message)
        {
            writeQueue.Add(message);
            writeSignal.Set();
        }

        public void Close()
        {
            CloseImpl();
        }

        private void CloseImpl()
        {
            streamWrapper.Close();
            writeSignal.Set();
        }

        private void OnSucceeded()
        {
            if (_notifiedSucceeded)
            {
                return;
            }

            _notifiedSucceeded = true;

            Disconnected?.Invoke(this);
        }

        private void OnError(Exception exception) => Error?.Invoke(this, exception);

        private void ReadPipe()
        {

            while (IsConnected && streamWrapper.CanRead)
            {
                try
                {
                    var obj = streamWrapper.ReadObject();
                    if (obj == null)
                    {
                        CloseImpl();
                        return;
                    }

                    ReceiveMessage?.Invoke(this, obj);
                }
                catch
                {
                    //we must igonre exception, otherwise, the namepipe wrapper will stop work.
                }
            }

        }

        private void WritePipe()
        {
            while (IsConnected && streamWrapper.CanWrite)
            {
                try
                {
                    streamWrapper.WriteObject(writeQueue.Take());
                    streamWrapper.WaitForPipeDrain();
                }
                catch
                {
                    //we must igonre exception, otherwise, the namepipe wrapper will stop work.
                }
            }

        }
    }

    static class ConnectionFactory
    {
        private static int _lastId;

        public static NamedPipeConnection<TRead, TWrite> CreateConnection<TRead, TWrite>(PipeStream pipeStream)
            where TRead : class
            where TWrite : class
        {
            return new NamedPipeConnection<TRead, TWrite>(++_lastId, "Client " + _lastId, pipeStream);
        }
    }

    public delegate void ConnectionEventHandler<TRead, TWrite>(NamedPipeConnection<TRead, TWrite> connection)
        where TRead : class
        where TWrite : class;

    public delegate void ConnectionMessageEventHandler<TRead, TWrite>(NamedPipeConnection<TRead, TWrite> connection, TRead message)
        where TRead : class
        where TWrite : class;

    public delegate void ConnectionExceptionEventHandler<TRead, TWrite>(NamedPipeConnection<TRead, TWrite> connection, Exception exception)
        where TRead : class
        where TWrite : class;
}
