using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;
using LanguageServerRobot.Utilities;
using Newtonsoft.Json.Linq;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// An abstract Robot Mode Controller class, that can be as based class. 
    /// For instance it provides log mechanism over unconsumed messages.
    /// </summary>
    public abstract class AbstractModeController : IRobotModeController, IConnectionLog
    {
        public abstract bool IsModeInitialized { get; }
        public abstract bool IsModeStarted { get; }
        public abstract bool IsModeStopped { get; }

        public TextWriter LogWriter
        {
            get; set;
        }

        public TextWriter ProtocolLogWriter
        {
            get; set;
        }

        public TextWriter MessageLogWriter
        {
            get; set;
        }

        public abstract void FromClient(string message);
        public abstract void FromServer(string message);

        /// <summary>
        /// The default script repository path
        /// </summary>
        public string ScriptRepositoryPath
        {
            get;
            protected set;
        }

        /// <summary>
        /// Protectec constructor
        /// </summary>
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        protected AbstractModeController(string scriptRepositoryPath = null)
        {
            ScriptRepositoryPath = scriptRepositoryPath == null ? Utilities.Util.DefaultScriptRepositorPath : scriptRepositoryPath;
        }

        /// <summary>
        /// Log a message that has not been consumed.
        /// </summary>
        /// <param name="message"></param>
        public void LogNotConsumedMessage(string message)
        {
            JObject jsonMessage = null;
            if (Protocol.IsShowMessageNotification(message, out jsonMessage))
            {
                MessageLogWriter?.WriteLine(message);
            }            
            else
            {
                ProtocolLogWriter?.WriteLine(message);
            }
        }

        /// <summary>
        /// Log an unexpected message from the protocol
        /// </summary>
        /// <param name="format">The format of message to usse</param>
        /// <param name="message">The message to log</param>
        public void LogUnexpectedMessage(string format, string message)
        {
            ProtocolLogWriter?.WriteLine(string.Format(format,message));
        }
    }
}
