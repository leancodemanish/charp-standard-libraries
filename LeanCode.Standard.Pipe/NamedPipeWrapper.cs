using System.IO.Pipes;

namespace LeanCode.Standard.Pipes
{
    public class PipeStreamWrapper<TReadWrite> : PipeStreamWrapper<TReadWrite, TReadWrite>
            where TReadWrite : class
    {
        public PipeStreamWrapper(PipeStream stream) : base(stream)
        {
        }
    }

    public class PipeStreamWrapper<TRead, TWrite>
        where TRead : class
        where TWrite : class
    {
        private readonly PipeStreamReader<TRead> reader;
        private readonly PipeStreamWriter<TWrite> writer;

        public PipeStream BaseStream { get; private set; }

        public bool IsConnected
        {
            get { return BaseStream.IsConnected && reader.IsConnected; }
        }

        public bool CanRead
        {
            get { return BaseStream.CanRead; }
        }

        public bool CanWrite
        {
            get { return BaseStream.CanWrite; }
        }

        public PipeStreamWrapper(PipeStream stream)
        {
            BaseStream = stream;

            reader = new PipeStreamReader<TRead>(BaseStream);
            writer = new PipeStreamWriter<TWrite>(BaseStream);
        }

        public TRead ReadObject()
        {
            return reader.ReadObject();
        }

        public void WriteObject(TWrite obj)
        {
            writer.WriteObject(obj);
        }

        public void WaitForPipeDrain()
        {
            writer.WaitForPipeDrain();
        }

        public void Close()
        {
            BaseStream.Close();
        }
    }
}
