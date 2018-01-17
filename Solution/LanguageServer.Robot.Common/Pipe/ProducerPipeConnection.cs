using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.Robot.Common.Pipe
{
    /// <summary>
    /// The Connection used by the producer using a Pipe mechanism
    /// </summary>
    public class ProducerPipeConnection : PipeDataConnection
    {
        /// <summary>
        /// Open a connection, This method will block for an incoming client connection.
        /// </summary>
        /// <para m name="connectionData">Here is the pipe's name</param>
        /// <returns>true if ok, false otherwise</returns>
        public override bool OpenConnection(object connectionData)
        {
            if (!(connectionData is String))
            {
                throw new ArgumentException("Expected a String as argument");
            }
            String pipeName = (String)connectionData;
            PipeDataStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1);

            //int threadId = Thread.CurrentThread.ManagedThreadId;

            // Wait for a client to connect
            System.Console.WriteLine("Producer Wait for a client to connect");
            (PipeDataStream as NamedPipeServerStream).WaitForConnection();
            System.Console.WriteLine("Producer got the connection");
            return true;
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        /// <returns>true if OK, false otherwise</returns>
        public override bool CloseConnection()
        {
            (PipeDataStream as NamedPipeServerStream).Disconnect();
            return base.CloseConnection();
        }

    }
}
