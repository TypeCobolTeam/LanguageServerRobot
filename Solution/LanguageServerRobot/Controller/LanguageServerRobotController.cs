using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// This class represents the Controller of the LanguageServerRobot business rules.
    /// </summary>
    public class LanguageServerRobotController
    {
        /// <summary>
        /// Language Server Robot Controller Connection mode
        /// </summary>
        public enum ConnectionMode
        {
            Client,
            ClientServer,
        }

        /// <summary>
        /// The Connection mode
        /// </summary>
        public ConnectionMode Mode
        {
            get;
            private set;
        }
        /// <summary>
        /// The Connection with the Client if the Connection mode is ClientServer, null otherwise.
        /// </summary>
        public ClientRobotConnectionController ClientConnection
        {
            get;
            private set;
        }

        /// <summary>
        /// The Connection with the Server
        /// </summary>
        public ServerRobotConnectionController ServerConnection
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor for the LanguageServerRobot running as Client for the Test replay mode.
        /// </summary>
        /// <param name="serverConnection">The target server</param>
        public LanguageServerRobotController(ServerRobotConnectionController serverConnection)
        {
            System.Diagnostics.Contracts.Contract.Assert(serverConnection != null);
            this.ServerConnection = serverConnection;
            Mode = ConnectionMode.Client;
        }

        /// <summary>
        /// Constructor for the LanguageServerRobot running as Client/Server for the test recording mode.
        /// </summary>
        /// <param name="clientConnection">The source client</param>
        /// <param name="serverConnection">The target server</param>
        public LanguageServerRobotController(ClientRobotConnectionController clientConnection, ServerRobotConnectionController serverConnection)
        {
            System.Diagnostics.Contracts.Contract.Assert(clientConnection != null);
            System.Diagnostics.Contracts.Contract.Assert(serverConnection != null);
            this.ClientConnection = clientConnection;
            this.ServerConnection = serverConnection;
            Mode = ConnectionMode.ClientServer;
        }

        /// <summary>
        /// Propagate Connection Log settings to this.
        /// </summary>
        /// <param name="log">The Connection Logs setting.</param>
        public void PropagateConnectionLogs(ConnectionLog log = null)
        {
            log = log ?? ConnectionLog.GetInstance();
            ClientConnection?.PropagateConnectionLogs(log);
            ServerConnection?.PropagateConnectionLogs(log);
        }

        /// <summary>
        /// Start Roboting :-)
        /// </summary>
        protected async void StartRoboting()
        {
            if (this.ClientConnection != null)
                this.ClientConnection.Start();
            if (this.ServerConnection != null)
                await this.ServerConnection.Start();
        }

        /// <summary>
        /// Start Replaying
        /// </summary>
        protected async void StartReplaying()
        {
            if (this.ServerConnection != null)
                await this.ServerConnection.Start();
        }

        /// <summary>
        /// Start controlling according the mode.
        /// </summary>
        public void Start()
        {
            switch(Mode)
            {
                case ConnectionMode.Client:
                    StartReplaying();
                    break;
                case ConnectionMode.ClientServer:
                    StartRoboting();
                    break;
            }
        }
    }
}
