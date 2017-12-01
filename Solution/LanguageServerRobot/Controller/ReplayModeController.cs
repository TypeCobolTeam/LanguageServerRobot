﻿using LanguageServerRobot.Model;
using LanguageServerRobot.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// Robot Replay mode controller
    /// </summary>
    public class ReplayModeController : AbstractModeController
    {
        /// <summary>
        /// Empty constructor.
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        /// </summary>
        public ReplayModeController(Model.Script script, string scriptRepositoryPath = null) : base(scriptRepositoryPath)
        {
            System.Diagnostics.Debug.Assert(script != null);
            State = ModeState.NotInitialized;
            SourceScript = script;
            ResultScript = new Script();
        }

        /// <summary>
        /// The Current message index in the script message.
        /// </summary>
        private long m_ScriptMessageIndex;

        /// <summary>
        /// Current Script Message Index.
        /// </summary>
        internal long ScriptMessageIndex
        {
            get
            {
                return System.Threading.Interlocked.Read(ref m_ScriptMessageIndex);
            }
            set
            {
                System.Threading.Interlocked.Exchange(ref m_ScriptMessageIndex, value);
            }
        }

        /// <summary>
        /// The resulting script of the replay session.
        /// </summary>
        public Script ResultScript
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Script being replayed
        /// </summary>
        public Model.Script SourceScript
        {
            get;
            private set;
        }

        /// <summary>
        /// Determine if yes or not we must stop At the first error.
        /// </summary>
        public bool StopAtFirstError
        {
            get;
            set;
        }

        long m_ErrorIndex = -1;
        /// <summary>
        /// The Index of the firt error encountered in the Script.
        /// </summary>
        public long ErrorIndex
        {
            get
            {
                return System.Threading.Interlocked.Read(ref m_ErrorIndex);
            }           
            internal set
            {
                if (ErrorIndex < 0)
                {
                    System.Threading.Interlocked.Exchange(ref m_ScriptMessageIndex, value);
                }
            }
        }

        /// <summary>
        /// Event handlers when a Response to a request is received from the server.
        /// </summary>
        public event EventHandler< Tuple<string,JObject> > ResponseEvent;

        /// <summary>
        /// Event handlers when a Notification is received from the server.
        /// </summary>
        public event EventHandler<Tuple<string, JObject> > NotificationEvent;

        public override void FromClient(string message)
        {
            System.Diagnostics.Contracts.Contract.Assert(message != null);
            JObject jsonObject = null;
            bool consumed = false;
            switch (State)
            {
                case ModeState.NotInitialized:
                    {
                        System.Diagnostics.Contracts.Contract.Ensures(SessionModel == null && JInitializeObject == null);
                        if (SessionModel == null && JInitializeObject == null)
                        {
                            if (Protocol.IsInitializeRequest(message, out jsonObject))
                            {   //Save the original initialization object.
                                JInitializeObject = jsonObject;
                                InitializeRequest = message;
                                consumed = true;
                            }
                        }
                        if (!consumed)
                        {
                            LogNotConsumedMessage(message);
                        }
                    }
                    break;
                case ModeState.Initialized:
                    {
                        if (Protocol.IsNotification(message, out jsonObject))
                        {
                            if (Protocol.IsInitializedNotification(message, out jsonObject))
                            {   //Notification from the Client that it has take in account the "initialize" result from the server.
                                //==> We can start both Client and Server are OK.
                                State |= ModeState.Start;
                                consumed = true;
                            }
                        }
                    }
                    break;
                case ModeState.Initialized | ModeState.Start:
                    {
                        string uri = null;
                        if (Protocol.IsNotification(message, out jsonObject))
                        {//1)Detect any new script for a document ==> detect didOpen notification
                            //Detect the exit Notification()
                            if (Protocol.IsExitNotification(jsonObject))
                            {//So forces the End of the session
                                if (JShutdownObject != null)
                                {//We have already received a shutdown request.
                                    this.State = this.State | ModeState.ShutingDownOrExiting;
                                    consumed = true;
                                }
                            }
                            else if (Protocol.IsDidOpenTextDocumentNotification(jsonObject))
                            {
                                //Store the message in the result script
                                this.ResultScript.didOpen = message;
                                consumed = true;
                            }
                            else if (Protocol.IsDidCloseTextDocumentNotification(jsonObject))
                            {
                                //Store the message in the result script
                                this.ResultScript.didClose = message;
                                consumed = true;
                            }
                            else if (Protocol.IsMessageWithUri(jsonObject, out uri))
                            {
                                //Store the notification in the result script
                                this.ResultScript.AddMessage(Script.MessageCategory.Client, message);
                                consumed = true;
                            }
                        }
                        else if (Protocol.IsRequest(jsonObject))
                        {
                            if (Protocol.IsShutdownRequest(jsonObject))
                            {
                                consumed = true;
                            }
                            else
                            {
                                this.ResultScript.AddMessage(Script.MessageCategory.Client, message);
                                consumed = true;
                            }
                        }
                        else if (Protocol.IsErrorResponse(jsonObject))
                        {//Hum...A response receive from the Client this cannot happend ==> Log it.
                            LogUnexpectedMessage(Resource.UnexpectedResponseFromClient, message);
                            //But we must register it has result
                            this.ResultScript.AddMessage(Script.MessageCategory.Client, message);
                            consumed = true;
                        }
                    }
                    if (!consumed)
                    {
                        LogUnexpectedMessage(Resource.UnexpectedMessageFromClient, message);
                        //But we must register it has result
                        this.ResultScript.AddMessage(Script.MessageCategory.Client, message);
                    }
                    break;
                default:
                    break;
            }
        }

        public override void FromServer(string message)
        {
            System.Diagnostics.Contracts.Contract.Assert(message != null);
            JObject jsonObject = null;
            bool consumed = false;
            switch (State)
            {
                case ModeState.NotInitialized:
                    {
                        System.Diagnostics.Contracts.Contract.Ensures(SessionModel == null && JInitializeObject != null);
                        if (SessionModel == null && JInitializeObject != null)
                        {
                            if (Protocol.IsResponse(message, out jsonObject))
                            {
                                string requestId = Protocol.GetRequestId(JInitializeObject);
                                string responseIde = Protocol.GetRequestId(jsonObject);
                                if (requestId != null && responseIde != null && requestId.Equals(responseIde))
                                {
                                    State &= ~ModeState.NotInitialized;
                                    State |= ModeState.Initialized;
                                    consumed = true;
                                    RaiseResponseEvent(message, jsonObject);
                                }
                            }
                        }
                        if (!consumed)
                        {
                            LogNotConsumedMessage(message);
                        }
                    }
                    break;
                case ModeState.Initialized:
                    break;
                case ModeState.Initialized | ModeState.Start:
                    {
                        if (Protocol.IsNotification(message, out jsonObject))
                        {
                            //Ignore any notification which is not an uri notification                        
                            string uri = null;
                            if (Protocol.IsMessageWithUri(jsonObject, out uri))
                            {
                                this.ResultScript.AddMessage(Script.MessageCategory.Server, message);
                                RaiseNotificationEvent(message, jsonObject);
                                if (StopAtFirstError)
                                {
                                    if ((ResultScript.messages.Count - 1) < this.SourceScript.messages.Count)
                                    {
                                        if (!(this.SourceScript.messages[ResultScript.messages.Count - 1].category == Script.MessageCategory.Server &&
                                            this.SourceScript.messages[ResultScript.messages.Count - 1].message == message))
                                        {//We have a mismatch notification
                                            this.ErrorIndex = ResultScript.messages.Count - 1;
                                        }
                                    }
                                }
                            }
                            consumed = true;
                        }
                        else if (Protocol.IsResponse(jsonObject))
                        {
                            this.ResultScript.AddMessage(Script.MessageCategory.Server, message);
                            RaiseResponseEvent(message, jsonObject);
                            if (StopAtFirstError)
                            {
                                if ((ScriptMessageIndex + 1) < this.SourceScript.messages.Count)
                                {
                                    if (!(this.SourceScript.messages[(int)ScriptMessageIndex + 1].category == Script.MessageCategory.Result &&
                                        this.SourceScript.messages[(int)ScriptMessageIndex + 1].message == message))
                                    {   //We have a mismatch Result.
                                        this.ErrorIndex = ScriptMessageIndex + 1;
                                    }
                                }
                            }
                            consumed = true;
                        }
                        else
                        {//ignore something else
                            consumed = true;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// raise a response event
        /// </summary>
        /// <param name="message">The text message</param>
        /// <param name="jsonObject">The Json object representing the message</param>
        private void RaiseResponseEvent(string message, JObject jsonObject)
        {
            if (ResponseEvent != null)
            {//Notify all listener
                ResponseEvent(this, new Tuple<string, JObject>(message, jsonObject));
            }
        }

        /// <summary>
        /// raise a notification event
        /// </summary>
        /// <param name="message">The text message</param>
        /// <param name="jsonObject">The Json object representing the message</param>
        private void RaiseNotificationEvent(string message, JObject jsonObject)
        {
            if (NotificationEvent != null)
            {//Notify all listener
                NotificationEvent(this, new Tuple<string, JObject>(message, jsonObject));
            }
        }
    }
}
