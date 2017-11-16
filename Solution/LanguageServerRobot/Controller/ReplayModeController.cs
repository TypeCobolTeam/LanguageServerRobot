using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// Robot Replay mode controller
    /// </summary>
    public class ReplayModeController : AbstractModeController
    {
        /// <summary>
        /// Empty constructor.
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        /// </summary>
        public ReplayModeController(string scriptRepositoryPath = null) : base(scriptRepositoryPath)
        {
            State = ModeState.NotInitialized;
        }

        public override void FromClient(string message)
        {
            throw new NotImplementedException();
        }

        public override void FromServer(string message)
        {
            throw new NotImplementedException();
        }
    }
}
