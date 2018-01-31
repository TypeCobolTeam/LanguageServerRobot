using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.Robot.Common.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class MonitorLanguageServerRobotController : LanguageServerRobotController
    {
        /// <summary>
        /// Constructor for the LanguageServerRobot running as ClientScenarion/Server recording mode.
        /// </summary>
        /// <param name="serverConnection">The target server</param>
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        public MonitorLanguageServerRobotController(ServerRobotConnectionController serverConnection, string scriptRepositoryPath = null) :
            base(serverConnection, scriptRepositoryPath)
        {
        }

        public override void FromServer(string message)
        {
            ServerConnection.FromServer(message);
            //Don't forward again to our client.
            //ClientConnection.SendMessage(message);
        }
    }
}
