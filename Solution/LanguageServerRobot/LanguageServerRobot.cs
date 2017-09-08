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
        /// Main Entry point of the Server.
        /// </summary>
        /// <param name="args">Arguments: arg[0] the LogLevel (0=Lifecycle,1=Message,2=Protocol) - args[1] a Log File</param>
        /// <see cref="TypeCobol.LanguageServer.StdioHttp.ServerLogLevel"/>
        static void Main(string[] args)
        {
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
                { "s|server=","{PATH} the server path", (string v) => ServerPath = v },
            };

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
                            var robot = new LanguageServerRobotController(client, server);
                            robot.PropagateConnectionLogs();
                            robot.Start();
                        }
                        break;
                    case LanguageServerRobotController.ConnectionMode.Client:
                        {
                            var server = new ServerRobotConnectionController(new ProcessMessageConnection(ServerPath));
                            var robot = new LanguageServerRobotController(server);
                            robot.PropagateConnectionLogs();
                            robot.Start();
                        }
                        break;
                }
            }
            finally
            {
                logger.Close();
            }
        }

        /// <summary>
        /// Command Line Option Set
        /// </summary>
        public static OptionSet Options
        {
            get;
            internal set;
        }
    }
}
