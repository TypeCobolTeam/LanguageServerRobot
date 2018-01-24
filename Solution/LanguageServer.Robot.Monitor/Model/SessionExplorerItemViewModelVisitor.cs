using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// Visitor on Session Explorer Item View Model objects.
    /// </summary>
    public abstract class SessionExplorerItemViewModelVisitor : ITreeViewItemViewModelVisitor
    {
        public abstract void VisitSessionItemViewModel(SessionItemViewModel item);
        public abstract void VisitDocumentItemViewModel(DocumentItemViewModel item);
        public abstract void VisitScenarioItemViewModel(ScenarioItemViewModel item);
        public void Visit(TreeViewItemViewModel item)
        {
            if (item is SessionItemViewModel)
            {
                (item as SessionItemViewModel).Accept(this);
            }
            else if (item is DocumentItemViewModel)
            {
                (item as DocumentItemViewModel).Accept(this);
            }
            else if (item is ScenarioItemViewModel)
            {
                (item as ScenarioItemViewModel).Accept(this);
            }
        }
        internal void VisitChildren(IEnumerable<TreeViewItemViewModel> children)
        {
            foreach (TreeViewItemViewModel child in children)
            {
                Visit(child);
            }
        }
    }
}
