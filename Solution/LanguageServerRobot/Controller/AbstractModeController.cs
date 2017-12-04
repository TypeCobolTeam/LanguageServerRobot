using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;
using LanguageServerRobot.Model;
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
        /// <summary>
        /// The recording State
        /// </summary>
        public enum ModeState
        {
            /// <summary>
            /// Waiting for initialization.
            /// </summary>
            NotInitialized = 0x01 << 0,
            /// <summary>
            /// Initialized
            /// </summary>
            Initialized = 0x01 << 1,
            /// <summary>
            /// The Recording is started.
            /// </summary>
            Start = 0x01 << 2,

            /// <summary>
            /// If we are ShutingDown or Exiting down
            /// </summary>
            ShutingDownOrExiting = 0x01 << 3,

            /// <summary>
            /// The recording is stopped
            /// </summary>
            Stop = 0x01 << 4,

            /// <summary>
            /// Initialization request error
            /// </summary>
            InitializationError = 0x01 << 5

        };

        private int myState;
        /// <summary>
        /// State change event handler.
        /// </summary>
        event EventHandler StageChangedEvent;

        /// <summary>
        /// The Current State.
        /// </summary>
        public ModeState State
        {
            get
            {
                return (ModeState)System.Threading.Interlocked.Exchange(ref myState, myState);
            }
            internal set
            {
                if (System.Threading.Interlocked.Exchange(ref myState, (int)value) != (int)value)
                {
                    if (StageChangedEvent != null)
                    {
                        StageChangedEvent(this, null);
                    }
                }
            }
        }

        /// <summary>
        /// The Model of the current session.
        /// </summary>
        public Session SessionModel
        {
            get; protected set;
        }

        /// <summary>
        /// The JSON Object of the "initialize" request;
        /// </summary>
        public JObject JInitializeObject
        {
            get;
            protected set;
        }

        /// <summary>
        /// The original "initialize" request
        /// </summary>
        protected string InitializeRequest
        {
            get;
            set;
        }

        /// <summary>
        /// The original "shutdown" request
        /// </summary>
        protected string ShutdownRequest
        {
            get;
            set;
        }

        /// <summary>
        /// The original JSON Object of "shutdown" request
        /// </summary>
        protected JObject JShutdownObject
        {
            get;
            set;
        }

        public bool IsModeInitialized
        {
            get
            {
                return ((int)State & (int)ModeState.Initialized) != 0 && !IsInitializationError;
            }
        }

        public bool IsModeStarted
        {
            get
            {
                return ((int)State & (int)ModeState.Start) != 0 && !IsModeStopped;
            }
        }

        public bool IsModeStopped
        {
            get
            {
                return ((int)State & (int)ModeState.Stop) != 0;
            }
        }

        protected bool IsInitializationError
        {
            get
            {
                return ((int)State & (int)ModeState.InitializationError) != 0;
            }
        }

        protected bool IsShutingDownOrExiting
        {
            get
            {
                return ((int)State & (int)ModeState.ShutingDownOrExiting) != 0;
            }
        }

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
