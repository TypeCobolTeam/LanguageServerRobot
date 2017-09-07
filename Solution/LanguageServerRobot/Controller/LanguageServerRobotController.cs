using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// This class represents the Controller of the LanguageServerRobot business rules.
    /// </summary>
    public class LanguageServerRobotController
    {
        /// <summary>
        /// Language Server Robot Controller Connection mode
        /// </summary>
        public enum ConnectionMode
        {
            Client,
            ClientServer,
        }

        /// <summary>
        /// The Connection mode
        /// </summary>
        public ConnectionMode Mode
        {
            get;
            private set;
        }
        /// <summary>
        /// The Connection with the Client if the Connection mode is ClientServer, null otherwise.
        /// </summary>
        public ClientRobotConnectionController ClientConnection
        {
            get;
            private set;
        }

        /// <summary>
        /// The Connection with the Server
        /// </summary>
        public ServerRobotConnectionController ServerConnection
        {
            get;
            private set;
        }
    }
}
