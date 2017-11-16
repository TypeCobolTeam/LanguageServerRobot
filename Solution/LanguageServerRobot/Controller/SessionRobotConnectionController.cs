using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// A Client controller which is able to execute a session
    /// </summary>
    public class SessionRobotConnectionController : ClientRobotConnectionController
    {
        /// <summary>
        /// Session Connection constructor
        /// </summary>
        /// <param name="session">The session to be executed by this Client</param>
        public SessionRobotConnectionController(Model.Session session)
        {
            this.Session = session;
        }

        /// <summary>
        /// The session to be executed by this controller
        /// </summary>
        public Model.Session Session
        {
            get;
            private set;
        }
    }
}
