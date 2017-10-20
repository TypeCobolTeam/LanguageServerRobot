using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// Class that will handle the Server side Robot connection business logic
    /// </summary>
    public class ServerRobotConnectionController : JsonRPCConnection, IRobotModeController
    {
        /// <summary>
        /// Message Connection constructor
        /// </summary>
        /// <param name="messageConnection">The Message Connection with the client.</param>
        public ServerRobotConnectionController(IMessageConnection messageConnection) : base(messageConnection)
        {            
        }

        /// <summary>
        /// Constructor using a ProcessMessageConnection
        /// </summary>
        /// <param name="connection"></param>
        public ServerRobotConnectionController(ProcessMessageConnection connection) : this((IMessageConnection)connection)
        {
            connection.ProcessExited += ProcessExitedEventHandler;
        }

        public bool IsModeInitialized
        {
            get
            {
                return RobotModeController.IsModeInitialized;
            }
        }

        public bool IsModeStarted
        {
            get
            {
                return RobotModeController.IsModeStarted;
            }
        }

        public bool IsModeStopped
        {
            get
            {
                return RobotModeController.IsModeStopped;
            }
        }

        /// <summary>
        /// The Robot Mode Controller for the Business Logic.
        /// </summary>
        public IRobotModeController RobotModeController
        {
            get;
            internal set;
        }

        /// <summary>
        /// Handler for a message that commes from the Client
        /// </summary>
        /// <param name="message"></param>
        public void FromClient(string message)
        {
            //Do Nothing, let the client controller do its logic.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handler for a message that comes from the Server, that is to say from me.
        /// </summary>
        /// <param name="message"></param>
        public void FromServer(string message)
        {
            System.Diagnostics.Contracts.Contract.Requires(RobotModeController != null);
            RobotModeController.FromServer(message);
        }

        /// <summary>
        /// Process exited event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessExitedEventHandler(object sender, EventArgs e)
        {
        }
    }
}
