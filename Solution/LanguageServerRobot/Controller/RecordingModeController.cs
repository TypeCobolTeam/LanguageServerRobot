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
    /// Robot Recording Mode Controller.
    /// </summary>
    public class RecordingModeController : AbstractModeController
    {
        /// <summary>
        /// The recording State
        /// </summary>
        public enum RecordingState
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
            /// The recording is stopped
            /// </summary>
            Stop = 0x01 << 3,

            /// <summary>
            /// Initialization request error
            /// </summary>
            InitializationError = 0x01 << 4

        };

        /// <summary>
        /// The Current State.
        /// </summary>
        public RecordingState State
        {
            get;
            protected set;
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
            private set;
        }

        /// <summary>
        /// The original "initialize" request
        /// </summary>
        protected string InitializeRequest
        {
            get;
            set;
        }

        public override bool IsModeInitialized
        {
            get
            {
                return ((int)State & (int)RecordingState.Initialized) != 0 && !IsInitializationError;
            }
        }

        public override bool IsModeStarted
        {
            get
            {
                return ((int)State & (int)RecordingState.Start) != 0 && !IsModeStopped;
            }
        }

        public override bool IsModeStopped
        {
            get
            {
                return ((int)State & (int)RecordingState.Stop) != 0;
            }
        }

        private bool IsInitializationError
        {
            get
            {
                return ((int)State & (int)RecordingState.InitializationError) != 0;
            }
        }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public RecordingModeController()
        {
            State = RecordingState.NotInitialized;
        }

        /// <summary>
        /// Handle message incoming from the client side.
        /// </summary>
        /// <param name="message">The client side message</param>
        public override void FromClient(string message)
        {
            System.Diagnostics.Contracts.Contract.Assert(message != null);
            JObject jsonObject = null;
            bool consumed = false;
            switch(State)
            {
                case RecordingState.NotInitialized:
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
                case RecordingState.Initialized:
                    {
                        if (Protocol.IsNotification(message, out jsonObject))
                        {
                            if (Protocol.IsInitializedNotification(message, out jsonObject))
                            {   //Notification from the Client that it has take in account the "initialize" result from the server.
                                //==> We can start both Client and Server are OK.
                                State |= RecordingState.Start;
                                SessionModel.initialized_notification = message;
                                consumed = true;
                            }
                        }
                        if (!consumed)
                        {
                            SessionModel.client_in_initialize_messages.Add(message);
                        }
                    }
                    break;
                case RecordingState.Initialized | RecordingState.Start:
                    {

                    }
                    if (!consumed)
                    {
                        SessionModel.client_in_start_messages.Add(message);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handle message comming from the server side.
        /// </summary>
        /// <param name="message">The Server side message.</param>
        public override void FromServer(string message)
        {
            System.Diagnostics.Contracts.Contract.Assert(message != null);
            JObject jsonObject = null;
            bool consumed = false;
            switch (State)
            {
                case RecordingState.NotInitialized:
                    {
                        System.Diagnostics.Contracts.Contract.Ensures(SessionModel == null && JInitializeObject != null);
                        if (SessionModel == null && JInitializeObject != null)
                        {
                            if (Protocol.IsResponse(message, out jsonObject))
                            {
                                string requestId = Protocol.GetRequestId(JInitializeObject);
                                string responseIde = Protocol.GetRequestId(jsonObject);
                                if (requestId != null && responseIde != null && requestId.Equals(responseIde))
                                {//Ok Initalization result
                                    InitializeSession(message, jsonObject);
                                    consumed = true;
                                }
                            }
                        }
                        if (!consumed)
                        {
                            LogNotConsumedMessage(message);
                        }
                    }
                    break;
                case RecordingState.Initialized:
                    if (!consumed)
                    {
                        SessionModel.server_in_initialize_messages.Add(message);
                    }
                    break;
                case RecordingState.Initialized | RecordingState.Start:
                    {

                    }
                    if (!consumed)
                    {
                        SessionModel.server_in_start_messages.Add(message);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Initialization of a Recording Session
        /// </summary>
        /// <param name="message">The LSP initialization result</param>
        /// <param name="jsonObject">The Initialization JSon result</param>
        private void InitializeSession(string message, JObject jsonObject)
        {
            //Maybe initailization failed            
            if (Protocol.IsErrorResponse(jsonObject))
            {
                State |= RecordingState.InitializationError;
            }
            else
            {
                State &= ~RecordingState.NotInitialized;
                State |= RecordingState.Initialized;                                
                SessionModel = new Session();
                SessionModel.initialize = InitializeRequest;
                SessionModel.initialize_result = message;
            }
        }
    }
}
