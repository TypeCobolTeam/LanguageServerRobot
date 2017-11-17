using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// A Client controller which is able to execute a script
    /// </summary>
    public class ScriptRobotConnectionController : ClientRobotConnectionController
    {
        /// <summary>
        /// Script Connection constructor
        /// </summary>
        /// <param name="script">The script to be executed by this Client</param>
        public ScriptRobotConnectionController(Model.Script script)
        {
            this.Script = script;
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
        /// Get the Replay Mode Contoller.
        /// </summary>
        internal ReplayModeController ReplayController
        {
            get
            {
                return (ReplayModeController)base.RobotModeController;
            }
        }

        /// <summary>
        /// Replay the message contained in the Script;
        /// </summary>
        /// <returns>Return true if all messages have been replayed, false otherwise.</returns>
        private bool ReplayMessage()
        {
            if (Script == null)
                return true;//No script => No message.
            if (!Script.IsValid)
                return false;
            //1) Send the "textDocument/didOpen" notification
            base.Consume(Script.didOpen);
            //2) Run thru all messages and take in account messages that are client requests or notifications.
            for(int i = 0; i < Script.messages.Count; i++)
            {
                if (Script.messages[i].category == Model.Script.MessageCategory.Client)
                {
                    ReplayController.ScriptMessageIndex = i;
                    base.Consume(Script.messages[i].message);
                }                
            }
            //3) Close the document
            base.Consume(Script.didClose);
            return true;
        }
        /// <summary>
        /// Starts the connection
        /// </summary>
        /// <returns>The connection's listener task if any, null otherwise</returns>
        public override async Task<bool> Start()
        {
            //We are replaying a script.
            if (Script == null)
                return false;

            //1) If the script containsan initialize request use it, otherwise use the default one
            base.Consume(Script.initialize ?? Utilities.Protocol.DEFAULT_INITIALIZE);

            //2) Replay all client messages
            Task<bool> replayTask = new Task<bool>(() => { return ReplayMessage();  });
            replayTask.Start();
            bool bResult = await replayTask;

            //3) Shutdown and exit
            base.Consume(Utilities.Protocol.DEFAULT_SHUTDOWN);
            base.Consume(Utilities.Protocol.DEFAULT_EXIT);

            return bResult;
        }
    }
}
