using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// A Client controller which is able to execute a script
    /// </summary>
    public class ScriptRobotConnectionController : ClientRobotConnectionController
    {
        /// <summary>
        /// Script Connection constructor
        /// </summary>
        /// <param name="script">The script to be executed by this Client</param>
        public ScriptRobotConnectionController(Model.Script script)
        {
            this.Script = script;
        }

        /// <summary>
        /// The script to be executed by this controller
        /// </summary>
        public Model.Script Script
        {
            get;
            private set;
        }
    }
}
