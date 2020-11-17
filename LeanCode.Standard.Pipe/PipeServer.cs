using System;
using System.IO.Pipes;
using System.Text;

namespace LeanCode.Standard.Pipes
{
    public delegate void DelegateMessage(string message);

    public class PipeServer
    {
        public event DelegateMessage PipeMessage;
        private NamedPipeServerStream _pipeServer;
        private string _pipeName;
        private bool keepAlive;

        public PipeServer(bool keepAlive = false)
        {
            this.keepAlive = keepAlive;
        }

        public void Listen(string pipeName)
        {
            try
            {
                _pipeName = pipeName;
                _pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

                _pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), _pipeServer);
            }
            catch
            {
            }
        }

        private void WaitForConnectionCallBack(IAsyncResult iar)
        {
            try
            {
                _pipeServer.EndWaitForConnection(iar);

                var buffer = new byte[1024];
                var sb = new StringBuilder();

                int read;

                while ((read = _pipeServer.Read(buffer, 0, buffer.Length)) > 0)
                {
                    sb.Append(Encoding.ASCII.GetString(buffer, 0, read));
                    if (_pipeServer.IsMessageComplete)
                    {
                        break;
                    }
                }

                PipeMessage.Invoke(sb.ToString());

                // Kill original sever and create new wait server
                _pipeServer.Close();
                _pipeServer = null;

                if (keepAlive)
                {
                    _pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

                    // Recursively wait for the connection again and again....
                    _pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), _pipeServer);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
