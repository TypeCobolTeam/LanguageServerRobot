using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// Verbosity of the connection traces
    /// </summary>
    public enum ConnectionLogLevel
    {
        /// <summary>
        /// Traces only important connection lifecycle events (startup / shutdown)
        /// </summary>
        Lifecycle,
        /// <summary>
        /// Traces timestamp + content length for each message received and sent
        /// </summary>
        Message,
        /// <summary>
        /// Traces the complete text content of each message received and sent
        /// </summary>
        Protocol
    }
}
