using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;
using LanguageServerRobot.Model;
using LanguageServerRobot.Utilities;
using Newtonsoft.Json.Linq;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// This class represents the Controller of the LanguageServerRobot business rules.
    /// </summary>
    public class LanguageServerRobotController : IRobotModeController
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
        /// Constructor for the LanguageServerRobot running as Client for the Script Test replay mode.
        /// </summary>
        /// <param name="script">The script to be replayed</param>
        /// <param name="serverConnection">The target server</param>
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        public LanguageServerRobotController(Script script, ServerRobotConnectionController serverConnection, string scriptRepositoryPath = null)
        {
            System.Diagnostics.Contracts.Contract.Assert(serverConnection != null);
            this.ClientConnection = new ScriptRobotConnectionController(script);
            this.ServerConnection = serverConnection;
            Mode = ConnectionMode.Client;
            //Transfert the roboting mode controller instance to the client and server controller
            RobotModeController = new ReplayModeController(script, scriptRepositoryPath);
            this.ClientConnection.RobotModeController = RobotModeController;
            this.ServerConnection.RobotModeController = RobotModeController;
        }

        /// <summary>
        /// Constructor for the LanguageServerRobot running as Client for the Session Test replay mode.
        /// </summary>
        /// <param name="session">The session to be replayed</param>
        /// <param name="serverConnection">The target server</param>
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        public LanguageServerRobotController(Session session, ServerRobotConnectionController serverConnection, string scriptRepositoryPath = null)
        {
            System.Diagnostics.Contracts.Contract.Assert(serverConnection != null);            
            this.ClientConnection = new SessionRobotConnectionController(session);
            this.ServerConnection = serverConnection;
            Mode = ConnectionMode.Client;
            //Transfert the roboting mode controller instance to the client and server controller
            //TODO ==> This shall be done for each script in the session
            //RobotModeController = new ReplayModeController(scriptRepositoryPath);
            this.ClientConnection.RobotModeController = RobotModeController;
            this.ServerConnection.RobotModeController = RobotModeController;
        }

        /// <summary>
        /// Constructor for the LanguageServerRobot running as Client/Server for the test recording mode.
        /// </summary>
        /// <param name="clientConnection">The source client</param>
        /// <param name="serverConnection">The target server</param>
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        public LanguageServerRobotController(ClientRobotConnectionController clientConnection, ServerRobotConnectionController serverConnection, string scriptRepositoryPath = null)
        {
            System.Diagnostics.Contracts.Contract.Assert(clientConnection != null);
            System.Diagnostics.Contracts.Contract.Assert(serverConnection != null);
            this.ClientConnection = clientConnection;
            this.ServerConnection = serverConnection;
            Mode = ConnectionMode.ClientServer;            
            //Transfert the roboting mode controller instance to the client and server controller
            RobotModeController = new RecordingModeController(scriptRepositoryPath);
            clientConnection.RobotModeController = RobotModeController;
            serverConnection.RobotModeController = RobotModeController;
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
            if (RobotModeController is IConnectionLog)
                log.AssignTo(RobotModeController as IConnectionLog);            
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
        /// Method to filter messaged produced for the Client
        /// </summary>
        /// <param name="message">The mmessage to filter</param>
        /// <param name="connection">The source connection of the message</param>
        /// <returns>true if the message is filtered, false otherwise.</returns>
        private bool ClientProducedMessageFilter(string message, IMessageConnection connection)
        {
            JObject jsonMessage = null;
            //If we have a server connection directly forward the message to it
            if (ServerConnection != null && ServerConnection.State == ConnectionState.Listening)
            {
                FromClient(message);                
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Method to filter messaged produced for the Server
        /// </summary>
        /// <param name="message">The mmessage to filter</param>
        /// <param name="connection">The source connection of the message</param>
        /// <returns>true if the message is filtered, false otherwise.</returns>
        private bool ServerProducedMessageFilter(string message, IMessageConnection connection)
        {
            JObject jsonMessage = null;
            //If we have a client connection directly forward the message to it
            if (ClientConnection != null && ClientConnection.State == ConnectionState.Listening)
            {
                FromServer(message);                
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Starts the client
        /// </summary>
        private void StartClient()
        {
            if (this.ClientConnection != null)
            {
                //Set Message filter to us
                this.ClientConnection.ProducedMessageFilter = ClientProducedMessageFilter;
                ClientTaskConnectionState = new TaskCompletionSource<ConnectionState>();
                this.ClientConnection.AddStageChangedEventHandler(ClientStateChanged);
            }
            ClientTaskCompletionSource = new TaskCompletionSource<bool>();
            ClientTask = new Task<bool>(
                () =>
                {
                    bool bResult = false;
                    Task <bool> task = null;
                    try
                    {
                        task = this.ClientConnection != null ? this.ClientConnection.Start() : null;
                        bResult = task != null ? task.Result : false;
                    }                    
                    catch(Exception e)
                    {
                        this.ClientConnection.LogWriter?.WriteLine(e.Message);
                        switch (this.ClientConnection.State)
                        {
                            case ConnectionState.Closed:
                                bResult = true;
                                break;
                            default:
                                bResult = false;
                                break;
                        }                        
                    }
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
                //Set Message filter to us
                this.ServerConnection.ProducedMessageFilter = ServerProducedMessageFilter;
                ServerTaskConnectionState = new TaskCompletionSource<ConnectionState>();
                this.ServerConnection.AddStageChangedEventHandler(ServerStateChanged);
            }
            ServerTaskCompletionSource = new TaskCompletionSource<bool>();
            ServerTask = new Task<bool>(
                () =>
                {
                    bool bResult = false;
                    Task<bool> task = null;
                    try
                    {
                        task = this.ServerConnection != null ? this.ServerConnection.Start() : null;
                        bResult = task != null ? task.Result : false;
                    }
                    catch (Exception e)
                    {
                        this.ServerConnection.LogWriter?.WriteLine(e.Message);
                        switch (this.ServerConnection.State)
                        {
                            case ConnectionState.Closed:
                                bResult = true;
                                break;
                            default:
                                bResult = false;
                                break;
                        }
                    }
                    ServerTaskCompletionSource.SetResult(bResult);
                    return bResult;
                }
                );
            ServerTask.Start();
        }

        /// <summary>
        /// Wait that the client Terminate
        /// </summary>
        /// <returns></returns>
        private async Task<bool> WaitClientTermination()
        {
            //Wait till the client and the server are listening.
            //ConnectionState clientState = ConnectionState.Disposed;
            //if (ClientTaskConnectionState != null)
            //{
            //    clientState = ClientTaskConnectionState.Task.Result;
            //}
            //return clientState == ConnectionState.Listening;
            await ClientTask;
            return ClientTask.Result;
        }

        /// <summary>
        /// Wait that the Server Terminate
        /// </summary>
        /// <returns></returns>
        private async Task<bool> WaitServerTermination()
        {
            //Wait till the server are listening.
            //ConnectionState serverState = ConnectionState.Disposed;
            //if (ServerTaskConnectionState != null)
            //{
            //    serverState = ServerTaskConnectionState.Task.Result;
            //}
            //return serverState == ConnectionState.Listening;
            await ServerTask;
            return ServerTask.Result;
        }

        /// <summary>
        /// Wait that the Client and the Server Terminate
        /// </summary>
        /// <returns></returns>
        private async Task<bool> WaitClientServerTermination()
        {
            bool bResultClient = await WaitClientTermination();
            bool bResultServer = await WaitServerTermination();
            return bResultClient && bResultServer;
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
        /// Start Roboting :-)
        /// <returns>true if both Client and Server have been launched, false otherwise.</returns>
        /// </summary>
        protected bool StartRoboting()
        {
            StartClient();
            StartServer();
            Task<bool> task = WaitClientServerTermination();
            return task.Result;
        }

        /// <summary>
        /// Start Replaying
        /// <returns>true if the server has been launched, false otherwise.</returns>
        /// </summary>
        protected bool StartReplaying()
        {
            StartServer();
            //Ensure that the server has succcesfuly strated
            var serverConnectionSate = ServerTaskConnectionState.Task.Result; ;
            {
                StartClient();
                Task<bool> task = WaitClientServerTermination();
                return task.Result;
            }
            return false;
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

        /// <summary>
        /// Forces the calling thread to wait for Client/Server controller exit.
        /// </summary>
        public bool WaitExit()
        {
            switch (Mode)
            {
                case ConnectionMode.Client:
                    if (ClientTaskCompletionSource != null)
                    {
                        bool bResult = ClientTaskCompletionSource.Task.Result;
                        return bResult;
                    }
                    break;
                case ConnectionMode.ClientServer:
                    if (ClientTaskCompletionSource != null && ServerTaskCompletionSource != null)
                    {
                        bool bResult = ClientTaskCompletionSource.Task.Result && ServerTaskCompletionSource.Task.Result;
                        return bResult;
                    }
                    break;
            }
            return true;
        }

        /// <summary>
        /// The Robot Mode Controller for the Business Logic.
        /// </summary>
        public IRobotModeController RobotModeController
        {
            get;
            internal set;
        }

        public bool IsModeInitialized
        {
            get
            {
                return RobotModeController.IsModeInitialized;
            }
        }

        public bool IsModeStarted
        {
            get
            {
                return RobotModeController.IsModeStarted;
            }
        }

        public bool IsModeStopped
        {
            get
            {
                return RobotModeController.IsModeStopped;
            }
        }

        public void FromClient(string message)
        {
            ClientConnection.FromClient(message);
            ServerConnection.SendMessage(message);
        }

        public void FromServer(string message)
        {
            ServerConnection.FromServer(message);
            ClientConnection.SendMessage(message);
        }
    }
}
