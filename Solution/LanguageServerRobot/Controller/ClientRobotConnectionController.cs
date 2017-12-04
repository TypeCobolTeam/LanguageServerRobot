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
    public class ClientRobotConnectionController : JsonRPCConnection, IRobotModeController
    {        
        /// <summary>
        /// Default Client Robot Connection controller, using LanguageServerRobot's application
        /// Standard Input/Output Streams.
        /// </summary>       
        public ClientRobotConnectionController() : this(new MessageConnection())
        {                        
        }
        /// <summary>
        /// Message Connection constructor
        /// </summary>
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        /// <param name="messageConnection">The Message Connection with the source client.</param>
        public ClientRobotConnectionController(IMessageConnection messageConnection) : base(messageConnection)
        {            
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
        /// Handling a message that comes from the Client: From Me.
        /// </summary>
        /// <param name="message"></param>
        public virtual void FromClient(string message)
        {
            System.Diagnostics.Contracts.Contract.Requires(RobotModeController != null);
            RobotModeController.FromClient(message);
        }

        /// <summary>
        /// handling a message that comes from the server.
        /// </summary>
        /// <param name="message"></param>
        public virtual void FromServer(string message)
        {
            //Do nothing let the server controller do its logic.
            throw new NotImplementedException();
        }
    }
}
