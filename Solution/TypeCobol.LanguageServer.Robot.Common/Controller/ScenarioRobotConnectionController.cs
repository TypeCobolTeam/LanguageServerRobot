using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TypeCobol.LanguageServer.JsonRPC;
using TypeCobol.LanguageServer.Protocol;
using TypeCobol.LanguageServer.Robot.Common.Model;
using Newtonsoft.Json.Linq;

namespace TypeCobol.LanguageServer.Robot.Common.Controller
{
    /// <summary>
    /// A Controller for recording a specific scenario.
    /// </summary>
    public class ScenarioRobotConnectionController : ClientRobotConnectionController
    {
        /// <summary>
        /// Message Connection constructor.
        /// </summary>
        /// <param name="messageConnection"></param>
        public ScenarioRobotConnectionController(IMessageConnection messageConnection) : base(messageConnection)
        {
            base.ProducedMessageFilter = ClientProducedMessageFilter;
        }

        /// <summary>
        /// Did Open parameters as received from the didOpen notification.
        /// </summary>
        public DidOpenTextDocumentParams DidOpenParameters
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Did Close Text Document notification if any have been seen
        /// </summary>
        public string DidCloseTextDocument
        {
            get;
            internal set;
        }

        /// <summary>
        /// Any Shtudown requet seen
        /// </summary>
        public string ShutdownRequest
        {
            get; internal set;
        }

        /// <summary>
        /// Any Exit Notification.
        /// </summary>
        public string ExitNotification
        {
            get; internal set;
        }

        /// <summary>
        /// The Session
        /// </summary>
        public Model.Session Session
        {
            get; internal set;
        }

        public Model.Script Script
        {
            get; internal set;
        }

        /// <summary>
        /// Get the Recording Mode Contoller.
        /// </summary>
        internal RecordingModeController RecordingController
        {
            get
            {
                return (RecordingModeController)base.RobotModeController;
            }
        }

