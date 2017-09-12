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
        /// Task for waiting Client's the connection state change event at start time.
        /// </summary>
        public TaskCompletionSource<ConnectionState> ClientTaskConnectionState
        {
            get;
            private set;
        }

        /// <summary>
        /// Task for waiting Server's the connection state change event at start time.
        /// </summary>
        public TaskCompletionSource<ConnectionState> ServerTaskConnectionState
        {
            get;
            private set;
        }

        /// <summary>
        /// Task for waiting the Client's termination
        /// </summary>
        public TaskCompletionSource<bool> ClientTaskCompletionSource
        {
            get;
            private set;
        }

        /// <summary>
        /// Task for waiting the Server's termination
        /// </summary>
        public TaskCompletionSource<bool> ServerTaskCompletionSource
        {
            get;
            private set;
        }

        /// <summary>
        /// The Client Task
        /// </summary>
        public Task<bool> ClientTask
        {
            get;
            private set;
        }

        //The Sever Task
        public Task<bool> ServerTask
        {
            get;
            private set;
        }


        /// <summary>
        /// Starts the client
        /// </summary>
        private void StartClient()
        {
            if (this.ClientConnection != null)
            {
                ClientTaskConnectionState = new TaskCompletionSource<ConnectionState>();
                this.ClientConnection.AddStageChangedEventHandler(ClientStateChanged);
            }
            ClientTaskCompletionSource = new TaskCompletionSource<bool>();
            ClientTask = new Task<bool>(
                () =>
                {
                    Task<bool> task = this.ClientConnection != null ? this.ClientConnection.Start() : null;
                    bool bResult = task != null ? task.Result : false;
                    ClientTaskCompletionSource.SetResult(bResult);
                    return bResult;
                }
                );
            ClientTask.Start();
        }

        /// <summary>
        /// Starts the server
        /// </summary>
        private void StartServer()
        {
            if (this.ServerConnection != null)
            {
                ServerTaskConnectionState = new TaskCompletionSource<ConnectionState>();
                this.ServerConnection.AddStageChangedEventHandler(ServerStateChanged);
            }
            ServerTaskCompletionSource = new TaskCompletionSource<bool>();
            ServerTask = new Task<bool>(
                () =>
                {
                    Task<bool> task = this.ServerConnection != null ? this.ServerConnection.Start() : null;
                    bool bResult = task != null ? task.Result : false;
                    ServerTaskCompletionSource.SetResult(bResult);
                    return bResult;
                }
                );
            ServerTask.Start();
        }

        /// <summary>
        /// Start Roboting :-)
        /// <returns>true if both Client and Server have been launched, false otherwise.</returns>
        /// </summary>
        protected bool StartRoboting()
        {
            StartClient();
            StartServer();            
            //Wait till the client and the server are listening.
            ConnectionState clientState = ConnectionState.Disposed;
            if (ClientTaskConnectionState != null)
            {
                clientState = ClientTaskConnectionState.Task.Result;
            }
            ConnectionState serverState = ConnectionState.Disposed;
            if (ClientTaskConnectionState != null)
            {
                serverState = ClientTaskConnectionState.Task.Result;
            }
            return clientState == ConnectionState.Listening && serverState == ConnectionState.Listening;
        }

        private void ServerStateChanged(object sender, EventArgs e)
        {
            if (ServerTaskConnectionState != null)
                ServerTaskConnectionState.SetResult(this.ServerConnection.State);
        }

        private void ClientStateChanged(object sender, EventArgs e)
        {
            if (ClientTaskConnectionState != null)
                ClientTaskConnectionState.SetResult(this.ClientConnection.State);
        }

        /// <summary>
        /// Start Replaying
        /// <returns>true if the server has been launched, false otherwise.</returns>
        /// </summary>
        protected bool StartReplaying()
        {
            ServerTask.Start();
            //Wait till the server is listening.
            ConnectionState serverState = ConnectionState.Disposed;
            if (ClientTaskConnectionState != null)
            {
                serverState = ClientTaskConnectionState.Task.Result;
            }
            return serverState == ConnectionState.Listening;
        }

        /// <summary>
        /// Start controlling according the mode.
        /// </summary>
        public bool Start()
        {
            switch(Mode)
            {
                case ConnectionMode.Client:
                    return StartReplaying();
                case ConnectionMode.ClientServer:
                    return StartRoboting();
            }
            return false;
        }
    }
}
