using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.Robot.Common.Model;

namespace LanguageServer.Robot.Monitor.Model
{
    public class ScenarioItemViewModel : TreeViewDataViewModel<Common.Model.Script>
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="Session">The associated script</param>
        public ScenarioItemViewModel(Script script) : base(script, false)
        {
        }

        /// <summary>
        /// Accept method for a Visitor Design Pattern.
        /// </summary>
        /// <param name="visitor">The Visitor to Accept</param>
        public virtual void Accept(SessionExplorerItemViewModelVisitor visitor)
        {
            visitor.VisitScenarioItemViewModel(this);
            visitor.VisitChildren(Children);
        }
    }
}
