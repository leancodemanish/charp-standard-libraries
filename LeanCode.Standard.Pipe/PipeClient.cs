using System;
using System.IO.Pipes;
using System.Text;

namespace LeanCode.Standard.Pipes
{
    public class PipeClient
    {
        private static NamedPipeClientStream pipeStream;

        public void Send(string message, string pipeName, int timeout = 1000)
        {
            var pipeStream = new NamedPipeClientStream(".", pipeName, PipeDirection.Out, PipeOptions.Asynchronous);

            pipeStream.Connect(timeout);

            var buffer = Encoding.UTF8.GetBytes(message);
            pipeStream.BeginWrite(buffer, 0, buffer.Length, AsyncSend, pipeStream);
        }

        private void AsyncSend(IAsyncResult result)
        {
            try
            {
                var pipeStream = (NamedPipeClientStream)result.AsyncState;
                pipeStream.EndWrite(result);
                pipeStream.Flush();
                pipeStream.Close();
                pipeStream.Dispose();
            }
            finally
            {
            }
        }
    }
}
