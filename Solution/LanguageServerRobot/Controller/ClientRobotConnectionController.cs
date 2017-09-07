using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// Class that will handle the Client side Robot connection business logic
    /// </summary>
    public class ClientRobotConnectionController : JsonRPCConnection
    {
        /// <summary>
        /// Default Client Robot Connection controller, using LanguageServerRobot's application
        /// Standard Input/Output Streams.
        /// </summary>
        public ClientRobotConnectionController() : base(new MessageConnection())
        {
        }
        /// <summary>
        /// Message Connection constructor
        /// </summary>
        /// <param name="messageConnection">The Message Connection with the source client.</param>
        public ClientRobotConnectionController(IMessageConnection messageConnection) : base(messageConnection)
        {

        }
    }
}
