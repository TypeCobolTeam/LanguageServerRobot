using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.Robot.Common.Connection;

namespace LanguageServer.Robot.Common.Controller
{
    /// <summary>
    /// Robot Monitoring Mode Consumer Controller.
    /// </summary>
    public class MonitoringProducerController : AbstractModeController
    {
        /// <summary>
        /// Monitoring Data Connection
        /// </summary>
        public IDataConnection DataConnection
        {
            get;
            private set;
        }
        /// <summary>
        /// The Monitor Process
        /// </summary>
        public System.Diagnostics.Process MonitorProcess
        {
            get;
            internal set;
        }

        /// <summary>
        /// Data Connection controller
        /// </summary>
        /// <param name="connection">Data Connection object.</param>
        /// <param name="scriptRepositoryPath"></param>
        public MonitoringProducerController(IDataConnection connection, string scriptRepositoryPath) : base(scriptRepositoryPath)
        {
            DataConnection = connection;
        }
        public override void FromClient(string message)
        {
            if (MonitorProcess != null && DataConnection != null)
            {
                LanguageServer.Robot.Common.Model.Message.LspMessage lsp_message =
                    new LanguageServer.Robot.Common.Model.Message.LspMessage(LanguageServer.Robot.Common.Model.Message.LspMessage.MessageFrom.Client, message);
                lock (DataConnection)
                {
                    DataConnection.WriteData(lsp_message);
                }
            }            
        }

        public override void FromServer(string message)
        {
            if (MonitorProcess != null && DataConnection != null)
            {
                LanguageServer.Robot.Common.Model.Message.LspMessage lsp_message =
                    new LanguageServer.Robot.Common.Model.Message.LspMessage(LanguageServer.Robot.Common.Model.Message.LspMessage.MessageFrom.Server, message);
                lock (DataConnection)
                {
                    DataConnection.WriteData(lsp_message);
                }
            }
        }
    }
}
