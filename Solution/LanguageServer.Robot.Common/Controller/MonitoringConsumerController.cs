using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.Robot.Common.Connection;

namespace LanguageServer.Robot.Common.Controller
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
    }
}
