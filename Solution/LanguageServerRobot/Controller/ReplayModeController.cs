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
        public ReplayModeController(Model.Script script, string scriptRepositoryPath = null) : base(scriptRepositoryPath)
        {
            System.Diagnostics.Debug.Assert(script != null);
            State = ModeState.NotInitialized;
        }

        /// <summary>
        /// The Current message index in the script message.
        /// </summary>
        private long m_ScriptMessageIndex;

        /// <summary>
        /// Current Script Message Index.
        /// </summary>
        internal long ScriptMessageIndex
        {
            get
            {
                return System.Threading.Interlocked.Read(ref m_ScriptMessageIndex);
            }
            set
            {
                System.Threading.Interlocked.Exchange(ref m_ScriptMessageIndex, value);
            }
        }

        /// <summary>
        /// The Script being replayed
        /// </summary>
        public Model.Script Script
        {
            get;
            private set;
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
