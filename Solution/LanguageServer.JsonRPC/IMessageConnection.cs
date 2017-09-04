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
        /// Starts a connection
        /// </summary>
        /// <param name="messageConsumer">The connection's message consumer</param>
        /// <returns>The connection's listening Task.</returns>
        Task<bool> Start(IMessageConsumer messageConsumer);

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
