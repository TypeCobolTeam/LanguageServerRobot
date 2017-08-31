using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// Interface for a single message consumer.
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// Consume a String message
        /// </summary>
        /// <param name="message"></param>
        void Consume(string message);
    }
}
