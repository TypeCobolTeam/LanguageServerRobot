using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// A Delegate for filtering message delivered by a producer.
    /// </summary>
    /// <param name="message">The message to filter</param>
    /// <param name="connection">The connection from which the message commes from.</param>
    /// <returns>true if the message has been filtered and thus shall be ignored, false if the message is not filtered
    /// and shall be handle.</returns>
    public delegate bool ProducerMessageFilter(string message, IMessageConnection connection);
}
