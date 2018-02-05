using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LanguageServer.JsonRPC;
using LanguageServer.Robot.Common.Utilities;
using Mono.Options;
using LanguageServer.Robot.Monitor.Properties;
using LanguageServer.Robot.Common.Controller;
using System.Windows.Input;
using LanguageServer.Robot.Monitor.Model;
using LanguageServer.Robot.Common.Model;
using LanguageServer.Robot.Monitor.View;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace LanguageServer.Robot.Monitor.Controller
{
    /// <summary>
    /// The Main Controller of the Language Server Robot Monitor.
    /// </summary>
    public class LanguageServerRobotMonitor : ICommand
    {
        /// <summary>
        /// Program name from Assembly name
        /// </summary>
        public static string ProgName
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Name;
            }
        }
        /// <summary>
        /// Assembly version
        /// </summary>
        public static string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        /// <summary>
        /// The Logging Level
        /// </summary>
        public static ConnectionLogLevel LogLevel
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Log File.
        /// </summary>
        public static string LogFile
        {
            get;
            internal set;
        }

        /// <summary>
        /// Communication Pipe's name.
        /// </summary>
        public static string PipeName
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Scripts Repository path.
        /// </summary>
        public static string ScriptRepositoryPath
        {
            get;
            internal set;
        }

        public static ConnectionLog Log
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Monitoring connection instance.
        /// </summary>
        public MonitoringConnectionController MonitoringConnection
        {
            get;
            set;
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
        /// The monitoring connection thread
        /// </summary>
        public Task<MonitoringConsumerController.ConnectionState> MonitoringConnectionTask
        {
            get;
            private set;
        }
        /// <summary>
        /// The monitoring connection thread completion state.
        /// </summary>
        public TaskCompletionSource<MonitoringConsumerController.ConnectionState> MonitoringConnectionTaskCompletionSource
        {
            get;
            private set;
        }

        /// <summary>
        /// The session explorer.
        /// </summary>
        public SessionExplorerController SessionExplorer
        {
            get;
            private set;
        }

        /// <summary>
        /// Queue of pending session before MainWidow view binding.
        /// </summary>
        private Queue<Common.Model.Session> PendingSession
        {
            get;
            set;
        }

        /// <summary>
        /// The Main Window as View
        /// </summary>
        private LanguageServer.Robot.Monitor.MainWindow View
        {
            get { return App.Current.MainWindow as LanguageServer.Robot.Monitor.MainWindow; }
            
        }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static LanguageServerRobotMonitor()
        {
            Log = new ConnectionLog();
            Log.LogWriter = new DebugTextWriter();
            Log.MessageLogWriter = Log.LogWriter;
            Log.ProtocolLogWriter = Log.LogWriter;
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Starts the client
        /// </summary>
        private void StartMonitoringConnection()
        {
            PendingSession = new Queue<Common.Model.Session>();
            this.ConnectMonitoringController(true);
            MonitoringConnectionTaskCompletionSource = new TaskCompletionSource<MonitoringConsumerController.ConnectionState>();
            MonitorTaskCompletionSource = new TaskCompletionSource<bool>();
            MonitoringConnectionTask = new Task<MonitoringConsumerController.ConnectionState>(
                () =>
                {
                    MonitoringConsumerController.ConnectionState result = MonitoringConsumerController.ConnectionState.ConnectionFailed;
                    Task<MonitoringConsumerController.ConnectionState> task = null;
                    try
                    {
                        MonitorTaskCompletionSource.SetResult(true);
                        task = this.MonitoringConnection != null ? this.MonitoringConnection.Start() : null;
                        result = task != null ? task.Result : MonitoringConsumerController.ConnectionState.ConnectionFailed;
                    }
                    catch (Exception e)
                    {
                        Log.LogWriter.WriteLine(e.Message);
                    }
                    MonitoringConnectionTaskCompletionSource.SetResult(result);
                    return result;
                }
                );
            MonitoringConnectionTask.Start();
        }

        const string DefaultTypeCobolLanguageServerPath = "C:\\TypeCobol\\Sources\\##Latest_Release##\\TypeCobol.LanguageServer.exe";

        /// <summary>
        /// The Server path.
        /// </summary>
        public string ServerPath
        {
            get;
            internal set;
        }

        /// <summary>
        /// Entry point of the LSRM Controller, with access to the command line arguments.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Main(object sender, StartupEventArgs e)
        {
            bool help = false;
            bool version = false;

            var p = new OptionSet()
            {
                "USAGE",
                "  "+ProgName+" [OPTIONS]",
                "",
                "VERSION:",
                "  "+Version,
                "",
                "DESCRIPTION:",
                "  Run the Language Server Robot Monitor.",
                { "l|loglevel=",  "Logging level (1=Lifecycle, 2=Message, 3=Protocol).", (string v) =>
                    {
                        LogLevel = ConnectionLogLevel.Lifecycle;//(ConnectionLogLevel)(v!=null)
                        if (v != null)
                        {
                            try
                            {
                                // args[0] : Trace level
                                LogLevel = (ConnectionLogLevel)Int32.Parse(v);
                                if (!System.Enum.IsDefined(typeof(ConnectionLogLevel), (Int32)LogLevel))
                                {
                                    LogLevel = ConnectionLogLevel.Protocol;
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.LogWriter.WriteLine(ex.Message);
                            }
                        }
                    }
                },
                { "v|version","Show version", _ => version = true },
                { "h|help","Show help", _ => help = true },
                { "lf|logfile=","{PATH} the target log file", (string v) => LogFile = v },
                { "p|pipe=","Communication Pipe's name with LSR", (string v) => PipeName = v },
                { "d|dir=","{PATH} Scripts repository directory", (string v) => ScriptRepositoryPath = v },
                { "s|server=","{PATH} the server path", (string v) => ServerPath = v },
            };
            System.Collections.Generic.List<string> arguments;
            try { arguments = p.Parse(e.Args); }
            catch (OptionException ex)
            {
                MessageBox.Show(ex.Message, LanguageServer.Robot.Monitor.Properties.Resources.LSRMName, MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
            }
            if (help)
            {
                System.IO.StringWriter sw = new System.IO.StringWriter();
                p.WriteOptionDescriptions(sw);
                sw.Flush();
                MessageBox.Show(sw.ToString(), LanguageServer.Robot.Monitor.Properties.Resources.LSRMName, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (version)
            {
                System.IO.StringWriter sw = new System.IO.StringWriter();
                sw.WriteLine(Version);
                sw.Flush();
                MessageBox.Show(string.Format(LanguageServer.Robot.Monitor.Properties.Resources.VersionTitle, sw.ToString()), LanguageServer.Robot.Monitor.Properties.Resources.LSRMName, MessageBoxButton.OK, MessageBoxImage.Information);
            }

            if (help || version)
            {
                App.Current.Shutdown();
            }

            if (ServerPath == null)
            {
                ServerPath = DefaultTypeCobolLanguageServerPath;
            }

            if (ScriptRepositoryPath == null)
            {//Default path the user document path
                ScriptRepositoryPath = Util.DefaultScriptRepositorPath;
            }
            if (PipeName != null)
            {
                //MessageBox.Show(PipeName);
                //There is a pipe connection
                MonitoringConnection = new MonitoringConnectionController(new MonitoringConsumerController(PipeName));
                StartMonitoringConnection();
                if (MonitorTaskCompletionSource.Task.Result)
                {//Monitoring as started.
                    MonitoringConnection.Consumer.LspMessageHandler += Consumer_LspMessageHandler;
                }
                //try
                //{
                //    if (!MonitoringConnection.Start())
                //    {
                //        MessageBox.Show(string.Format(LanguageServer.Robot.Monitor.Properties.Resources.FailMessageConnectionWithLSR, PipeName));
                //        MonitoringConsumer = null;
                //    }
                //}
                //catch(Exception exc)
                //{
                //    MessageBox.Show(string.Format(LanguageServer.Robot.Monitor.Properties.Resources.FailMessageConnectionWithLSR, exc.Message + ':' + PipeName));
                //}
            }
        }

        /// <summary>
        /// Add a session
        /// </summary>
        /// <param name="session">The session to be aadded</param>
        internal void AddSession(Session session, bool bLock = true)
        {
            Action action = () =>
            {
                if (SessionExplorer != null)
                    SessionExplorer.AddSession(session);
                else
                    PendingSession.Enqueue(session);
            };
            if (bLock)
            {
                lock (PendingSession)
                {
                    action();
                }
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Add a new document
        /// </summary>
        /// <param name="script">The Document's script</param>
        internal void AddDocument(Script script)
        {
            //MessageBox.Show("COUCOU");
            lock (PendingSession)
            {
                SessionExplorer.AddDocument(this.MonitoringConnection.Consumer.SessionModel, script);
            }
        }

        /// <summary>
        /// Set the active document in the Solution explorer
        /// </summary>
        /// <param name="document">The Document's script to set as the active one</param>
        internal void SetActiveDocument(Script document)
        {
            SessionExplorer.SetActiveDocument(this.MonitoringConnection.Consumer.SessionModel, document);
        }

        /// <summary>
        /// The itemViewModel of the tree session explorer that was previously selected (before selection changed)
        /// </summary>
        protected TreeViewItemViewModel PreviousTreeItemViewModel
        {
            get;
            set;
        }

        /// <summary>
        /// The Controller of the current Scenario being edited.
        /// </summary>
        protected ScenarioAttributesController CurrentScenarioController
        {
            get; set;
        }

        /// <summary>
        /// The View of the current Scenario selected
        /// </summary>
        protected ScenarioItemViewModel CurrentScenarioItemViewModel
        {
            get; set;
        }

        /// <summary>
        /// Connect the Model
        /// </summary>
        protected void ConnectModel()
        {
            this.SessionExplorer.ElementIsSelectedPropertyChanged += OnSessiontExplorerElementIsSelected;
            this.SessionExplorer.View.MouseDoubleClick += OnSessionExplorerElementDoubleClick;
        }

        /// <summary>
        /// Disconnect the Model
        /// </summary>
        protected void DisonnectModel()
        {
            this.SessionExplorer.ElementIsSelectedPropertyChanged -= OnSessiontExplorerElementIsSelected;
            this.SessionExplorer.View.MouseDoubleClick -= OnSessionExplorerElementDoubleClick;
        }

        private void OnSessiontExplorerElementIsSelected(object sender, EventArgs e)
        {
            TreeViewItemViewModel item = sender as TreeViewItemViewModel;
            if (item.IsSelected)
            {
                if (sender is ScenarioItemViewModel)
                {
                    if (sender != CurrentScenarioItemViewModel)
                    {
                        ScenarioItemViewModel model = sender as ScenarioItemViewModel;
                        PreviousTreeItemViewModel = model;
                        ScenarioAttributesController controller = new ScenarioAttributesController(
                            new ScenarioAttributesModel(model.Data),
                            new ScenarioAttributesView());
                        this.View.AttributesPanel.Children.Add(controller.View);
                        CurrentScenarioItemViewModel = model;
                        CurrentScenarioController = controller;                        
                    }
                    return;
                }
            }
            //Remove the panel
            this.View.AttributesPanel.Children.Clear();
            CurrentScenarioItemViewModel = null;
            CurrentScenarioController = null;
            PreviousTreeItemViewModel = null;                
        }

        private void OnSessionExplorerElementDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItemViewModel item = (sender as SessionExplorerView).TreeView.SelectedItem as TreeViewItemViewModel;
            if (item != null && item.IsSelected)
            {
                if (item is ScenarioItemViewModel)
                {//Edit Scenario attributes.
                    EditScenarioAttributesView();
                }
            }
        }

        /// <summary>
        /// Edit the current Scenario
        /// </summary>
        protected void EditScenarioAttributesView()
        {
            View.DockPnlLeft.IsEnabled = false;//Disable the panel with the explorer in order to not allow selection change.
            CurrentScenarioController.Edit();
            CurrentScenarioController.SaveEvent += OnScenarioAttributesSaved;
            CurrentScenarioController.ApplyEvent += OnScenarioAttributesApplied;
            CurrentScenarioController.CancelEvent += OnScenarioAttributesCancelled;
        }

        /// <summary>
        /// To call at the end of the edition of the scenarion attributes view.
        /// </summary>
        protected void EndEditScenarioAttributesView()
        {
            // Memorise the current calculation model selected
            ScenarioItemViewModel scenarioItmViewMdel = CurrentScenarioItemViewModel;
            CurrentScenarioController.SaveEvent -= OnScenarioAttributesSaved;
            CurrentScenarioController.ApplyEvent -= OnScenarioAttributesApplied;
            CurrentScenarioController.CancelEvent -= OnScenarioAttributesCancelled;
            View.DockPnlLeft.IsEnabled = true;            
            scenarioItmViewMdel.Name = scenarioItmViewMdel.Data.name;            
            // Refresh the tree and the TabItem Name
            SessionExplorer.View.TreeView.Items.Refresh();
            SessionExplorer.View.TreeView.UpdateLayout();
            scenarioItmViewMdel.IsSelected = true;
            OnSessiontExplorerElementIsSelected(scenarioItmViewMdel, EventArgs.Empty);
        }

        private void OnScenarioAttributesCancelled(object sender, EventArgs e)
        {
            EndEditScenarioAttributesView();
        }

        private void OnScenarioAttributesApplied(object sender, EventArgs e)
        {
            EndEditScenarioAttributesView();
        }

        private void OnScenarioAttributesSaved(object sender, EventArgs e)
        {
            EndEditScenarioAttributesView();
        }

        /// <summary>
        /// Called when the Main Window View can be binded.
        /// </summary>
        /// <param name="window">The Main view to be binded</param>
        internal void BindView(LanguageServer.Robot.Monitor.MainWindow window)
        {            
            lock(PendingSession)                
            {
                this.SessionExplorer = new SessionExplorerController(window.SessionExplorerTree);
                this.SessionExplorer.StartScenarioHandler += SessionExplorer_StartScenarioHandler;
                //Add any pending session
                SessionExplorer.AddSessions(PendingSession);
                PendingSession.Clear();
                ConnectModel();
            }
        }
        /// <summary>
        /// Startinga Scenario Handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SessionExplorer_StartScenarioHandler(object sender, DocumentItemViewModel e)
        {
            var server = new ServerRobotConnectionController(new ProcessMessageConnection(ServerPath));
            ScenarioController = new MonitorLanguageServerRobotController(server, Util.DefaultScriptRepositorPath);
            ScenarioRobotConnectionController scenarioConnect =
                ScenarioController.ClientConnection as ScenarioRobotConnectionController;
            if (ScenarioController.Start())
            {
                if (!scenarioConnect.InitializeScenario(this.MonitoringConnection.Consumer.SessionModel, e.Data))
                {
                    MessageBox.Show(LanguageServer.Robot.Monitor.Properties.Resources.FailInitalizeScerarioRecording,
                        LanguageServer.Robot.Monitor.Properties.Resources.LSRMName,
                        MessageBoxButton.OK,
                        MessageBoxImage.Hand
                        );
                    var saveCtrl = ScenarioController;
                    ScenarioController = null;
                    saveCtrl.Dispose();
                    return;
                }
                //Open a dialog
                var result = MessageBox.Show(LanguageServer.Robot.Monitor.Properties.Resources.RecordingMessage,
                    LanguageServer.Robot.Monitor.Properties.Resources.LSRMName,
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information);
                if (result == MessageBoxResult.OK)
                {
                    if (!scenarioConnect.StopScenario(this.MonitoringConnection.Consumer.SessionModel, e.Data))
                    {
                        MessageBox.Show(LanguageServer.Robot.Monitor.Properties.Resources.FailStopScenarioRecording,
                            LanguageServer.Robot.Monitor.Properties.Resources.LSRMName,
                            MessageBoxButton.OK,
                            MessageBoxImage.Hand
                            );
                        var saveCtrl = ScenarioController;
                        ScenarioController = null;
                        saveCtrl.Dispose();
                        return;
                    }
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Script file (*.tlsp)|*.tlsp";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        var saveCtrl = ScenarioController;
                        ScenarioController = null;
                        Script recordedScenario;
                        if (scenarioConnect.SaveScenario(e.Data, saveFileDialog.FileName, out recordedScenario))
                        {                            
                            //Add the scenario to its Document
                            e.AddScenario(recordedScenario);
                        }
                        saveCtrl.Dispose();
                    }
                    else
                    {
                        var saveCtrl = ScenarioController;
                        ScenarioController = null;
                        saveCtrl.Dispose();
                    }
                }
                else
                {
                    var saveCtrl = ScenarioController;
                    ScenarioController = null;
                    saveCtrl.Dispose();                    
                }
            }
        }

        private LanguageServerRobotController MyScenarioController;
        private LanguageServerRobotController ScenarioController
        {
            get
            {
                return Interlocked.Exchange<LanguageServerRobotController>(ref MyScenarioController, MyScenarioController);
            }
            set
            {
                Interlocked.Exchange<LanguageServerRobotController>(ref MyScenarioController, value);
            }
        }

        private void RunScenarioController()
        {
            if (ScenarioController == null)
            {
                string path = "C:\\Users\\MAYANJE\\Source\\Repos\\TypeCobol_Develop\\TypeCobol\\bin\\Debug\\TypeCobol.LanguageServer.exe";
                var server = new ServerRobotConnectionController(new ProcessMessageConnection(path));
                 ScenarioController = new LanguageServerRobotController(server, Util.DefaultScriptRepositorPath);
                ScenarioController.Start();
            }
        }

        private void Consumer_LspMessageHandler(object sender, Common.Model.Message.LspMessage e)
        {
            //RunScenarioController();
            Log.LogWriter.WriteLine(e.Message);
        }

        public bool CanExecute(object parameter)
        {
            if (parameter == (App.Current.MainWindow as LanguageServer.Robot.Monitor.MainWindow).MenuItemQuit)
                return true;
            return false;
        }

        public void Execute(object parameter)
        {
            if (parameter == (App.Current.MainWindow as LanguageServer.Robot.Monitor.MainWindow).MenuItemQuit)
                App.Current.Shutdown();
        }

        /// <summary>
        /// Connect to the Monitoring Controller
        /// </summary>
        /// <param name="bConnect">true to establish a connection, false otherwise.</param>
        private void ConnectMonitoringController(Boolean bConnect)
        {
            if (MonitoringConnection != null)
            {
                if (bConnect)
                {
                    MonitoringConnection.StartSessionHandler += MonitoringConnection_StartSessionHandler;
                    MonitoringConnection.StartDocumentHandler += MonitoringConnection_StartDocumentHandler;
                    MonitoringConnection.RecordedMessageHandler += MonitoringConnection_RecordedMessageHandler;
                }
                else
                {
                    MonitoringConnection.StartSessionHandler -= MonitoringConnection_StartSessionHandler;
                    MonitoringConnection.StartDocumentHandler -= MonitoringConnection_StartDocumentHandler;
                    MonitoringConnection.RecordedMessageHandler -= MonitoringConnection_RecordedMessageHandler;
                }
            }
        }

        /// <summary>
        /// Handler of a received message handler.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="category"></param>
        /// <param name="kind"></param>
        /// <param name="message"></param>
        /// <param name="uri"></param>
        /// <param name="jsonObject"></param>
        private void MonitoringConnection_RecordedMessageHandler(Script script, Script.MessageCategory category, Common.Utilities.Protocol.Message_Kind kind, string message, string uri, JObject jsonObject)
        {
            if (ScenarioController != null)
            {
                if (category == Script.MessageCategory.Client)
                {
                    //In recording mode add the message
                    ScenarioRobotConnectionController controller =
                        ScenarioController.ClientConnection as ScenarioRobotConnectionController;
                    controller?.AddMessage(script, message);
                }
            }
            App.Current.Dispatcher.Invoke(() => SetActiveDocument(script));
        }

        /// <summary>
        /// Handler for a Start Session from the Monitor Connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The started session</param>
        private void MonitoringConnection_StartSessionHandler(object sender, Common.Model.Session e)
        {
            lock (PendingSession)
            {
                if (this.SessionExplorer != null)
                {
                    App.Current.Dispatcher.Invoke(() => AddSession(e, false));
                }
                else
                {//Pending session
                    PendingSession.Enqueue(e);
                }
            }
        }

        /// <summary>
        /// Handler for a document that have been started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The started script</param>
        private void MonitoringConnection_StartDocumentHandler(object sender, Common.Model.Script e)
        {
            App.Current.Dispatcher.Invoke(() => AddDocument(e));
        }

        /// <summary>
        /// Internal testing for a scenario engine.
        /// </summary>
        private void TestScenarioEngine()
        {            
        }
    }
}
