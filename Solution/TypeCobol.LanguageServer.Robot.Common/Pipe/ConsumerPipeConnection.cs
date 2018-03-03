using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TypeCobol.LanguageServer.Robot.Common.Pipe
{
    /// <summary>
    /// The Consumer Pipe Connection
    /// </summary>
    public class ConsumerPipeConnection : PipeDataConnection
    {
        /// <summary>
        /// Open a connection, This method will block for a server connection.
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

            PipeDataStream =
                        new NamedPipeClientStream(".", pipeName,
                            PipeDirection.InOut, PipeOptions.None,
                            TokenImpersonationLevel.Impersonation);

            Console.WriteLine("Connecting to server...\n");
            (PipeDataStream as NamedPipeClientStream).Connect();
            return true;
        }
    }
}
