using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace LeanCode.Standard.Pipes
{
    public class PipeStreamWriter<T> where T : class
    {
        private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        public PipeStream BaseStream { get; private set; }

        public PipeStreamWriter(PipeStream stream)
        {
            BaseStream = stream;
        }

        private byte[] Serialize(T obj)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    _binaryFormatter.Serialize(memoryStream, obj);
                    return memoryStream.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }

        private void WriteLength(int len)
        {
            var lenbuf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(len));
            BaseStream.Write(lenbuf, 0, lenbuf.Length);
        }

        private void WriteObject(byte[] data)
        {
            BaseStream.Write(data, 0, data.Length);
        }

        private void Flush()
        {
            BaseStream.Flush();
        }

        public void WriteObject(T obj)
        {
            var data = Serialize(obj);
            WriteLength(data.Length);
            WriteObject(data);
            Flush();
        }

        public void WaitForPipeDrain()
        {
            BaseStream.WaitForPipeDrain();
        }
    }
}
