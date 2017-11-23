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
    /// A Client controller which is able to execute a script
    /// </summary>
    public class ScriptRobotConnectionController : ClientRobotConnectionController
    {
        /// <summary>
        /// Script Connection constructor
        /// </summary>
        /// <param name="script">The script to be executed by this Client</param>
        public ScriptRobotConnectionController(Model.Script script)
        {
            this.Script = script;
            ControllerState = ConnectionState.New;
        }

        /// <summary>
        /// The script to be executed by this controller
        /// </summary>
        public Model.Script Script
        {
            get;
            private set;
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
        private ConnectionState ControllerState
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
        /// Determine if yes or not we must stop At the first error.
        /// </summary>
        public bool StopAtFirstError
        {
            get
            {
                return ReplayController != null ? ReplayController.StopAtFirstError : false;
            }
            set
            {
                if (ReplayController != null)
                    ReplayController.StopAtFirstError = value;

            }
        }

        /// <summary>
        /// The Index of the firt error encountered in the Script.
        /// </summary>
        public long ErrorIndex
        {
            get
            {
                return ReplayController != null ? ReplayController.ErrorIndex : -1;
            }
        }

        event EventHandler StageChangedEvent;

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

        //Forces the server to shutdown
        private void ForceServerShutDown()
        {
            //This by sending Shutdown request and notificaion
            Task<ResponseResultOrError> initTaskResponse = ReplayRequest(Utilities.Protocol.DEFAULT_SHUTDOWN);
            base.Consume(Utilities.Protocol.DEFAULT_EXIT);
        }

        /// <summary>
        /// Replay a request
        /// </summary>
        /// <param name="message">The message corresponding to the request</param>
        /// <param name="jsonObject">The Json object corresponding to the request if any</param>
        /// <returns>The ResponseResultOrError instance of the message</returns>
        private async Task<ResponseResultOrError> ReplayRequest(string message, JObject jsonObject = null)
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
            ResponseWaitState responseWaitState = new ResponseWaitState(null, requestId, taskCompletionSource);
            responsesExpected.Add(requestId, responseWaitState);
            base.Consume(message);
            //Wait Sever Initialize response
            await taskCompletionSource.Task;
            ResponseResultOrError response = taskCompletionSource.Task.Result;
            return response;
        }
        /// <summary>
        /// Replay a message 
        /// </summary>
        /// <param name="message">The message to replay</param>
        /// <returns>If the message is a request this methods returns the response of the request, null otherwise</returns>
        private Task<ResponseResultOrError> ReplayMessage(string message)
        {
            JObject jsonObject = null;
            if (!Utilities.Protocol.IsRequest(message, out jsonObject))
            {
                Task<ResponseResultOrError> response = ReplayRequest(message, jsonObject);
                return response;
            }
            else
            {
                base.Consume(message);
            }
            return null;
        }
        /// <summary>
        /// Replay the message contained in the Script;
        /// </summary>
        /// <returns>Return true if all messages have been replayed, false otherwise.</returns>
        private bool ReplayMessages()
        {
            if (Script == null)
                return true;//No script => No message.
            if (!Script.IsValid)
                return false;
            //1) Send the "textDocument/didOpen" notification
            base.Consume(Script.didOpen);
            //2) Run thru all messages and take in account messages that are client requests or notifications.
            for(int i = 0; i < Script.messages.Count && ReplayController.ErrorIndex < 0; i++)
            {
                if (Script.messages[i].category == Model.Script.MessageCategory.Client)
                {
                    ReplayController.ScriptMessageIndex = i;
                    Task<ResponseResultOrError> response = ReplayMessage(Script.messages[i].message);
                }                
            }
            //3) Close the document
            if (ReplayController.ErrorIndex < 0)
                base.Consume(Script.didClose);
            return ReplayController.ErrorIndex < 0;
        }
        /// <summary>
        /// Starts the connection
        /// </summary>
        /// <returns>The connection's listener task if any, null otherwise</returns>
        public override async Task<bool> Start()
        {
            //We are replaying a script.
            if (Script == null)
            {
                ControllerState = ConnectionState.Disposed;
                return false;
            }

            ControllerState = ConnectionState.Listening;

            JObject jsonObject = null;
            //1) If the script contains an initialize request use it, otherwise use the default one            
            string init_message = Script.initialize ?? Utilities.Protocol.DEFAULT_INITIALIZE;
            if (!Utilities.Protocol.IsInitializeRequest(init_message, out jsonObject))
            {
                this.LogWriter?.WriteLine(string.Format(Resource.FailClientInitializeMessage, init_message));
                System.Console.Out.WriteLine(string.Format(Resource.FailClientInitializeMessage, init_message));
                ForceServerShutDown();
                ControllerState = ConnectionState.Closed;//CLOSED
                return false;
            }
            Task<ResponseResultOrError> initTaskResponse = ReplayRequest(init_message, jsonObject);
            ResponseResultOrError response = initTaskResponse.Result;
            if (response.code != 0)
            {//Server initialization error
                this.LogWriter?.WriteLine(string.Format(Resource.ServerInitializeError, response.code, response.message));
                System.Console.Out.WriteLine(string.Format(Resource.ServerInitializeError, response.code, response.message));
                ForceServerShutDown();
                ControllerState = ConnectionState.Closed;//CLOSED
                return false;
            }
            //Send an Initialized notification
            base.Consume(Utilities.Protocol.DEFAULT_INITIALIZED);

            //2) Replay all client messages
            Task<bool> replayTask = new Task<bool>(() => { return ReplayMessages();  });
            replayTask.Start();
            bool bResult = await replayTask;

            //3) Shutdown and exit
            ForceServerShutDown();

            ControllerState = ConnectionState.Closed;//CLOSED
            return bResult;
        }

        /// <summary>
        /// Send a message to the target client, in replaying mode the target client its us
        /// and the message is a message from the server.
        /// </summary>
        /// <param name="message"></param>
        public override void SendMessage(string message)
        {   //We capture any message to be send back to a dummy client that is to say us.
            //We only take care abour responses.
            JObject jsonObject = null;
            if (Utilities.Protocol.IsResponse(message, out jsonObject))
            {
                ResponseWaitState responseWaitState = null;
                string requestId = Utilities.Protocol.GetRequestId(jsonObject);
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
                    JToken result = jsonObject["result"];
                    JToken error = jsonObject["error"];

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
}
