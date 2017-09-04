using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// The producer produces messages to be submitted to a message consumer.
    /// </summary>
    public interface IMessageProducer
    {
        /// <summary>
        /// Listen to all messages to be submitted to the given message consumer.
        /// </summary>
        /// <param name="messageConsumer">The message consumer.</param>
        /// <returns>The listening task.</returns>
        Task<bool> Listen(IMessageConsumer messageConsumer);
    }
}
