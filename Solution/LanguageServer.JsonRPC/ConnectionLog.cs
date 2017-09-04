using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// The Connection Log instance. This class can be used as a Singleton.
    /// </summary>
    public class ConnectionLog : IConnectionLog
    {
        private static ConnectionLog m_Instance;
        /// <summary>
        /// Empty constructor
        /// </summary>
        public ConnectionLog()
        {
        }

        /// <summary>
        /// Return a unique instance of a ConnectionLog.
        /// </summary>
        /// <returns></returns>
        public static ConnectionLog GetInstance()
        {
            lock(typeof(ConnectionLog))
            {
                if (m_Instance == null)
                {
                    m_Instance = new ConnectionLog();
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// Assign the current Log writers to another Connection Log instance
        /// </summary>
        /// <param name="logger">The target Connection Log Instance</param>
        public void AssignTo(IConnectionLog to_logger)
        {
            if (to_logger != null)
            {
                to_logger.LogWriter = LogWriter;
                to_logger.MessageLogWriter = MessageLogWriter;
                to_logger.ProtocolLogWriter = ProtocolLogWriter;
            }
        }
        /// <summary>
        /// General Log TextWriter
        /// </summary>
        public TextWriter LogWriter { get; set; }

        /// <summary>
        /// Protocol Log TextWriter
        /// </summary>
        public TextWriter ProtocolLogWriter { get; set; }

        /// <summary>
        /// Message Log TextWriter
        /// </summary>
        public TextWriter MessageLogWriter { get; set; }
    }
}
