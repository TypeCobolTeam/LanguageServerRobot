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

namespace LanguageServer.Robot.Monitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
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
                                if (!System.Enum.IsDefined(typeof(ConnectionLogLevel), (Int32)LogLevel));
                                {
                                    LogLevel = ConnectionLogLevel.Protocol;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Console.Error.WriteLine(ex.Message);
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
        }
    }
}
