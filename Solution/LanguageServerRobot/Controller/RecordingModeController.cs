using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServerRobot.Model;
using LanguageServerRobot.Utilities;
using Newtonsoft.Json.Linq;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// Robot Recording Mode Controller.
    /// </summary>
    public class RecordingModeController : IRobotModeController
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
            Start = 0x01 << 1,

            /// <summary>
            /// The recording is stopped
            /// </summary>
            Stop = 0x01 << 2,

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

        public bool IsModeInitialized
        {
            get
            {
                return ((int)State & (int)RecordingState.Initialized) != 0 && !IsInitializationError;
            }
        }

        public bool IsModeStarted
        {
            get
            {
                return ((int)State & (int)RecordingState.Start) != 0 && !IsModeStopped;
            }
        }

        public bool IsModeStopped
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
        public void FromClient(string message)
        {
            System.Diagnostics.Contracts.Contract.Assert(message != null);
            JObject jsonObject = null;
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
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Handle message comming from the server side.
        /// </summary>
        /// <param name="message">The Server side message.</param>
        public void FromServer(string message)
        {
            System.Diagnostics.Contracts.Contract.Assert(message != null);
            JObject jsonObject = null;
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
                                }
                            }
                        }
                    }
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
                State |= RecordingState.Initialized;
                State |= RecordingState.Start;
                SessionModel = new Session();
                SessionModel.initialize = InitializeRequest;
                SessionModel.initialize_result = message;
            }
        }
    }
}
