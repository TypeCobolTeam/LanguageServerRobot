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
                Consumer.LspMessageHandler += LspMessageHandler; 
                Consumer.CommandMessageHandler += CommandMessageHandler;
                Consumer.AcknowledgmentMessageHandler += AcknowledgmentMessageHandler;
                Consumer.StartSessionHandler += StartSessionHandler;
                Consumer.StopSessionHandler += StopSessionHandler;
                Consumer.StartDocumentHandler += StartDocumentHandler;
                Consumer.StopDocumentHandler += StopDocumentHandler;
            }
        }
    }
}
