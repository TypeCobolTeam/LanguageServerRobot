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
    }
}
