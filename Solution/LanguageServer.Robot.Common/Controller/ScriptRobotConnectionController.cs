using LanguageServer.JsonRPC;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.Robot.Common.Controller
{
    /// <summary>
    /// A Client controller which is able to execute a script
    /// </summary>
    public class ScriptRobotConnectionController : AbstractReplayRobotConnectionController
    {
        /// <summary>
        /// Script Connection constructor
        /// </summary>
        /// <param name="script">The script to be executed by this Client</param>
        /// <param name="bFromSession">true if this Script controller is for a session to be replayed,
        /// in this case no "initialize" and "shutdown" requets will be replayed.</param>
        public ScriptRobotConnectionController(Model.Script script, bool bFromSession = false)
        {
            this.Script = script;
            FromSession = bFromSession;
        }

        /// <summary>
        /// Script Connection constructor
        /// <param name="messageConnection">The message connection to be used</param>
        /// </summary>
        /// <param name="script">The script to be executed by this Client</param>
        public ScriptRobotConnectionController(Model.Script script, IMessageConnection messageConnection) : base(messageConnection)
        {
            this.Script = script;
            FromSession = true;
        }

        /// <summary>
        /// The script to be executed by this controller
        /// </summary>
        public Model.Script Script
        {
            get;
            private set;
        }

        /// <summary>
        /// Is this Script Controller for a Session controller ? In this case no "initialize" and "shutdown" messages
        /// will be sent to the server.
        /// </summary>
        public bool FromSession
        {
            get;
            private set;
        }

        /// <summary>
        /// Determine if yes or not we must stop At the first error.
        /// </summary>
        public bool StopAtFirstError
        {
            get
            {
                return ReplayController != null ? ReplayController.StopAtFirstError : false;
            }
            set
            {
                if (ReplayController != null)
                    ReplayController.StopAtFirstError = value;

            }
        }

        /// <summary>
        /// The Index of the firt error encountered in the Script.
        /// </summary>
        public long ErrorIndex
        {
            get
            {
                return ReplayController != null ? ReplayController.ErrorIndex : -1;
            }
        }

        /// <summary>
        /// Replay the message contained in the Script;
        /// </summary>
        /// <returns>Return true if all messages have been replayed, false otherwise.</returns>
        private bool ReplayMessages()
        {
            if (Script == null)
                return true;//No script => No message.
            if (!Script.IsValid)
                return false;
            //1) Send the "textDocument/didOpen" notification
            base.Consume(Script.didOpen);
            //2) Run thru all messages and take in account messages that are client requests or notifications.
            int nNotificationCount = 0;//Counting notifications
            for(int i = 0; i < Script.messages.Count && (ReplayController.ErrorIndex < 0 || !StopAtFirstError); i++)
            {
                if (Script.messages[i].category == Model.Script.MessageCategory.Client)
                {
                    ReplayController.ScriptMessageIndex = i;
                    ResponseResultOrError response = ReplayMessage(Script.messages[i].message);
                }    
                else if (Script.messages[i].category == Model.Script.MessageCategory.Server)
                {
                    JObject jsonObject = null;
                    if (Utilities.Protocol.IsNotification(Script.messages[i].message, out jsonObject))
                    {
                        nNotificationCount += 1;
                        //This is a message from the server.
                        //Wait for a similar notification comming from the server, so here we wait 8s
                        bool bStop = false;
                        bool bFailed = false;
                        DateTime startSecond = DateTime.Now;
                        do
                        {                            
                            lock (InCommingNotification)
                            {
                                int count = InCommingNotification.Count;
                                if (nNotificationCount <= count)
                                {
                                    bStop = true;
                                }
                            }
                            if (!bStop)
                            {
                                System.Threading.Thread.Sleep(50);
                                DateTime curSecond = DateTime.Now;
                                if ((curSecond - startSecond).TotalSeconds >= 8)
                                {
                                    bStop = true;
                                    bFailed = true;
                                }
                            }
                        } while (!bStop);
                        if (bFailed)
                        {
                            if (ReplayController.ErrorIndex < 0)
                                ReplayController.ErrorIndex = i;
                            if (StopAtFirstError)
                                break;
                        }
                    }
                }
            }
            //3) Close the document
            if (ReplayController.ErrorIndex < 0)
                base.Consume(Script.didClose);
            return ReplayController.ErrorIndex < 0;
        }

        /// <summary>
        /// Forces the server to shutdown
        /// </summary>
        /// <param name="message">any shutdown exception message to be reported</param>
        protected override void ForceServerShutDown(string message = null)
        {            
            if (!FromSession)
            {
                base.ForceServerShutDown(message);
            }
            else
            {
                WriteReplayResult(message);
            }
        }

        /// <summary>
        /// Starts the connection
        /// </summary>
        /// <returns>The connection's listener task if any, null otherwise</returns>
        public override async Task<bool> Start()
        {
            try
            {
                //We are replaying a script.
                if (Script == null)
                {
                    ControllerState = ConnectionState.Disposed;
                    return false;
                }

                //Register message handlers
                ConnectMessageEventHandlers(true);

                //Now we start listening
                ControllerState = ConnectionState.Listening;

                //JObject jsonObject = null;
                if (!FromSession)
                {
                    if (!PerformInitializeRequest(Script.initialize, true))
                    {
                        ConnectMessageEventHandlers(false);
                        return false;
                    }
                    //If there is a did change configuration also sent it.
                    if (this.Script.did_change_configuation != null)
                    {
                        this.Consume(this.Script.did_change_configuation);
                    }
                }
                else
                {//In this case forces the Replay Mode Controler to be initialized and started.
                    ReplayController.State = AbstractModeController.ModeState.Initialized | AbstractModeController.ModeState.Start;
                }

                //2) Replay all client messages
                Task<bool> replayTask = new Task<bool>(() => { return ReplayMessages(); });
                replayTask.Start();
                bool bResult = await replayTask;

                //3) Shutdown and exit
                ForceServerShutDown();

                ConnectMessageEventHandlers(false);
                ControllerState = ConnectionState.Closed;//CLOSED            
                return bResult;
            }
            catch(Exception e)
            {
                if (ControllerState != ConnectionState.Closed)
                {
                    WriteReplayResult(e.Message);
                    ConnectMessageEventHandlers(false);
                    ControllerState = ConnectionState.Closed;//CLOSED            
                }
                return false;
            }
        }
    }
}
