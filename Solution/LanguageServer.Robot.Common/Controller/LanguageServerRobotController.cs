using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;
using LanguageServer.Robot.Common.Connection;
using LanguageServer.Robot.Common.Model;
using LanguageServer.Robot.Common.Utilities;
using Newtonsoft.Json.Linq;

namespace LanguageServer.Robot.Common.Controller
{
    /// <summary>
    /// This class represents the Controller of the LanguageServerRobot business rules.
    /// </summary>
    public class LanguageServerRobotController : IRobotModeController, IConnectionLog, IDisposable
    {
        /// <summary>
        /// Language Server Robot Controller Connection mode
        /// </summary>
        public enum ConnectionMode
        {
            Client,
            ClientServer,
            Monitor,
            //Scenario recording mode
            Scenario,
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
        /// The Connection with the Monitor
        /// </summary>
        public IDataConnection MonitorConnection
        {
            get;
            private set;
        }

        /// <summary>
        /// Server Option sif any.
        /// </summary>
        public string ServerOptions
        {
            get; set;
        }

        /// <summary>
        /// Timeoout to wait for the Monitor Application to establish the connection : 30s == 10000ms
        /// </summary>
        private const int MONITOR_CONNECTION_TIMEOUT = 30 * 1000;
        /// <summary>
        /// The LSR Executable name.
        /// </summary>LSR_MONITOR_EXE
        private static readonly string LSR_MONITOR_EXE = "LanguageServer.Robot.Monitor.exe";

        /// <summary>
        /// Constructor for the LanguageServerRobot running as Client for the Script Test replay mode.
        /// </summary>
        /// <param name="script">The script to be replayed</param>
        /// <param name="serverConnection">The target server</param>
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        public LanguageServerRobotController(string script_path, Script script, ServerRobotConnectionController serverConnection, string scriptRepositoryPath = null)
        {
            System.Diagnostics.Contracts.Contract.Assert(serverConnection != null);
            this.ClientConnection = new ScriptRobotConnectionController(script);
            this.ServerConnection = serverConnection;
            Mode = ConnectionMode.Client;
            //Transfert the roboting mode controller instance to the client and server controller
            RobotModeController = new ReplayModeController(script, script_path, scriptRepositoryPath);            
            this.ClientConnection.RobotModeController = RobotModeController;
            this.ServerConnection.RobotModeController = RobotModeController;
        }

