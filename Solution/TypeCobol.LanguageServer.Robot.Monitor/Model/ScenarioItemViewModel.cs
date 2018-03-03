using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TypeCobol.LanguageServer.Robot.Common.Model;

namespace TypeCobol.LanguageServer.Robot.Monitor.Model
{
    public class ScenarioItemViewModel : TreeViewDataViewModel<Common.Model.Script>
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="script">The associated script</param>
        /// <param name="document">The parent document</param>
        public ScenarioItemViewModel(Script script, DocumentItemViewModel document) : base(script, document, false)
        {
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="script">The associated script</param>
        public ScenarioItemViewModel(Script script) : base(script, false)
        {
        }

        /// <summary>
        /// The Scenario Name.
        /// </summary>
        public String ScenarioName
        {
            get
            {
                return Data.name;
            }
            set
            {
                Data.name = value;
            }
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
