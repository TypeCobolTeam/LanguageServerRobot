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
    public class ServerRobotConnectionController : JsonRPCConnection
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
