using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeCobol.LanguageServer.JsonRPC
{
    /// <summary>
    /// Interface for supporting Connection Logs.
    /// </summary>
    public interface IConnectionLog
    {
        /// <summary>
        /// General Log TextWriter
        /// </summary>C:\Users\MAYANJE\Source\Repos\TypeCobol9\LanguageServerRobot\LanguageServerRobot\Solution\LanguageServer.JsonRPC\IConnectionLog.cs
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
