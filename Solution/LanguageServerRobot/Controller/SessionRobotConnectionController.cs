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
    /// A Client controller which is able to execute a session
    /// </summary>
    public class SessionRobotConnectionController : AbstractReplayRobotConnectionController
    {
        /// <summary>
        /// Session Connection constructor
        /// </summary>
        /// <param name="session">The session to be executed by this Client</param>
        /// <param name="scriptRepositoryPath">The script repository path.</param>
        public SessionRobotConnectionController(Model.Session session, string scriptRepositoryPath = null)
        {
            this.Session = session;
            ControllerState = ConnectionState.New;
            Successfull = true;//We assume that this session is successfull
            //Create for each Script to replay its Script Controller
            List<ScriptRobotConnectionController> controllers = new List<ScriptRobotConnectionController>();
            foreach(string scriptPath in session.scripts)
            {
                Script script;
                Exception exc;
                bool bValid = ReadScript(scriptPath, out script, out exc);
                if (bValid)
                {
                    ScriptRobotConnectionController controller = new ScriptRobotConnectionController(script, this.MessageConnection);
                    ReplayModeController replayController = new ReplayModeController(script, scriptPath, scriptRepositoryPath);                    
                    controller.RobotModeController = replayController;
                    controllers.Add(controller);
                }
                else
                {
                    Successfull = false;
                }
            }
            ScriptControllers = controllers.AsReadOnly();
        }

        /// <summary>
        /// A Lock to access to the Replay Controller instance.
        /// </summary>
        private object ReplayControllerLock = new object();
        /// <summary>
        /// Is this session successfull.
        /// </summary>
        public bool Successfull
        {
            get;
            private set;
        }

        /// <summary>
        /// Read a script file
        /// </summary>
        /// <param name="scriptPath"> The Script's file path</param>
        /// <param name="script">[out] The Script model read</param>
        /// <param name="exc">[out] Any exception that occured if failed.</param>
        /// <returns></returns>
        private bool ReadScript(string scriptPath, out Script script, out Exception exc)
        {
            exc = null;
            if (!Util.ReadScriptFile(scriptPath, out script, out exc))
            {//Invalid Script File.
                System.Console.Out.WriteLine(string.Format(Resource.FailReadScriptFile, scriptPath, exc != null ? exc.Message : ""));
                LogWriter?.WriteLine(string.Format(Resource.FailReadScriptFile, scriptPath, exc != null ? exc.Message : ""));
                return false;
            }
            return true;
        }

        /// <summary>
        /// The Server Path
        /// </summary>
        public string ServerPath
        {
            get;
            set;
        }
        /// <summary>
        /// The session to be executed by this controller
        /// </summary>
        public Model.Session Session
        {
            get;
            private set;
        }

        /// <summary>
        /// All Script controllers for this session
        /// </summary>
        public IReadOnlyList<ScriptRobotConnectionController> ScriptControllers
        {
            get;
            private set;
        }

        /// <summary>
        /// Propagate Connection Log settings to this.
        /// </summary>
        /// <param name="log">The Connection Logs setting.</param>
        public void PropagateConnectionLogs(ConnectionLog log = null)
        {
            log = log ?? ConnectionLog.GetInstance();
            foreach (ScriptRobotConnectionController controller in ScriptControllers)
            {
                controller.PropagateConnectionLogs(log);
                if (controller.RobotModeController is IConnectionLog)
                    log.AssignTo(RobotModeController as IConnectionLog);
            }
        }


        /// <summary>
        /// The Current Replay controller.
        /// </summary>
        private ReplayModeController ReplayController;

        /// <summary>
        /// Starts the connection
        /// </summary>
        /// <returns>The connection's listener task if any, null otherwise</returns>
        public override async Task<bool> Start()
        {
            bool bResult = true;
            try
            {
                if (Session == null)
                {
                    ControllerState = ConnectionState.Disposed;
                    return false;
                }

                //Now we start listening
                ControllerState = ConnectionState.Listening;

                //Register message handlers
                ConnectMessageEventHandlers(true);
                //Send Initialization request to teh server.
                if (!PerformInitializeRequest(Session.initialize, false))
                {
                    ConnectMessageEventHandlers(false);
                    return false;
                }
                else
                {
                    ConnectMessageEventHandlers(false);
                }

                foreach (ScriptRobotConnectionController controller in ScriptControllers)
                {//For each script                    
                    Task<bool> task = null;
                    try
                    {
                        lock (ReplayControllerLock)
                        {
                            ReplayController = controller.ReplayController;
                        }
                        controller.ProducedMessageFilter = this.ProducedMessageFilter;
                        task = controller.Start();
                        await task;
                        bool bMyResult = task != null ? task.Result : false;
                        if (!bMyResult)
                            bResult = false;
                    }
                    catch (Exception e)
                    {
                        controller.LogWriter?.WriteLine(e.Message);
                        switch (controller.State)
                        {
                            case ConnectionState.Closed:                                
                                break;
                            default:
                                bResult = false;
                                break;
                        }
                    }
                    lock (ReplayControllerLock)
                    {
                        ReplayController = null;
                    }                    
                }
                ControllerState = ConnectionState.Closed;//CLOSED            
                return bResult;
            }
            catch (Exception e)
            {
                if (ControllerState != ConnectionState.Closed)
                {
                    ControllerState = ConnectionState.Closed;//CLOSED            
                }
                return false;
            }
        }

        /// <summary>
        /// Handling a message that comes from the Client: From Me.
        /// </summary>
        /// <param name="message"></param>
        public override void FromClient(string message)
        {
            lock (ReplayControllerLock)
            {
                if (ReplayController != null)
                    ReplayController.FromClient(message);
                else
                    base.FromClient(message);
            }
        }

        /// <summary>
        /// Handler for a message that comes from the Server, that is to say from me.
        /// </summary>
        /// <param name="message"></param>
        public override void FromServer(string message)
        {
            lock (ReplayControllerLock)
            {
                if (ReplayController == null)
                    RobotModeController.FromServer(message);
                else
                    ReplayController.FromServer(message);
            }
        }
    }
}
