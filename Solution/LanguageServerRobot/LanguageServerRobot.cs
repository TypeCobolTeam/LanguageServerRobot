using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;
using LanguageServerRobot.Utilities;

namespace LanguageServerRobot
{
    public class LanguageServerRobot
    {
        /// <summary>
        /// Main Entry point of the Server.
        /// </summary>
        /// <param name="args">Arguments: arg[0] the LogLevel (0=Lifecycle,1=Message,2=Protocol) - args[1] a Log File</param>
        /// <see cref="TypeCobol.LanguageServer.StdioHttp.ServerLogLevel"/>
        static void Main(string[] args)
        {
            ConnectionLogLevel logLevel;
            TextWriter logWriter;

            GetArguments(args, out logLevel, out logWriter);
            //Prepare a Connection logger
            ConnectionLog logger = new ConnectionLog();
            switch(logLevel)
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
                /*
                // Configure the protocols stack
                var client = new ClientRobotConnectionController();
                var server = new ServerRobotConnectionController(new ProcessMessageConnection("C:\TypeCobol.LanguageServer.exe"));
                var robot = new LanguageServerRobotController(client, server);
                robot.start();
                */
            }
            finally
            {
                logger.Close();
            }
        }

        /// <summary>
        /// Collect Arguments
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <param name="logLevel">Output: The LogLevel from the arguments</param>
        /// <param name="logWriter">Output: The Log Writer from the arguments</param>
        static void GetArguments(String[] args, out ConnectionLogLevel logLevel, out TextWriter logWriter)
        {
            logLevel = ConnectionLogLevel.Lifecycle;
            logWriter = null;
            try
            {
                if (args != null && args.Length != 0)
                {//Standard output
                    try
                    {
                        // args[0] : Trace level
                        logLevel = (ConnectionLogLevel)Int32.Parse(args[0]);
                        if (!System.Enum.IsDefined(typeof(ConnectionLogLevel), (Int32)logLevel)) ;
                        {
                            logLevel = ConnectionLogLevel.Protocol;
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.Error.WriteLine(e.Message);
                    }

                    if (args.Length > 1)
                    {
                        // Open log file
                        // args[1] : Log file name
                        string logFile = args[1];
                        try
                        {
                            StreamWriter sw = new StreamWriter(logFile);
                            sw.AutoFlush = true;
                            logWriter = sw;
                        }
                        catch (Exception e)
                        {
                            System.Console.Error.WriteLine(e.Message);
                        }
                    }
                }
            }
            finally
            {
                if (logWriter == null)
                {
                    logWriter = new DebugTextWriter();
                }
            }
        }
    }
}
