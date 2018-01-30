using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.Robot.Common.Connection;

namespace LanguageServer.Robot.Common.Controller
{
    /// <summary>
    /// The Monitoring class for handle monitoring message dispatching from
    /// the consumer point of view.
    /// </summary>
    public class MonitoringConnectionController
    {
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
        public event MonitoringConsumerController.RecordedMessageEventhandler RecordedMessageHandler;
        /// <summary>
        /// Event when a document has been stopped.
        /// </summary>
        public event EventHandler<Model.Script> StopDocumentHandler;

        /// <summary>
        /// The consumer
        /// </summary>
        public MonitoringConsumerController Consumer
        {
            get;
            private set;
        }

        /// <summary>
        /// Connection/Consumer constructor
        /// </summary>
        /// <param name="consumer">The Consumer Controller</param>
        public MonitoringConnectionController(MonitoringConsumerController consumer)
        {
            this.Consumer = consumer;
            ConnectConsumer();
        }

        /// <summary>
        /// Start the asynchronous task of listening to the DataConnection and dispatching
        /// consumer messages.
        /// </summary>
        /// <returns></returns>
        public async Task<MonitoringConsumerController.ConnectionState> Start()
        {
            return await Consumer.Start();
        }

        /// <summary>
        /// Connect to the consumer.
        /// </summary>
        private void ConnectConsumer()
        {
            if (Consumer != null)
            {
                Consumer.LspMessageHandler += Consumer_LspMessageHandler;
                Consumer.CommandMessageHandler += Consumer_CommandMessageHandler;
                Consumer.AcknowledgmentMessageHandler += Consumer_AcknowledgmentMessageHandler;
                Consumer.StartSessionHandler += Consumer_StartSessionHandler;
                Consumer.StopSessionHandler += Consumer_StopSessionHandler;
                Consumer.StartDocumentHandler += Consumer_StartDocumentHandler;
                Consumer.RecordedMessageHandler += Consumer_RecordedMessageHandler;
                Consumer.StopDocumentHandler += Consumer_StopDocumentHandler;
            }
        }

        private void Consumer_StopDocumentHandler(object sender, Model.Script e)
        {
            if (StopDocumentHandler != null)
                StopDocumentHandler(sender, e);
        }

        private void Consumer_RecordedMessageHandler(Model.Script script, Model.Script.MessageCategory category, Utilities.Protocol.Message_Kind kind, string message, string uri, Newtonsoft.Json.Linq.JObject jsonObject)
        {
            if (RecordedMessageHandler != null)
                RecordedMessageHandler(script, category, kind, message, uri, jsonObject);
        }

        private void Consumer_StartDocumentHandler(object sender, Model.Script e)
        {
            if (StartDocumentHandler != null)
                StartDocumentHandler(sender, e);
        }

        private void Consumer_StopSessionHandler(object sender, Model.Session e)
        {
            if (StopSessionHandler != null)
                StopSessionHandler(sender, e);
        }

        private void Consumer_StartSessionHandler(object sender, Model.Session e)
        {
            if (StartSessionHandler != null)
                StartSessionHandler(sender, e);
        }

        private void Consumer_AcknowledgmentMessageHandler(object sender, Model.Message e)
        {
            if (AcknowledgmentMessageHandler != null)
                AcknowledgmentMessageHandler(sender, e);
        }

        private void Consumer_CommandMessageHandler(object sender, Model.Message e)
        {
            if (CommandMessageHandler != null)
                CommandMessageHandler(sender, e);
        }

        private void Consumer_LspMessageHandler(object sender, Model.Message.LspMessage e)
        {
            if (LspMessageHandler != null)
                LspMessageHandler(sender, e);
        }
    }
}
