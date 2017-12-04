using LanguageServer.JsonRPC;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// Abstract class for a Robot Connection controller for replaying script or session
    /// </summary>
    public class AbstractReplayRobotConnectionController : ClientRobotConnectionController
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        protected AbstractReplayRobotConnectionController()
        {
            ControllerState = ConnectionState.New;
            InCommingNotification = new List<string>();
        }

        /// <summary>
        /// Message Connection constructor
        /// </summary>
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        /// <param name="messageConnection">The Message Connection with the source client.</param>
        public AbstractReplayRobotConnectionController(IMessageConnection messageConnection) : base(messageConnection)
        {
            ControllerState = ConnectionState.New;
            InCommingNotification = new List<string>();
        }

        private long MyConnectionState;
        /// <summary>
        /// The Message Connection state
        /// </summary>
        public override ConnectionState State
        {
            get
            {
                return (ConnectionState)System.Threading.Interlocked.Read(ref MyConnectionState);
            }
        }

        /// <summary>
        /// Internal state so that we can fire state change events.
        /// </summary>
        protected ConnectionState ControllerState
        {
            get
            {
                return State;
            }
            set
            {
                if (System.Threading.Interlocked.Exchange(ref MyConnectionState, (long)value) != (long)value)
                {
                    if (StageChangedEvent != null)
                    {
                        StageChangedEvent(this, null);
                    }
                }
            }
        }

        /// <summary>
        /// Get the Replay Mode Contoller.
        /// </summary>
        internal ReplayModeController ReplayController
        {
            get
            {
                return (ReplayModeController)base.RobotModeController;
            }
        }

        /// <summary>
        /// Tracking incoming notifications
        /// </summary>
        protected List<String> InCommingNotification
        {
            get;
            set;
        }
        event EventHandler StageChangedEvent;

        /// <summary>
        /// Add a Statge Changed Event Handler
        /// </summary>
        /// <param name="handler">The handler to be added</param>
        public override void AddStageChangedEventHandler(EventHandler handler)
        {
            StageChangedEvent += handler;
        }

        /// <summary>
        /// Remove a State Change Event Handler. 
        /// </summary>
        /// <param name="handler"></param>
        public override void RemoveStageChangedEventHandler(EventHandler handler)
        {
            StageChangedEvent -= handler;
        }

        /// <summary>
        /// Write the replay result
        /// </summary>
        /// <param name="message">any exception message to be reported</param>
        protected void WriteReplayResult(string message = null)
        {
            this.ReplayController.SaveResult(message);
        }

        /// <summary>
        /// Connect or disconnect message event hnadlers from the REplya Controller.
        /// </summary>
        /// <param name="bConnect">true to connect , false to disconnect.</param>
        protected virtual void ConnectMessageEventHandlers(bool bConnect)
        {
            if (bConnect)
            {
                this.ReplayController.ResponseEvent += ReplayControllerResponseEvent;
                this.ReplayController.NotificationEvent += ReplayControllerNotificationEvent;
            }
            else
            {
                this.ReplayController.ResponseEvent -= ReplayControllerResponseEvent;
                this.ReplayController.NotificationEvent -= ReplayControllerNotificationEvent;
            }
        }

        /// <summary>
        /// Perform the sending of an initialization message.
        /// </summary>
        /// <param name="initialize_message">The initialization message to sent</param>
        /// <param name="bFromScript">true if the request comes from a Script, false for a session </param>
        /// <returns></returns>
        protected bool PerformInitializeRequest(string initialize_message, bool bFromScript)
        {
            JObject jsonObject = null;
            //1) If the script contains an initialize request use it, otherwise use the default one            
            string init_message = initialize_message ?? Utilities.Protocol.DEFAULT_INITIALIZE;
            if (!Utilities.Protocol.IsInitializeRequest(init_message, out jsonObject))
            {
                string message = string.Format(Resource.FailClientInitializeMessage, init_message);
                this.LogWriter?.WriteLine(message);
                System.Console.Out.WriteLine(message);
                if (bFromScript)
                {
                    WriteReplayResult(message);
                }
                ControllerState = ConnectionState.Closed;//CLOSED
                return false;
            }
            ResponseResultOrError initTaskResponse = SyncReplayRequest(init_message, LanguageServer.Protocol.InitializeRequest.Type, jsonObject);
            ResponseResultOrError response = initTaskResponse;
            if (response.code.HasValue && response.code != 0)
            {//Server initialization error
                string message = string.Format(Resource.ServerInitializeError, response.code, response.message);
                this.LogWriter?.WriteLine(message);
                System.Console.Out.WriteLine(message);
                ForceServerShutDown(message);
                ControllerState = ConnectionState.Closed;//CLOSED                
                return false;
            }
            //Send an Initialized notification
            base.Consume(Utilities.Protocol.DEFAULT_INITIALIZED);
            return true;
        }

        /// <summary>
        /// Forces the server to shutdown
        /// </summary>
        /// <param name="message">any shutdown exception message to be reported</param>
        protected virtual void ForceServerShutDown(string message = null)
        {
            WriteReplayResult(message);
            //This by sending Shutdown request and notificaion
            Task<ResponseResultOrError> initTaskResponse = AsyncReplayRequest(Utilities.Protocol.DEFAULT_SHUTDOWN, LanguageServer.Protocol.ShutdownRequest.Type);
            base.Consume(Utilities.Protocol.DEFAULT_EXIT);
        }

        /// <summary>
        /// Replay a request
        /// </summary>
        /// <param name="message">The message corresponding to the request</param>
        /// <param name="jsonObject">The Json object corresponding to the request if any</param>
        /// <returns>The ResponseResultOrError instance of the message</returns>
        private async Task<ResponseResultOrError> AsyncReplayRequest(string message, RequestType requestType, JObject jsonObject = null)
        {
            System.Diagnostics.Contracts.Contract.Requires(message != null);
            if (jsonObject != null)
            {
                System.Diagnostics.Contracts.Contract.Requires(Utilities.Protocol.IsRequest(message, out jsonObject));
            }
            else
            {
                System.Diagnostics.Contracts.Contract.Requires(Utilities.Protocol.IsRequest(jsonObject));
            }
            string requestId = Utilities.Protocol.GetRequestId(jsonObject);
            TaskCompletionSource<ResponseResultOrError> taskCompletionSource = new TaskCompletionSource<ResponseResultOrError>();
            ResponseWaitState responseWaitState = new ResponseWaitState(requestType, requestId, taskCompletionSource);
            responsesExpected.Add(requestId, responseWaitState);
            base.Consume(message);
            //Wait Sever Initialize response
            await taskCompletionSource.Task;
            ResponseResultOrError response = taskCompletionSource.Task.Result;
            return response;
        }

        /// <summary>
        /// Synchronous Replay a request
        /// </summary>
        /// <param name="message">The message corresponding to the request</param>
        /// <param name="jsonObject">The Json object corresponding to the request if any</param>
        /// <returns>The ResponseResultOrError instance of the message</returns>
        protected ResponseResultOrError SyncReplayRequest(string message, RequestType requestType, JObject jsonObject = null)
        {
            System.Diagnostics.Contracts.Contract.Requires(message != null);
            if (jsonObject != null)
            {
                System.Diagnostics.Contracts.Contract.Requires(Utilities.Protocol.IsRequest(message, out jsonObject));
            }
            else
            {
                System.Diagnostics.Contracts.Contract.Requires(Utilities.Protocol.IsRequest(jsonObject));
            }
            string requestId = Utilities.Protocol.GetRequestId(jsonObject);
            TaskCompletionSource<ResponseResultOrError> taskCompletionSource = new TaskCompletionSource<ResponseResultOrError>();
            ResponseWaitState responseWaitState = new ResponseWaitState(requestType, requestId, taskCompletionSource);
            responsesExpected.Add(requestId, responseWaitState);
            base.Consume(message);
            ResponseResultOrError response = taskCompletionSource.Task.Result;
            return response;
        }

        /// <summary>
        /// Replay a message 
        /// </summary>
        /// <param name="message">The message to replay</param>
        /// <returns>If the message is a request this methods returns the response of the request, null otherwise</returns>
        protected ResponseResultOrError ReplayMessage(string message)
        {
            JObject jsonObject = null;
            if (Utilities.Protocol.IsRequest(message, out jsonObject))
            {
                ResponseResultOrError response = SyncReplayRequest(message, null, jsonObject);
                return response;
            }
            else
            {
                base.Consume(message);
            }
            return null;
        }

        /// <summary>
        /// Handle a notification event from the ReplayController
        /// </summary>
        /// <param name="sender">Sender of the message in fact the ReplayController</param>
        /// <param name="e">The response message.</param>
        protected void ReplayControllerNotificationEvent(object sender, Tuple<string, JObject> message_jsonObject)
        {
            System.Diagnostics.Contracts.Contract.Requires(Utilities.Protocol.IsNotification(message_jsonObject.Item2));
            string uri = null;
            if (Utilities.Protocol.IsMessageWithUri(message_jsonObject.Item2, out uri))
            {
                lock (InCommingNotification)
                {
                    InCommingNotification.Add(message_jsonObject.Item1);
                }
            }
        }

        /// <summary>
        /// Handle a response event from the ReplayController
        /// </summary>
        /// <param name="sender">Sender of the message in fact the ReplayController</param>
        /// <param name="e">The response message.</param>
        protected void ReplayControllerResponseEvent(object sender, Tuple<string, JObject> message_jsonObject)
        {
            System.Diagnostics.Contracts.Contract.Requires(Utilities.Protocol.IsResponse(message_jsonObject.Item2));
            ResponseWaitState responseWaitState = null;
            string requestId = Utilities.Protocol.GetRequestId(message_jsonObject.Item2);
            responsesExpected.TryGetValue(requestId, out responseWaitState);
            if (responseWaitState == null)
            {
                WriteConnectionLog(String.Format("No response was expected for request id \"{0}\"", requestId));
            }
            else
            {
                RequestType requestType = responseWaitState.RequestType;
                //string method = (string)jsonObject["method"];
                //JToken parameters = jsonObject["params"];
                JToken result = message_jsonObject.Item2["result"];
                JToken error = message_jsonObject.Item2["error"];

                object objResult = null;
                if (result != null && requestType != null && requestType.ResultType != null)
                {
                    objResult = result.ToObject(requestType.ResultType);
                }
                object objErrorData = null;
                if (error != null && error["data"] != null && requestType != null && requestType.ErrorDataType != null)
                {
                    objErrorData = error["data"].ToObject(requestType.ErrorDataType);
                }

                ResponseResultOrError resultOrError = new ResponseResultOrError();
                resultOrError.result = objResult;
                if (error != null && error["code"] != null)
                {
                    resultOrError.code = (int)error["code"];
                    resultOrError.message = (string)error["message"];
                    resultOrError.data = objErrorData;
                }

                try
                {
                    responseWaitState.TaskCompletionSource.SetResult(resultOrError);
                }
                catch (Exception e)
                {
                    WriteConnectionLog(String.Format("Task completion for the response expected by request {0} of type {1} failed : {1}", requestId, requestType.GetType().Name, e.Message));
                }
            }
        }

    }
}