        /// <summary>
        /// Connect or disconnect message event hnadlers from the REplya Controller.
        /// </summary>
        /// <param name="bConnect">true to connect , false to disconnect.</param>
        protected virtual void ConnectMessageEventHandlers(bool bConnect)
        {
            if (bConnect)
            {
                this.RecordingController.ResponseEvent += RecordingControllerResponseEvent;                
            }
            else
            {
                this.RecordingController.ResponseEvent -= RecordingControllerResponseEvent;                
            }
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
        /// Handle a response event from the RecordingController
        /// </summary>
        /// <param name="sender">Sender of the message in fact the ReplayController</param>
        /// <param name="e">The response message.</param>
        protected void RecordingControllerResponseEvent(object sender, Tuple<string, JObject> message_jsonObject)
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

        /// <summary>
        /// Perform the sending of an initialization message.
        /// </summary>
        /// <param name="initialize_message">The initialization message to sent</param>
        /// <returns></returns>
        protected bool PerformInitializeRequest(string initialize_message)
        {
            JObject jsonObject = null;
            //1) If the script contains an initialize request use it, otherwise use the default one            
            string init_message = initialize_message ?? Utilities.Protocol.DEFAULT_INITIALIZE;
            if (!Utilities.Protocol.IsInitializeRequest(init_message, out jsonObject))
            {
                string message = string.Format(Resource.FailClientInitializeMessage, init_message);
                this.LogWriter?.WriteLine(message);
                System.Console.Out.WriteLine(message);
                return false;
            }
            ResponseResultOrError initTaskResponse = SyncReplayRequest(init_message, LanguageServer.Protocol.InitializeRequest.Type, jsonObject);
            ResponseResultOrError response = initTaskResponse;
            if (response.code.HasValue && response.code != 0)
            {//Server initialization error
                string message = string.Format(Resource.ServerInitializeError, response.code, response.message);
                this.LogWriter?.WriteLine(message);
                System.Console.Out.WriteLine(message);
                return false;
            }
            //Send an Initialized notification
            base.Consume(Utilities.Protocol.DEFAULT_INITIALIZED);
            return true;
        }

        /// <summary>
        /// Initialize a Scenario. This means that we must send the initialize request, any didchangeConfiguration.
        /// Replay and didSave and textChange notification to this point.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="script"></param>
        /// <param name="lastSaveIndex">[out]lastSaveIndex Last Save Index position</param>
        /// <param name="bUseLastSave">true if the last save position must be used, false otherwise</param>
        public bool InitializeScenario(Model.Session session, Model.Script script, out int lastSaveIndex, bool bUseLastSave = true)
        {
            //Don't save the script when the didClose notification arrive.
            lastSaveIndex = -1;
            this.RecordingController.IsSaveOnDidClose = false;
            ConnectMessageEventHandlers(true);
            this.Session = session;
            this.Script = script;
            if (session.initialize != null)
            {
                if (!PerformInitializeRequest(session.initialize))
                {
                    return false;
                }
                if (session.did_change_configuation != null)
                {
                    this.Consume(session.did_change_configuation);
                }
                //1Send the "textDocument/didOpen" notification
                if (script.didOpen != null)
                {
                    JObject jsonJObject = null;
                    if (Utilities.Protocol.IsDidOpenTextDocumentNotification(script.didOpen, out jsonJObject))
                    {
                        NotificationType notificationType = DidOpenTextDocumentNotification.Type;
                        object objParams = null;
                        JToken parameters = jsonJObject["params"];
                        if (parameters != null)
                        {
                            objParams = parameters.ToObject(notificationType.ParamsType);
                        }
                        DidOpenParameters = (DidOpenTextDocumentParams) objParams;
                        base.Consume(Script.didOpen);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                //Lookup the position of the last didSave message.
                lastSaveIndex = bUseLastSave ? LookupLastSavePosition(script) : 0;
                //Apply didOpen all text changes from the beginning.
                for (int i = lastSaveIndex; i < script.messages.Count; i++)
                {
                    var msg = script.messages[i];
                    if (msg.category == Script.MessageCategory.Client)
                    {
                        JObject jsonJObject = null;
                        if (Utilities.Protocol.IsDidSaveTextDocumentNotification(msg.message, out jsonJObject))
                        {
                            this.Consume(msg.message);
                        }
                        else if (Utilities.Protocol.IsDidChangeTextDocumentNotification(jsonJObject))
                        {
                            this.Consume(msg.message);
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add a LSP message to the scenario
        /// </summary>
        /// <param name="script">The Script associated to the message</param>
        /// <param name="msg">The Lsp Message to be added</param>
        /// <param name="bWaitRequestMode">true if we must wait for request return Id, false otherwise.</param>
        public void AddMessage(Model.Script script, Model.Message.LspMessage msg, bool bWaitRequestMode)
        {
            if (this.Script == script && msg.From == Message.LspMessage.MessageFrom.Client)
            {
                AddMessage(script, msg.Message, bWaitRequestMode);
            }
        }

        /// <summary>
        /// Add a message to the scenario
        /// </summary>
        /// <param name="script">The Script associated to the message</param>
        /// <param name="msg">The message to be added</param>
        /// <param name="bWaitRequestMode">true if we must wait for request return Id, false otherwise.</param>
        public void AddMessage(Model.Script script, string msg, bool bWaitRequestMode)
        {
            JObject jsonObject = null;
            if (bWaitRequestMode)
            {
                if (Utilities.Protocol.IsRequest(msg, out jsonObject))
                {
                    this.SyncReplayRequest(msg, null, jsonObject);
                }
                else
                {
                    this.Consume(msg);
                }
            }
            else
            {
                this.Consume(msg);
            }
        }

        /// <summary>
        /// Stop the Scenaro by sending a didClose notification a shutdown request and an exit notification.
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="script">The Original script.</param>
        /// <returns></returns>
        public bool StopScenario(Model.Session session, Model.Script script)
        {
            //Send the close Notification            
            if (DidCloseTextDocument == null)
            {
                JObject jsonObject = null;
                DidCloseTextDocumentParams parameters =
                    new DidCloseTextDocumentParams(new TextDocumentIdentifier(DidOpenParameters.textDocument.uri));
                string message = CreateNotification(DidCloseTextDocumentNotification.Type, parameters, out jsonObject);
                DidCloseTextDocument = message;
                this.Consume(message);
            }
            else
            {
                this.Consume(DidCloseTextDocument);
            }

            //Send the shutdown request         
            if (this.ShutdownRequest != null)   
                base.Consume(Utilities.Protocol.DEFAULT_SHUTDOWN);
            //Send the Exit notification
            if (this.ExitNotification != null)
                base.Consume(Utilities.Protocol.DEFAULT_EXIT);
            return true;
        }

        /// <summary>
        /// Abort the secanario recording.
        /// </summary>
        /// <returns>return true if ok, false otherwise.</returns>
        public bool AbortScenario()
        {
            //Send the shutdown request         
            if (this.ShutdownRequest != null)
                base.Consume(Utilities.Protocol.DEFAULT_SHUTDOWN);
            //Send the Exit notification
            if (this.ExitNotification != null)
                base.Consume(Utilities.Protocol.DEFAULT_EXIT);
            return true;
        }

        /// <summary>
        /// Method to filter messaged produced for the client
        /// </summary>
        /// <param name="message">The mmessage to filter</param>
        /// <param name="connection">The source connection of the message</param>
        /// <returns>true if the message is filtered, false otherwise.</returns>
        private bool ClientProducedMessageFilter(string message, IMessageConnection connection)
        {
            JObject jsonJObject = null;
            if (Utilities.Protocol.IsDidCloseTextDocumentNotification(message, out jsonJObject))
            {
                this.DidCloseTextDocument = message;
            }
            else if (Utilities.Protocol.IsShutdownRequest(jsonJObject))
            {
                ShutdownRequest = message;
            }
            else if (Utilities.Protocol.IsExitNotification(jsonJObject))
            {
                ExitNotification = message;
            }
            this.RobotModeController.FromClient(message);
            connection.SendMessage(message);
            return true;
        }

        /// <summary>
        /// Save the Scenario in a file.
        /// </summary>
        /// <param name="script">The original script from which the scenario was created is created</param>
        /// <param name="filepath">The filepath to save the scenarion</param>
        /// <param name="recordedScenario">(out] the recorded scenario</param>
        /// <returns>true if the scenario has been saved false otherwise</returns>
        public bool SaveScenario(Script script, string filepath, out Script recordedScenario)
        {
            recordedScenario = null;
            if (DidCloseTextDocument != null && DidOpenParameters != null)
            {
                recordedScenario = this.RecordingController[DidOpenParameters.textDocument.uri];                
                if (recordedScenario != null)
                {
                    recordedScenario.uri = (new Uri(filepath)).ToString();
                    FileInfo fi = new FileInfo(filepath);
                    recordedScenario.name = fi.Name;
                    return this.RecordingController.SaveScript(recordedScenario, filepath);
                }
            }            
            return false;
        }

        /// <summary>
        /// Lookup the last save position in the script
        /// </summary>
        /// <param name="script">Last save position</param>
        /// <returns>The index of the last save position in the script's messages.</returns>
        private static int LookupLastSavePosition(Script script)
        {
            //Lookup the position of the last didSave message.
            int lastSaveIndex = 0;
            for (int i = script.messages.Count - 1; i >= 0; i--)
            {
                var msg = script.messages[i];
                if (msg.category == Script.MessageCategory.Client)
                {
                    string text = msg.message;
                    if (text.Contains(DidSaveTextDocumentNotification.Type.Method))
                    {
                        JObject jsonJObject = null;
                        if (Utilities.Protocol.IsDidSaveTextDocumentNotification(text, out jsonJObject))
                        {
                            lastSaveIndex = i;
                            break;
                        }
                    }
                }
            }
            return lastSaveIndex;
        }

        /// <summary>
        /// Create a Snapshot for a script
        /// </summary>
        /// <param name="session">The session to which belongs the script.</param>
        /// <param name="script">The script to create a session</param>
        /// <param name="snapshot">[out] The snapshot script</param>
        /// <returns>true if the snapshot has been created, false</returns>
        public static bool CreateSnapshot(Model.Session session, Model.Script script, out Script snapshot, bool bFromLastSave = false)
        {
            snapshot = null;
            if (session.initialize == null)
            {
                return false;
            }
            if (script.didOpen == null)
            {
                return false;
            }

            DidOpenTextDocumentParams didOpen = null;
            JObject jsonJObject = null;
            if (Utilities.Protocol.IsDidOpenTextDocumentNotification(script.didOpen, out jsonJObject))
            {
                NotificationType notificationType = DidOpenTextDocumentNotification.Type;
                object objParams = null;
                JToken parameters = jsonJObject["params"];
                if (parameters != null)
                {
                    objParams = parameters.ToObject(notificationType.ParamsType);
                }
                didOpen = (DidOpenTextDocumentParams)objParams;
            }
            if (didOpen == null)
                return false;

            snapshot = new Script();
            if (bFromLastSave)
            {
                int lastSaveIndex = LookupLastSavePosition(script);
                List<Script.Message> messages = new List<Script.Message>();
                messages.AddRange(script.messages.Skip(lastSaveIndex));
                snapshot.Copy(script);
                snapshot.messages = messages;
            }
            else
            {
                snapshot.Copy(script, true);
            }
            snapshot.initialize = session.initialize;
            snapshot.initialize_result = session.initialize_result;
            snapshot.did_change_configuation = session.did_change_configuation;

            JObject jsonObject = null;
            DidCloseTextDocumentParams didCloseParameters =
                new DidCloseTextDocumentParams(new TextDocumentIdentifier(didOpen.textDocument.uri));
            string didClosMessage = CreateNotification(DidCloseTextDocumentNotification.Type, didCloseParameters, out jsonObject);
            snapshot.didClose = didClosMessage;
            return true;
        }

        /// <summary>
        /// Record Snapshot using a server connection controller, that is a say the snapshot is created by resending all client messages to the server connection, thus
        /// establishing a real communication with the server.
        /// </summary>
        /// <param name="session">The session to which belongs the script.</param>
        /// <param name="script">The script to create a session</param>
        /// <param name="lastSaveIndex">The last save index position</param>
        /// <param name="scenarioController">The scenario controller in connection with an instantiated server</param>
        /// <returns>true if the snaphot has been recorded, false otherwise.</returns>
        public static bool RecordServerSnapshot(Model.Session session, Model.Script script, int lastSaveIndex, MonitorLanguageServerRobotController scenarioController)
        {
            if (lastSaveIndex < 0)
                lastSaveIndex = LookupLastSavePosition(script);
            if (lastSaveIndex < 0)
                return false;
                //Apply didOpen all text changes from the beginning.
                for (int i = lastSaveIndex; i < script.messages.Count; i++)
            {
                var msg = script.messages[i];
                if (msg.category == Script.MessageCategory.Client)
                {
                    ScenarioRobotConnectionController controller =
                        scenarioController.ClientConnection as ScenarioRobotConnectionController;
                    controller?.AddMessage(script, msg.message, true);
                }
            }
            return true;
        }
    }
}
