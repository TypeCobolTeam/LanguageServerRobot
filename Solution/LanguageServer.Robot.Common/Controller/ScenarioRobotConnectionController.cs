using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;

namespace LanguageServer.Robot.Common.Controller
{
    /// <summary>
    /// A Controller for recording a specific scenario.
    /// </summary>
    public class ScenarioRobotConnectionController : AbstractReplayRobotConnectionController
    {
        /// <summary>
        /// Message Connection constructor.
        /// </summary>
        /// <param name="messageConnection"></param>
        public ScenarioRobotConnectionController(IMessageConnection messageConnection) : base(messageConnection)
        {
        }
    }
}
