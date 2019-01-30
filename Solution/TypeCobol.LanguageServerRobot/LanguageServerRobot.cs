using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TypeCobol.LanguageServer.JsonRPC;
using TypeCobol.LanguageServer.Robot.Common;
using TypeCobol.LanguageServer.Robot.Common.Model;
using TypeCobol.LanguageServer.Robot.Common.Controller;
using TypeCobol.LanguageServer.Robot.Common.Utilities;
using Mono.Options;
using TypeCobol.LanguageServer.Robot.Common.Connection;
using Newtonsoft.Json.Linq;

namespace LanguageServerRobot
{
    public class LanguageServerRobot
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
        /// Language Server Robot mode: lient/Server or only Client mode.
        /// By Default its Client/Server mode.
        /// </summary>
        public static LanguageServerRobotController.ConnectionMode Mode
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Server path.
        /// </summary>
        public static string ServerPath
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
        /// Server options.
        /// </summary>
        public static string ServerOptions
        {
            get; internal set;
        }

        /// <summary>
        /// The file type
        /// </summary>
        public enum FileType
        {
            ScriptFile,
            SessionFile
        };

        /// <summary>
        /// The list of Files
        /// </summary>
        public static List<Tuple<string, FileType>> Files
        {
            get;
            internal set;
        }

        /// <summary>
        /// Prompt replay.
        /// </summary>
        public static bool PromptReplay
        {
            get; internal set;
        }

        public static bool InversionOfControl
        {
            get;
            internal set;
        }

        /// <summary>
        /// Stop At first error option.
        /// </summary>
        public static bool StopAtFirstError
        {
            get; internal set;
        }

        /// <summary>
        /// The path to the json file which contains the "initialize" request to be used
        /// </summary>
        public static string InitializeRequestPath
        {
            get; set;
        }

        /// <summary>
        /// The Initialize request as read in the -init option file
        /// </summary>
        public static string InitializeRequestOption
        {
            get; set;
        }

        /// <summary>
        /// The path to the json file which contains the workspace/didChangeConfiguration notification to be used
        /// </summary>
        public static string ConfigNotifyPath
        {
            get; set;
        }

