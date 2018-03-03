using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeCobol.LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// Interface of a Visitor Pattern on a TreeViewItemViewModel.
    /// </summary>
    public interface ITreeViewItemViewModelVisitor
    {
        /// <summary>
        /// General Visit method.
        /// </summary>
        /// <param name="item"></param>
        void Visit(TreeViewItemViewModel item);
    }
}
