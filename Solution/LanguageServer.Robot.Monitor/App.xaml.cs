using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using LanguageServer.JsonRPC;
using LanguageServer.Robot.Common.Utilities;
using Mono.Options;
using LanguageServer.Robot.Monitor.Properties;
using LanguageServer.Robot.Common.Controller;

namespace LanguageServer.Robot.Monitor
{
    /// <summary>
    /// Interaction logic for App.xaml, This implement ICommand Interface so that MainWindow Action
    /// are controlled here.
    /// </summary>
    public partial class App : Application, System.Windows.Input.ICommand
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

        public static ConnectionLog Log        {
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
        /// Static constructor.
        /// </summary>
        static App()
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

        /// <summary>
        /// Entry point of the LSRM Application, with access to the command line arguments.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LSRM_Application_Startup(object sender, StartupEventArgs e)
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
            };
            System.Collections.Generic.List<string> arguments;
            try { arguments = p.Parse(e.Args); }
            catch (OptionException ex)
            {
                MessageBox.Show(ex.Message, LanguageServer.Robot.Monitor.Properties.Resources.LSRMName, MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown();
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
                this.Shutdown();
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

        private void Consumer_LspMessageHandler(object sender, Common.Model.Message.LspMessage e)
        {
            Log.LogWriter.WriteLine(e.Message);
        }

        public bool CanExecute(object parameter)
        {
            if (parameter == (MainWindow as LanguageServer.Robot.Monitor.MainWindow).MenuItemQuit)
                return true;
            return false;
        }

        public void Execute(object parameter)
        {
            if (parameter == (MainWindow as LanguageServer.Robot.Monitor.MainWindow).MenuItemQuit)
                this.Shutdown();
        }
    }
}