        /// <summary>
        /// The didChangeConfiguration notification as read in the -config option file
        /// </summary>
        public static string ConfigNotifyOption
        {
            get; set;
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        static LanguageServerRobot()
        {
            Files = new List<Tuple<string, FileType>>();
        }
        const string DefaultTypeCobolLanguageServerPath = "C:\\TypeCobol\\Sources\\##Latest_Release##\\TypeCobol.LanguageServer.exe";

        /// <summary>
        /// Main Entry point of the Server.
        /// </summary>
        /// <param name="args">Arguments: arg[0] the LogLevel (0=Lifecycle,1=Message,2=Protocol) - args[1] a Log File</param>
        /// <see cref="TypeCobol.LanguageServer.StdioHttp.ServerLogLevel"/>
        static int Main(string[] args)
        {
            bool help = false;
            bool version = false;
            bool monitoring = false;
            //Default Connection Mod eid Client/Server
            Mode = LanguageServerRobotController.ConnectionMode.ClientServer;

            var p = new OptionSet()
            {
                "USAGE",
                "  "+ProgName+" [OPTIONS]",
                "",
                "VERSION:",
                "  "+Version,
                "",
                "DESCRIPTION:",
                "  Run the Language Server Robot.",
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
                            catch (Exception e)
                            {
                                System.Console.Error.WriteLine(e.Message);
                            }
                        }
                    }
                },
                { "v|version","Show version", _ => version = true },
                { "h|help","Show help", _ => help = true },
                { "lf|logfile=","{PATH} the target log file", (string v) => LogFile = v },
                { "p|prompt",  "Prompt to start a replay", v => PromptReplay = true },
                { "r|robot",  "Robot Client/Server mode.", v => Mode = (v!=null)
                        ? LanguageServerRobotController.ConnectionMode.ClientServer
                        : LanguageServerRobotController.ConnectionMode.Client
                },
                { "c|client",  "Robot Client/Replay mode.", v => Mode = LanguageServerRobotController.ConnectionMode.Client
                },
                { "ioc",  "Inversion of control.", _ => InversionOfControl = true
                },
                { "script=",  "{PATH} the script file to be replayed.", (string v) => { Mode = LanguageServerRobotController.ConnectionMode.Client;
                    if (!version)Files.Add(new Tuple<string,FileType>(v,FileType.ScriptFile)); }
                },
                { "suite=",  "{PATH} the session file to be replayed.", (string v) => { Mode = LanguageServerRobotController.ConnectionMode.Client;
                    if (!version)Files.Add(new Tuple<string,FileType>(v,FileType.SessionFile)); }
                },
                { "s|server=","{PATH} the server path", (string v) => ServerPath = v },
                { "so|soptions=","Server options", (string v) => ServerOptions = v },
                { "d|dir=","{PATH} Scripts repository directory", (string v) => ScriptRepositoryPath = v },
                { "m|monitoring","Show the monitoring Window", _ => monitoring = true },
                { "e|stoperror","Stop a replay at the first error", _ => StopAtFirstError = true },
                { "init=",  "{PATH} the json file for a LSP \"initialize\" request.", (string v) =>
                    {
                        InitializeRequestPath = v;
                    }
                },
                { "config=",  "{PATH} the json file for a LSP \"workspace/didChangeConfiguration\" notification.", (string v) =>
                    {
                        ConfigNotifyPath = v;
                    }
                },
            };
            //Util.MessageBoxTimeoutW(IntPtr.Zero, "TESTING", "TESTING", 0, 0, 40000);
            System.Collections.Generic.List<string> arguments;
            try { arguments = p.Parse(args); }
            catch (OptionException ex) { return exit(1, ex.Message); }

            if (help)
            {
                p.WriteOptionDescriptions(System.Console.Out);
                return 0;
            }
            if (version)
            {
                System.Console.WriteLine(Version);
                return 0;
            }
            if (monitoring && Mode != LanguageServerRobotController.ConnectionMode.ClientServer)
            {//using monitoring in ClientServer mode only.
                System.Console.WriteLine(Resource.MonitoringInClientServerModeOnly);
                return -1;
            }

            if (ServerPath == null)
            {//No server path specified ==> THINK ABOUT HAVING a CONFIG FILE.
                //Take the default.
                ServerPath = DefaultTypeCobolLanguageServerPath;
            }
            if (ScriptRepositoryPath == null)
            {//Default path the user document path
                ScriptRepositoryPath = Util.DefaultScriptRepositorPath;
            }

            if (InitializeRequestPath != null)
            {
                string initializeMessage;
                JObject jsonMessage;
                Exception exc;
                if (
                    !Protocol.LoadInitializeRequest(InitializeRequestPath, out initializeMessage, out jsonMessage,
                        out exc))
                {
                    System.Console.WriteLine(Resource.FailToLoadInitFile);
                    return -1;
                }
                InitializeRequestOption = initializeMessage;
            }

            if (ConfigNotifyPath != null)
            {
                string configMessage;
                JObject jsonMessage;
                Exception exc;
                if (
                    !Protocol.LoadConfigurationNotification(ConfigNotifyPath, out configMessage, out jsonMessage,
                        out exc))
                {
                    System.Console.WriteLine(Resource.FailToLoadConfigFile);
                    return -1;
                }
                ConfigNotifyOption = configMessage;
            }

            TextWriter logWriter = null;
            if (LogFile != null)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(LogFile);
                    sw.AutoFlush = true;
                    logWriter = sw;
                }
                catch (Exception e)
                {
                    System.Console.Error.WriteLine(e.Message);
                    logWriter = new DebugTextWriter();
                }
            }
            else
            {
                logWriter = new DebugTextWriter();
            }

            //Prepare a Connection logger
            ConnectionLog logger = ConnectionLog.GetInstance();
            switch (LogLevel)
            {
                case ConnectionLogLevel.Lifecycle:
                    logger.LogWriter = logWriter;
                    break;
                case ConnectionLogLevel.Message:
                    logger.MessageLogWriter = logWriter;
                    break;
                case ConnectionLogLevel.Protocol:
                    logger.ProtocolLogWriter = logWriter;
                    break;
            }

            // Open log file
            try
            {
                switch (Mode)
                {
                    case LanguageServerRobotController.ConnectionMode.ClientServer:
                        {
                            //Create and start Language Server Robot controller.
                            var client = new ClientRobotConnectionController();
                            var server = new ServerRobotConnectionController(new ProcessMessageConnection(ServerPath, ServerOptions));
                            var robot = monitoring
                                ? new LanguageServerRobotController(client, server, DataConnectionfactory.Create(DataConnectionfactory.ConnectionType.PIPE, DataConnectionfactory.ConnectionSide.Producer), ScriptRepositoryPath)
                                : new LanguageServerRobotController(client, server, ScriptRepositoryPath);
                            robot.ServerOptions = ServerOptions;
                            robot.PropagateConnectionLogs();
                            if (!robot.Start(false))
                            {
                                return -1;
                            }
                            else
                            {
                                bool bResult = robot.WaitExit();
                                robot.Dispose();
                                return bResult ? 0 : -1;
                            }
                        }
                        break;
                    case LanguageServerRobotController.ConnectionMode.Client:
                        {
                            if (Files.Count == 0)
                            {//No file to test
                                System.Console.Out.WriteLine(TypeCobol.LanguageServer.Robot.Common.Resource.NoSessionOrScriptFile);
                                p.WriteOptionDescriptions(System.Console.Out);

                                return 0;
                            }
                            ClientRobotConnectionController client = null;
                            Script script = null;
                            Session session = null;
                            switch (Files[0].Item2)
                            {
                                case FileType.ScriptFile:
                                    {
                                        if (Util.HasScriptFileExtension(Files[0].Item1))
                                        {
                                            Exception exc = null;
                                            if (!Util.ReadScriptFile(Files[0].Item1, out script, out exc))
                                            {//Invalid Script File.
                                                System.Console.Out.WriteLine(string.Format(Resource.FailReadScriptFile, Files[0], exc != null ? exc.Message : ""));
                                                logger.LogWriter?.WriteLine(string.Format(Resource.FailReadScriptFile, Files[0], exc != null ? exc.Message : ""));
                                                return -1;
                                            }
                                        }
                                    }
                                    break;
                                case FileType.SessionFile:
                                    {
                                        if (Util.HasSessionFileExtension(Files[0].Item1))
                                        {
                                            Exception exc = null;
                                            if (!Util.ReadSessionFile(Files[0].Item1, out session, out exc))
                                            {//Invalid Script File.
                                                System.Console.Out.WriteLine(string.Format(Resource.FailReadSessionFile, Files[0], exc != null ? exc.Message : ""));
                                                logger.LogWriter?.WriteLine(string.Format(Resource.FailReadSessionFile, Files[0], exc != null ? exc.Message : ""));
                                                return -1;
                                            }
                                        }
                                    }
                                    break;
                            }
                            if (script != null)
                            {
                                return ReplayScript(Files[0].Item1, script, StopAtFirstError, PromptReplay);
                            }
                            else
                            {
                                return ReplaySession(Files[0].Item1, session, StopAtFirstError);
                            }
                            //var server = new ServerRobotConnectionController(new ProcessMessageConnection(ServerPath));
                            //var robot = script != null ? new LanguageServerRobotController(Files[0].Item1, script, server, ScriptRepositoryPath)
                            //                           : new LanguageServerRobotController(Files[0].Item1, session, server, ScriptRepositoryPath);
                            //robot.PropagateConnectionLogs();
                            //if (!robot.Start())
                            //{
                            //    return -1;
                            //}
                            //else
                            //{
                            //    bool bResult = robot.WaitExit();
                            //    return bResult ? 0 : -1;
                            //}
                        }
                        break;
                }
            }
            finally
            {
                logger.Close();
            }
            return 0;
        }

        /// <summary>
        /// Plays a script
        /// </summary>
        /// <param name="script_path">The path of the script to replay</param>
        /// <param name="script">The script model to replay</param>
        /// <returns>0 if no error -1 otherwise.</returns>
        private static int ReplayScript(string script_path, Script script, bool bStopAtFirstError, bool promptReplay)
        {
            //Check if we have an initialzie request to apply
            if (InitializeRequestOption != null)
                script.initialize = InitializeRequestOption;
            //Check if we have an workspace/didChangeConfiguration notification to apply
            if (ConfigNotifyOption != null)
                script.did_change_configuation = ConfigNotifyOption;
            if (InversionOfControl)
                return LanguageServerRobotController.DumpScript(script_path, script, ScriptRepositoryPath, bStopAtFirstError);
            else
                return LanguageServerRobotController.ReplayScript(script_path, script, ServerPath, ServerOptions, ScriptRepositoryPath, bStopAtFirstError, promptReplay);
        }

        /// <summary>
        /// Plays a session
        /// </summary>
        /// <param name="session_path">The path of the session to replay</param>
        /// <param name="session">The session model to replay</param>
        /// <returns>0 if no error -1 otherwise.</returns>
        private static int ReplaySession(string session_path, Session session, bool bStopAtFirstError)
        {
            return LanguageServerRobotController.ReplaySession(session_path, session, ServerPath, ServerOptions, ScriptRepositoryPath, bStopAtFirstError);
        }

        /// <summary>
        /// Command Line Option Set
        /// </summary>
        public static OptionSet Options
        {
            get;
            internal set;
        }

        static int exit(int code, string message)
        {
            string errmsg = ProgName + ": " + message + "\n";
            errmsg += "Try " + ProgName + " --help for usage information.";
            System.Console.WriteLine(errmsg);
            return code;
        }
    }
}
