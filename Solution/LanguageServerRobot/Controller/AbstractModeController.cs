﻿using System;
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

    }
}
