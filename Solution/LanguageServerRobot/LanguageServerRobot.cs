using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;
using LanguageServerRobot.Controller;
using LanguageServerRobot.Utilities;
using Mono.Options;

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
        public static List<Tuple<string,FileType>> Files
        {
            get;
            internal set;
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
                { "lf|logfile=","{PATH} the target log file", (string v) => LogFile = v },
                { "r|robot",  "Robot Client/Server mode.", v => Mode = (v!=null)
                        ? LanguageServerRobotController.ConnectionMode.ClientServer
                        : LanguageServerRobotController.ConnectionMode.Client
                },
                { "c|client",  "Robot Client/Replay mode.", v => Mode = LanguageServerRobotController.ConnectionMode.Client
                },
                { "script=",  "{PATH} the script file to be replayed.", (string v) => { Mode = LanguageServerRobotController.ConnectionMode.Client;
                    if (version != null)Files.Add(new Tuple<string,FileType>(v,FileType.ScriptFile)); }
                },
                { "suite=",  "{PATH} the session file to be replayed.", (string v) => { Mode = LanguageServerRobotController.ConnectionMode.Client;
                    if (version != null)Files.Add(new Tuple<string,FileType>(v,FileType.SessionFile)); }
                },
                { "s|server=","{PATH} the server path", (string v) => ServerPath = v },
                { "d|Scripts repository directory=","{PATH} Scripts repository directory", (string v) => ScriptRepositoryPath = v },
            };
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

            if (ServerPath == null)
            {//No server path specified ==> THINK ABOUT HAVING a CONFIG FILE.
                //Take the default.
                ServerPath = DefaultTypeCobolLanguageServerPath;
            }
            if (ScriptRepositoryPath == null)
            {//Default path the user document path
                ScriptRepositoryPath = Util.DefaultScriptRepositorPath;
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
            ConnectionLog logger = new ConnectionLog();
            switch(LogLevel)
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
                            var server = new ServerRobotConnectionController(new ProcessMessageConnection(ServerPath));
                            var robot = new LanguageServerRobotController(client, server, ScriptRepositoryPath);
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
                        break;
                    case LanguageServerRobotController.ConnectionMode.Client:
                        {
                            if (Files.Count == 0)
                            {//No file to test
                                System.Console.Out.WriteLine(Resource.NoSessionOrScriptFile);
                                p.WriteOptionDescriptions(System.Console.Out);

                                return 0;
                            }
                            ClientRobotConnectionController client = null;
                            Model.Script script = null;
                            Model.Session session = null;
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
                            var server = new ServerRobotConnectionController(new ProcessMessageConnection(ServerPath));
                            var robot = script != null ? new LanguageServerRobotController(script, server, ScriptRepositoryPath)
                                                       : new LanguageServerRobotController(session, server, ScriptRepositoryPath);
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
