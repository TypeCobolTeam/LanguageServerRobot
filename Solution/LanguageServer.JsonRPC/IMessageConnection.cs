using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// Interface for a class which can send messages
    /// </summary>
    public interface IMessageConnection
    {
        /// <summary>
        /// Send a message to the client
        /// </summary>
        void SendMessage(string message);

        /// <summary>
        /// Write a trace in the connection log file
        /// </summary>
        void WriteConnectionLog(string trace);
    }
}
