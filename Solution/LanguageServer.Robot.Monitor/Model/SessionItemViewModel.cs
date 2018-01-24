using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.Robot.Common.Model;

namespace LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// Session Item Data View Model
    /// </summary>
    public class SessionItemViewModel : TreeViewDataViewModel<Session>
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="Session">The associated session</param>
        public SessionItemViewModel(Session session) : base(session, false)
        {
        }

        /// <summary>
        /// Accept method for a Visitor Design Pattern.
        /// </summary>
        /// <param name="visitor">The Visitor to Accept</param>
        public virtual void Accept(SessionExplorerItemViewModelVisitor visitor)
        {
            visitor.VisitSessionItemViewModel(this);
            visitor.VisitChildren(Children);
        }
    }
}