        /// <summary>
        /// Constructor for the LanguageServerRobot running as Client for the Session Test replay mode.
        /// </summary>
        /// <param name="session">The session to be replayed</param>
        /// <param name="serverConnection">The target server</param>
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        public LanguageServerRobotController(string session_path, Session session, ServerRobotConnectionController serverConnection, string scriptRepositoryPath = null)
        {
            System.Diagnostics.Contracts.Contract.Assert(serverConnection != null);
            this.ClientConnection = new SessionRobotConnectionController(session);
            this.ServerConnection = serverConnection;
            Mode = ConnectionMode.Client;
            //Transfert the roboting mode controller instance to the client and server controller
            //This is a dummy replay mode controller, because this shall be done for each script in the session
            RobotModeController = new ReplayModeController(session_path, scriptRepositoryPath);
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
        /// Constructor for the LanguageServerRobot running as Client/Server in monitoring mode.
        /// </summary>
        /// <param name="clientConnection">The source client</param>
        /// <param name="serverConnection">The target server</param>
        /// <param name="connection">The Data Connection for used for sending monitoring messages</param>
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        public LanguageServerRobotController(ClientRobotConnectionController clientConnection, ServerRobotConnectionController serverConnection, IDataConnection connection, string scriptRepositoryPath = null)
        {
            System.Diagnostics.Contracts.Contract.Assert(clientConnection != null);
            System.Diagnostics.Contracts.Contract.Assert(serverConnection != null);
            System.Diagnostics.Contracts.Contract.Assert(connection != null);
            this.ClientConnection = clientConnection;
            this.ServerConnection = serverConnection;
            this.MonitorConnection = connection;
            Mode = ConnectionMode.Monitor;
            //Transfert the roboting mode controller instance to the client and server controller
            RobotModeController = new MonitoringProducerController(connection, scriptRepositoryPath);
            clientConnection.RobotModeController = RobotModeController;
            serverConnection.RobotModeController = RobotModeController;
        }

        /// <summary>
        /// Constructor for the LanguageServerRobot running as ClientScenarion/Server recording mode.
        /// </summary>
        /// <param name="serverConnection">The target server</param>
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        public LanguageServerRobotController(ServerRobotConnectionController serverConnection, string scriptRepositoryPath = null)
        {
            System.Diagnostics.Contracts.Contract.Assert(serverConnection != null);
            this.ClientConnection = new ScenarioRobotConnectionController(serverConnection.MessageConnection);
            this.ServerConnection = serverConnection;
            Mode = ConnectionMode.Scenario;
            //Transfert the roboting mode controller instance to the client and server controller
            RobotModeController = new RecordingModeController(scriptRepositoryPath);
            ClientConnection.RobotModeController = RobotModeController;
            ServerConnection.RobotModeController = RobotModeController;
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
        /// Task for waiting the monitor's termination
        /// </summary>
        public TaskCompletionSource<bool> MonitorTaskCompletionSource
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

        //The Monitor Task
        public Task<bool> MonitorTask
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
            //JObject jsonMessage = null;
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
            //JObject jsonMessage = null;
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
        /// Starts the monitor
        /// </summary>
        private bool StartMonitor()
        {
            //Create a nammed pipe
            string pipeName = Util.CreatePipeName();
            MonitorTaskCompletionSource = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> monitorTaskStart = new TaskCompletionSource<bool>();
            MonitorTask = new Task<bool>(
                () =>
                {
                    bool bResult = false;
                    try
                    {
                        monitorTaskStart.SetResult(true);
                        //Connect to it, it will wait for Monitoring connection
                        bResult = this.MonitorConnection.OpenConnection(pipeName);
                    }
                    catch (Exception e)
                    {
                        LogWriter?.WriteLine(e.Message);
                    }
                    MonitorTaskCompletionSource.SetResult(bResult);
                    return bResult;
                }
                );
            MonitorTask.Start();
            if (monitorTaskStart.Task.Result)
            {
                //Run the Monitor Application.
                string location = Assembly.GetExecutingAssembly().Location;
                Uri uri = new Uri(location);
                string local_location = uri.LocalPath;
                FileInfo fi = new FileInfo(local_location);
                string local_dir = fi.DirectoryName;
                string monitor_path = Path.Combine(local_dir, LSR_MONITOR_EXE);
                string monitor_argument = string.Format("-pipe \"{0}\"", pipeName);
                if (ServerOptions != null)
                {
                    monitor_argument += " -so \"" + ServerOptions + "\"";
                }
                this.MonitorProcess = new System.Diagnostics.Process();
                this.MonitorProcess.StartInfo.FileName = monitor_path;
                this.MonitorProcess.StartInfo.Arguments = monitor_argument;
                try
                {
                    if (this.MonitorProcess.Start())
                    {
                        //Communicate the Monitoring Process to the Monitoring Mode Controller.
                        (this.RobotModeController as MonitoringProducerController).MonitorProcess = this.MonitorProcess;
                        return true;
                    }
                    else
                    {
                        this.MonitorConnection.CloseConnection();
                        return false;
                    }
                }
                catch (Exception e)
                {
                    LogWriter?.WriteLine(e.Message);
                    try
                    {
                        this.MonitorConnection.CloseConnection();
                    }
                    catch (System.InvalidOperationException ioe)
                    {
                        //     No pipe connections have been made yet.-or-The connected pipe has already disconnected.-or-The
                        //     pipe handle has not been set.
                        LogWriter?.WriteLine(ioe.Message);
                    }
                    return false;
                }
            }
            return false;
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
            {
                if (this.ClientConnection.State == ConnectionState.Closed || this.ClientConnection.State == ConnectionState.Disposed)
                {
                    //Force to kill the server process
                    ServerConnection.Stop();
                }
                if (ClientTaskConnectionState.Task.Status == TaskStatus.Running)
                {
                    ClientTaskConnectionState.SetResult(this.ClientConnection.State);
                }
            }
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
        /// Start Replaying a script
        /// <returns>true if the server has been launched, false otherwise.</returns>
        /// </summary>
        protected bool StartReplayingScript()
        {
            StartServer();
            //Ensure that the server has succcesfuly strated
            var serverConnectionSate = ServerTaskConnectionState.Task.Result; ;
            if (!(serverConnectionSate == ConnectionState.Closed || serverConnectionSate == ConnectionState.Disposed))
            {
                StartClient();
                Task<bool> task = WaitClientServerTermination();
                return task.Result;
            }
            return false;
        }

        /// <summary>
        /// Start Replaying a script
        /// <returns>true if the server has been launched, false otherwise.</returns>
        /// </summary>
        protected bool StartReplayingSession()
        {
            StartServer();
            //Ensure that the server has succcesfuly strated
            var serverConnectionSate = ServerTaskConnectionState.Task.Result; ;
            //if (!(serverConnectionSate == ConnectionState.Closed || serverConnectionSate == ConnectionState.Disposed))
            {
                StartClient();
                Task<bool> task = WaitClientServerTermination();
                return task.Result;
            }
            //return false;
        }

        /// <summary>
        /// Start Replaying
        /// <returns>true if the server has been launched, false otherwise.</returns>
        /// </summary>
        protected bool StartReplaying()
        {
            if (ClientConnection is ScriptRobotConnectionController)
                return StartReplayingScript();
            else
                return StartReplayingSession();
        }

        /// <summary>
        /// Start Scenarion
        /// <returns>true if the server has been launched, false otherwise.</returns>
        /// </summary>
        protected bool StartScenario()
        {
            StartServer();
            //Ensure that the server has succcesfuly strated
            var serverConnectionSate = ServerTaskConnectionState.Task.Result; ;
            if (!(serverConnectionSate == ConnectionState.Closed || serverConnectionSate == ConnectionState.Disposed))
            {
                //We only start a Server not a client we assume that caller is the client Thread.
                //StartClient();
                //Task<bool> task = WaitClientServerTermination();
                //return task.Result;
                return true;//The Server has been started
            }
            return false;
        }

        /// <summary>
        /// Start the monitor
        /// </summary>
        /// <returns>true if monitor has been started, false otherwise</returns>
        protected bool StartMonitoring()
        {
            StartMonitor();
            //Wait for the monitor starting.
            //MonitorTask.Wait(MONITOR_CONNECTION_TIMEOUT);
            MonitorTask.Wait();
            if (MonitorTaskCompletionSource.Task.Status == TaskStatus.Running || MonitorTaskCompletionSource.Task.Status == TaskStatus.WaitingForActivation)
            {//Connection timeout
                if (this.MonitorProcess != null)
                {
                    try
                    {
                        this.MonitorProcess.Kill();
                    }                    
                    catch(Exception e)
                    {

                    }
                }
                LogWriter?.WriteLine(Resource.MonitorConnectionTimeout);
                return false;
            }
            else
            {
                bool bResult = MonitorTaskCompletionSource.Task.Result;
                if (bResult)
                {
                    bResult = StartRoboting();
                }
                return bResult;
            }
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
                case ConnectionMode.Monitor:
                    return StartMonitoring();
                case ConnectionMode.Scenario:
                    return StartScenario();
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

        /// <summary>
        /// General Log TextWriter
        /// </summary>
        public TextWriter LogWriter { get; set; }

        /// <summary>
        /// Protocol Log TextWriter
        /// </summary>
        public TextWriter ProtocolLogWriter { get; set; }

        /// <summary>
        /// Message Log TextWriter
        /// </summary>
        public TextWriter MessageLogWriter { get; set; }

        public virtual void FromClient(string message)
        {
            ClientConnection.FromClient(message);
            ServerConnection.SendMessage(message);
        }

        public virtual void FromServer(string message)
        {
            if (ClientConnection is SessionRobotConnectionController)
            {   //For a session all message from the server must be redirected to
                //The ReplayModeController instance of teh script being replayed.
                ClientConnection.FromServer(message);
                ClientConnection.SendMessage(message);
            }
            else
            {
                ServerConnection.FromServer(message);
                ClientConnection.SendMessage(message);
            }
        }

        /// <summary>
        /// Plays a script
        /// </summary>
        /// <param name="script_path">The path of the script to replay</param>
        /// <param name="script">The script model to replay</param>
        /// <param name="serverPath">The Server's path</param>
        /// <param name="serverOptions">The Server's options</param>
        /// <param name="scriptRepositoryPath">The script repository path</param>
        /// <returns>0 if no error -1 otherwise.</returns>
        public static int ReplayScript(string script_path, Script script, string serverPath, string serverOptions, string scriptRepositoryPath)
        {
            var server = new ServerRobotConnectionController(new ProcessMessageConnection(serverPath));
            var robot = new LanguageServerRobotController(script_path, script, server, scriptRepositoryPath);
            robot.ServerOptions = serverOptions;
            robot.PropagateConnectionLogs();
            if (!robot.Start())
            {
                return -1;
            }
            else
            {
                bool bResult = robot.WaitExit();
                return bResult ? 0 : -1;
            }
        }

        /// <summary>
        /// Plays a session
        /// </summary>
        /// <param name="session_path">The path of the session to replay</param>
        /// <param name="session">The session model to replay</param>
        /// <param name="serverPath">The Server's path</param>
        /// <param name="serverOptions">The Server's options</param>
        /// <param name="scriptRepositoryPath">The script repository path</param>
        /// <returns>0 if no error -1 otherwise.</returns>
        public static int ReplaySession(string session_path, Session session, string serverPath, string serverOptions, string scriptRepositoryPath)
        {
            bool bResult = true;
            foreach (string scriptPath in session.scripts)
            {
                Script script;
                Exception exc;
                bool bValid = Util.ReadScriptFile(scriptPath, out script, out exc);
                if (bValid)
                {
                    if (ReplayScript(scriptPath, script, serverPath, serverOptions, scriptRepositoryPath) != 0)
                    {
                        bResult = false;
                    }
                }
                else
                {
                    bResult = false;
                }
            }
            return bResult ? 0 : -1;
        }

        /// <summary>
        /// Dispose the controller
        /// </summary>
        public void Dispose()
        {            
            ServerConnection?.Dispose();
            ClientConnection?.Dispose();
        }
    }
}
