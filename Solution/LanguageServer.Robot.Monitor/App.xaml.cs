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
using LanguageServer.Robot.Monitor.Controller;

namespace LanguageServer.Robot.Monitor
{
    /// <summary>
    /// Interaction logic for App.xaml, This implement ICommand Interface so that MainWindow Action
    /// are controlled here.
    /// </summary>
    public partial class App : Application, System.Windows.Input.ICommand
    {
        /// <summary>
        /// Main Language Server Robot Monitor Controller
        /// </summary>
        public LanguageServerRobotMonitor MonitorController
        {
            get;
            set;
        }

        public static ConnectionLog Log        {
            get;
            internal set;
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
        /// Entry point of the LSRM Application, with access to the command line arguments.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LSRM_Application_Startup(object sender, StartupEventArgs e)
        {
            MonitorController = new LanguageServerRobotMonitor();
            MonitorController.Main(sender, e);
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
