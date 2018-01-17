using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool Start()
        {            
            if (DataConnection.OpenConnection(PipeName))
            {
                return true;
            }
            else
            {//Failed to open the data connection.
                return false;
            }
        }
    }
}
