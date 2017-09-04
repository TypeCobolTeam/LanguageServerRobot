using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// Interface for supporting Connection Logs.
    /// </summary>
    public interface IConnectionLog
    {
        /// <summary>
        /// General Log TextWriter
        /// </summary>
        TextWriter LogWriter { get; set; }

        /// <summary>
        /// Protocol Log TextWriter
        /// </summary>
        TextWriter ProtocolLogWriter { get; set; }

        /// <summary>
        /// Message Log TextWriter
        /// </summary>
        TextWriter MessageLogWriter { get; set; }

    }
}
