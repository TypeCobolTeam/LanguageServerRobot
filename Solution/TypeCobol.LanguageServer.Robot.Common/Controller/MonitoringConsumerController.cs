using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TypeCobol.LanguageServer.Robot.Common.Connection;
using TypeCobol.LanguageServer.Robot.Common.Model;
using Newtonsoft.Json.Linq;

namespace TypeCobol.LanguageServer.Robot.Common.Controller
{
    /// <summary>
    /// A Monitoring consumer class
    /// </summary>
    public class MonitoringConsumerController : RecordingModeController
    {
        /// <summary>
        /// Consummer controller result state
        /// </summary>
        public enum ConnectionState
        {
            /// <summary>
            /// Echec of connection
            /// </summary>
            ConnectionFailed,
            /// <summary>
            /// The Connection has been released
            /// </summary>
            ConnectionReleased,
        }

        /// <summary>
        /// The Pipe's name to use
        /// </summary>
        public string PipeName
        {
            get;
            set;
        }

        /// <summary>
        /// Monitoring Data Connection
        /// </summary>
        public IDataConnection DataConnection
        {
            get;
            private set;
        }

        /// <summary>
        /// Delegate to represent an Event Handler for a RecordedMessage in a script.
        /// </summary>
        /// <param name="script">The script in to which the message is registered</param>
        /// <param name="category">The message's category</param>
        /// <param name="kind">The kind of message</param>
        /// <param name="message">The registered message</param>
        /// <param name="uri">The Script's uri</param>
        /// <param name="jsonObject">The Message JSON object</param>
        public delegate void RecordedMessageEventhandler(Script script, Model.Script.MessageCategory category, Utilities.Protocol.Message_Kind kind, string message, string uri, JObject jsonObject);

        /// <summary>
        /// Lsp Message Handler
        /// </summary>
        public event EventHandler<Model.Message.LspMessage> LspMessageHandler;
        /// <summary>
        /// Command Message Handler
        /// </summary>
        public event EventHandler<Model.Message> CommandMessageHandler;
        /// <summary>
        /// Acknowledgment Message Handler
        /// </summary>
        public event EventHandler<Model.Message> AcknowledgmentMessageHandler;
        /// <summary>
        /// Event when a new session has been Initialized.
        /// </summary>
        public event EventHandler<Model.Session> StartSessionHandler;
        /// <summary>
        /// Event when a session has been stopped.
        /// </summary>
        public event EventHandler<Model.Session> StopSessionHandler;
        /// <summary>
        /// Event when a new document has been started.
        /// </summary>
        public event EventHandler<Model.Script> StartDocumentHandler;
        /// <summary>
        /// Event when a message has been recorded in a script.
        /// </summary>
        public event RecordedMessageEventhandler RecordedMessageHandler;
        /// <summary>
        /// Event when a document has been stopped.
        /// </summary>
        public event EventHandler<Model.Script> StopDocumentHandler;

        /// <summary>
        /// Dispatch a message.
        /// </summary>
        /// <param name="message"></param>
        private void DispatchMessage(Model.Message message)
        {
            if (message == null)
                return;
            switch(message.Kind)
            {
                case Model.Message.MessageKind.Acknowledgment:
                    {
                        AcknowledgmentMessageHandler?.Invoke(this, message);
                    }
                    break;
                case Model.Message.MessageKind.Command:
                    {
                        CommandMessageHandler?.Invoke(this, message);
                    }
                    break;
                case Model.Message.MessageKind.Lsp:
                    {
                        LspMessageHandler?.Invoke(this, (Model.Message.LspMessage)message);

                        Model.Message.LspMessage lsp_message = (Model.Message.LspMessage)message;
                        switch(lsp_message.From)
                        {
                            case Model.Message.LspMessage.MessageFrom.Client:
                                this.FromClient(lsp_message.Message);
                                break;
                            case Model.Message.LspMessage.MessageFrom.Server:
                                this.FromServer(lsp_message.Message);
                                break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Pipe Name constructor.
        /// </summary>
        /// <param name="pipeName">Pipe's name</param>
        public MonitoringConsumerController(string pipeName, string scriptRepositoryPath = null) : base(scriptRepositoryPath)
        {
            System.Diagnostics.Debug.Assert(pipeName != null);
            PipeName = pipeName;
            DataConnection = DataConnectionfactory.Create(DataConnectionfactory.ConnectionType.PIPE, DataConnectionfactory.ConnectionSide.Consumer);
        }

        /// <summary>
        /// Start Consumming
        /// </summary>
        /// <returns></returns>
        public async Task<ConnectionState> Start()
        {            
            if (DataConnection.OpenConnection(PipeName))
            {
                for (;;)
                {
                    Model.Message message = null;
                    //Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " - Starting blocking read");
                    try
                    {
                        message = (Model.Message)DataConnection.ReadData();
                        if (message == null)
                            break;
                        DispatchMessage(message);
                    }
                    catch (SerializationException se)
                    {
                        break;
                    }
                }
                return ConnectionState.ConnectionReleased;
            }
            else
            {//Failed to open the data connection.
                return ConnectionState.ConnectionFailed;
            }
        }

        protected override bool InitializeSession(string message, JObject jsonObject)
        {
            bool bResult = base.InitializeSession(message, jsonObject);
            if (bResult)
            {   //Raise a Start session event.
                if (StartSessionHandler != null)
                    StartSessionHandler(this, this.SessionModel);
            }
            return bResult;
        }

        protected override Script StartScript(string message, JObject jsonObject)
        {
            Script script = base.StartScript(message, jsonObject);
            if (script != null)
            {
                if (StartDocumentHandler != null)
                    StartDocumentHandler(this, script);
            }
            return script;
        }

        protected override bool RecordScriptMessage(Script.MessageCategory category, Utilities.Protocol.Message_Kind kind,
            string message, string uri, JObject jsonObject)
        {
            if (base.RecordScriptMessage(category, kind, message, uri, jsonObject))
            {
                if (this.RecordedMessageHandler != null)
                {
                    Script script = base[uri];
                    this.RecordedMessageHandler(script, category, kind, message, uri, jsonObject);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Save a script in a file using UTF-8 encoding.
        /// </summary>
        /// <param name="script">The script to be saved</param>
        /// <param name="scriptFile">The script file path</param>
        internal override bool SaveScript(Script script, string scriptFile)
        {
            bool bResult = base.SaveScript(script, scriptFile);
            StopDocumentHandler?.Invoke(this, script);
            return bResult;
        }
    }
}
