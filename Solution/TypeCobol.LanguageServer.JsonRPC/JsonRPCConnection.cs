using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace TypeCobol.LanguageServer.JsonRPC
{
    /// <summary>
    /// Implementation of a JsonRPC 2.0 message handler
    /// </summary>
    public class JsonRPCConnection : IRPCConnection, IMessageConnection, IMessageHandler, IMessageProducer, IMessageConsumer, IConnectionLog
    {
        public JsonRPCConnection(IMessageConnection messageConnection)
        {
            System.Diagnostics.Contracts.Contract.Assert(messageConnection != null);
            if (messageConnection == null)
                throw new NullReferenceException("messageConnection is null");
            this.MessageConnection = messageConnection;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        // Message server used to send Remote Procedure Calls to the client
        public IMessageConnection MessageConnection
        {
            get;
            protected set;
        }

        /// <summary>
        /// The Message Connection state
        /// </summary>
        public virtual ConnectionState State
        {
            get
            {
                return MessageConnection != null ? MessageConnection.State : ConnectionState.Disposed;
            }
        }

        /// <summary>
        /// Add a State Change Event handler.
        /// </summary>
        public virtual void AddStageChangedEventHandler(EventHandler handler)
        {
            MessageConnection?.AddStageChangedEventHandler(handler);
        }

        /// <summary>
        /// Remove a State Change Event Handler. 
        /// </summary>
        /// <param name="handler"></param>
        public virtual void RemoveStageChangedEventHandler(EventHandler handler)
        {
            MessageConnection?.RemoveStageChangedEventHandler(handler);
        }

        // Notification methods supported by this RPC server
        protected class NotificationMethod { public NotificationType Type; public NotificationHandler HandleNotification; }
        protected IDictionary<string, NotificationMethod> notificationMethods = new Dictionary<string, NotificationMethod>();
        // Request methods supported by this RPC server
        protected class RequestMethod { public RequestType Type; public RequestHandler HandleRequest; }
        protected IDictionary<string, RequestMethod> requestMethods = new Dictionary<string, RequestMethod>();

        /// <summary>
        /// Register a description of all the notification methods supported by the RPC server
        /// </summary>
        public void RegisterNotificationMethod(NotificationType notificationType, NotificationHandler notificationHandler)
        {
            notificationMethods.Add(notificationType.Method, new NotificationMethod() { Type = notificationType, HandleNotification = notificationHandler });
        }

        /// <summary>
        /// Register a description of all the request methods supported by the RPC server
        /// </summary>
        public void RegisterRequestMethod(RequestType requestType, RequestHandler requestHandler)
        {
            requestMethods.Add(requestType.Method, new RequestMethod() { Type = requestType, HandleRequest = requestHandler });
        }

        /// <summary>
        /// Create a notification
        /// </summary>
        /// <param name="notificationType"></param>
        /// <param name="parameters"></param>
        /// <param name="jsonMessage"></param>
        /// <returns></returns>
        public static string CreateNotification(NotificationType notificationType, object parameters, out JObject jsonMessage)
        {
            jsonMessage = new JObject();
            PrepareJsonPRCMessage(jsonMessage);

            jsonMessage["method"] = notificationType.Method;
            if (parameters != null)
            {
                jsonMessage["params"] = JToken.FromObject(parameters);
            }
            return jsonMessage.ToString(Formatting.None);
        }

        /// <summary>
        /// Send a notification to the client
        /// </summary>
        public void SendNotification(NotificationType notificationType, object parameters)
        {
            JObject jsonMessage = null;
            string message = CreateNotification(notificationType, parameters, out jsonMessage);
            SendMessage(message);
        }

        // Add Json RPC standard property
        protected static void PrepareJsonPRCMessage(JObject jsonMessage)
        {
            jsonMessage["jsonrpc"] = "2.0";
        }

        // Sequence number used to generate unique identifiers for the requests and responses
        private int sequenceNumber;

        // Remeber all requests sent and still waiting for a response 
        protected IDictionary<string, ResponseWaitState> responsesExpected = new Dictionary<string, ResponseWaitState>();

        public string CreateRequest(RequestType requestType, object parameters, out string requestId, out JObject jsonMessage)
        {
            jsonMessage = new JObject();
            PrepareJsonPRCMessage(jsonMessage);

            // Generate a unique id for the request
            int id = Interlocked.Increment(ref sequenceNumber);
            requestId = id.ToString();
            jsonMessage["id"] = requestId;

            jsonMessage["method"] = requestType.Method;
            if (parameters != null)
            {
                jsonMessage["params"] = JToken.FromObject(parameters);
            }
            return jsonMessage.ToString(Formatting.None);
        }

        /// <summary>
        /// Send an async request to the client and await later for the response or error
        /// </summary>
        public virtual Task<ResponseResultOrError> SendRequest(RequestType requestType, object parameters)
        {
            JObject jsonMessage = null;
            string requestId = "";
            string message = CreateRequest(requestType, parameters, out requestId, out jsonMessage);
            //  Send text message
            SendMessage(message);

            // Remember all elements which will be needed to handle correctly the response to the request
            TaskCompletionSource<ResponseResultOrError> taskCompletionSource = new TaskCompletionSource<ResponseResultOrError>();
            ResponseWaitState responseWaitState = new ResponseWaitState(requestType, requestId, taskCompletionSource);
            responsesExpected.Add(requestId, responseWaitState);

            // The completion of the task will be signaled later, when the response arrives
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Producer message filter if any
        /// </summary>
        ProducerMessageFilter m_MessageFilter;
        public ProducerMessageFilter ProducedMessageFilter
        {
            get
            {
                return Interlocked.Exchange<ProducerMessageFilter>(ref m_MessageFilter, m_MessageFilter);
            }
            set
            {
                Interlocked.Exchange<ProducerMessageFilter>(ref m_MessageFilter, value);
            }
        }
        /// <summary>
        /// Implementation of IMessageHandler
        /// </summary>
        public virtual void HandleMessage(string message, IMessageConnection server)
        {
            // If a filter has captured the message --> then ignore it
            ProducerMessageFilter filter = ProducedMessageFilter;
            bool bHandled = filter != null ? filter(message, server) : false;
            if (bHandled)
                return;

            JObject jsonObject = JObject.Parse(message);

            // Try to read the JsonRPC message properties
            string requestId = (string)jsonObject["id"];
            string method = (string)jsonObject["method"];
            JToken parameters = jsonObject["params"];
            JToken result = jsonObject["result"];
            JToken error = jsonObject["error"];

            // Check message type
            // -- Notification --
            if (requestId == null && method != null)
            {
                HandleNotification(method, parameters);
            }
            // -- Request --
            else if (requestId != null && method != null)
            {
                HandleRequest(method, requestId, parameters);
            }
            // -- Response --
            else if (requestId != null && (result != null || error != null))
            {
                HandleResponse(requestId, result, error);
            }
        }

        protected virtual void HandleNotification(string method, JToken parameters)
        {
            NotificationMethod notificationMethod = null;
            notificationMethods.TryGetValue(method, out notificationMethod);
            if (notificationMethod == null)
            {
                WriteConnectionLog(String.Format("No notification handler was registered for method \"{0}\"", method));
            }
            else
            {
                NotificationType notificationType = notificationMethod.Type;
                object objParams = null;
                if (parameters != null)
                {
                    objParams = parameters.ToObject(notificationType.ParamsType);
                }
                try
                {
                    notificationMethod.HandleNotification(notificationType, objParams);
                }
                catch (Exception e)
                {
                    WriteConnectionLog(String.Format("Notification handler for {0} failed : {1}", notificationType.GetType().Name, e.Message));
                    ResponseResultOrError error = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message, data = parameters.ToString() };
                    Reply(method, error);
                }
            }
        }

        protected virtual void HandleRequest(string method, string requestId, JToken parameters)
        {
            RequestMethod requestMethod = null;
            requestMethods.TryGetValue(method, out requestMethod);
            if (requestMethod == null)
            {
                WriteConnectionLog(String.Format("No request handler was registered for method \"{0}\"", method));
            }
            else
            {
                RequestType requestType = requestMethod.Type;
                object objParams = null;
                if (parameters != null)
                {
                    objParams = parameters.ToObject(requestType.ParamsType);
                }
                try
                {
                    ResponseResultOrError resultOrError = requestMethod.HandleRequest(requestType, objParams);
                    Reply(requestId, resultOrError);
                }
                catch (Exception e)
                {
                    ResponseResultOrError error = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
                    Reply(requestId, error);
                }
            }
        }

        protected virtual void Reply(string requestId, ResponseResultOrError resultOrError)
        {
            JObject jsonMessage = new JObject();
            PrepareJsonPRCMessage(jsonMessage);

            // Response properties
            jsonMessage["id"] = requestId;
            if (resultOrError.result != null)
            {
                jsonMessage["result"] = JToken.FromObject(resultOrError.result);
            }
            else if (resultOrError.code != null)
            {
                jsonMessage["error"] = JToken.FromObject(resultOrError);
            }

            //  Send text message
            SendMessage(jsonMessage.ToString(Formatting.None));
        }

        protected virtual void HandleResponse(string requestId, JToken result, JToken error)
        {
            ResponseWaitState responseWaitState = null;
            responsesExpected.TryGetValue(requestId, out responseWaitState);
            if (responseWaitState == null)
            {
                WriteConnectionLog(String.Format("No response was expected for request id \"{0}\"", requestId));
            }
            else
            {
                //So remove It
                responsesExpected.Remove(requestId);
                RequestType requestType = responseWaitState.RequestType;
                object objResult = null;
                if (result != null && requestType.ResultType != null)
                {
                    objResult = result.ToObject(requestType.ResultType);
                }
                object objErrorData = null;
                if (error != null && error["data"] != null && requestType.ErrorDataType != null)
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
        /// IMessageConsumer Consume method implementation
        /// </summary>
        /// <param name="message">The message to consume</param>
        public void Consume(string message)
        {
            this.HandleMessage(message, this.MessageConnection);
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

        /// <summary>
        /// Write a trace in the server log file
        /// </summary>
        public void WriteConnectionLog(string trace)
        {
            LogWriter?.WriteLine(trace);
        }

        /// <summary>
        /// Starts the connection
        /// </summary>
        /// <returns>The connection's listener task if any, null otherwise</returns>
        public virtual Task<bool> Start()
        {
            return Start(this);
        }

        /// <summary>
        /// Starts the connection, which dispatch received messages to the given message consumer instance.
        /// </summary>
        /// <param name="messageConsumer">The message consumer instance</param>
        /// <returns>The connection's listener task if any, null otherwise</returns>
        public virtual Task<bool> Start(IMessageConsumer messageConsumer)
        {
            System.Diagnostics.Contracts.Contract.Assert(messageConsumer != null);
            System.Diagnostics.Contracts.Contract.Assert(this.MessageConnection != null);
            if (this.MessageConnection != null)
            {
                return this.MessageConnection.Start(messageConsumer);
            }
            return null;
        }

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        public virtual void SendMessage(string message)
        {
            System.Diagnostics.Contracts.Contract.Assert(this.MessageConnection != null);
            if (this.MessageConnection != null)
            {
                this.MessageConnection.SendMessage(message);
            }
        }

        /// <summary>
        /// JonRPCConnection implements IMessageProducer.
        /// </summary>
        /// <param name="messageConsumer"></param>
        /// <returns></returns>
        public Task<bool> Listen(IMessageConsumer messageConsumer)
        {
            return Start(messageConsumer);
        }

        /// <summary>
        /// Propagate Connection Log settings to this.
        /// </summary>
        /// <param name="log">The Connection Logs setting.</param>
        public void PropagateConnectionLogs(ConnectionLog log = null)
        {
            log = log ?? ConnectionLog.GetInstance();
            log.AssignTo(this);
            if (this.MessageConnection is LanguageServer.JsonRPC.MessageConnection)
            {
                (this.MessageConnection as LanguageServer.JsonRPC.MessageConnection).PropagateConnectionLogs(log);
            }            
        }

        public virtual void Dispose()
        {
            this.MessageConnection?.Dispose();
        }
    }
}
