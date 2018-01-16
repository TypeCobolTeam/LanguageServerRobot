using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.Robot.Common.Connection;

namespace LanguageServer.Robot.Common.Controller
{
    /// <summary>
    /// Robot Monitoring Mode Controller.
    /// </summary>
    public class MonitoringModeController : AbstractModeController
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
        /// Data Connection controller
        /// </summary>
        public MonitoringModeController(IDataConnection connection, string scriptRepositoryPath) : base(scriptRepositoryPath)
        {
            DataConnection = connection;
        }
        public override void FromClient(string message)
        {
            throw new NotImplementedException();
        }

        public override void FromServer(string message)
        {
            throw new NotImplementedException();
        }
    }
}
