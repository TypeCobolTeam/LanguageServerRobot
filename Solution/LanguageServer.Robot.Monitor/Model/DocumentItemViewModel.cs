using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.Robot.Common.Model;

namespace LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// Document Item Data View Model, a document is seen as a script model.
    /// </summary>
    public class DocumentItemViewModel : TreeViewDataViewModel<Common.Model.Script>
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="Session">The associated script</param>
        public DocumentItemViewModel(Script script) : base(script, false)
        {            
        }

        /// <summary>
        /// The Document Name.
        /// </summary>
        public String DocumentName
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
            visitor.VisitDocumentItemViewModel(this);
            visitor.VisitChildren(Children);
        }
    }
}
