using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// Interface for a class which can handle messages
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Do something useful with the message received, use the connection interface to reply
        /// </summary>
        void HandleMessage(string message, IMessageConnection connection);
    }
}
