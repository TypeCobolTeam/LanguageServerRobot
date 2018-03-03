using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeCobol.LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// A Checked Data Item
    /// </summary>
    /// <typeparam name="D">Generic Data Type</typeparam>
    public class CheckedDataItem<D> : CheckedListItem where D : class
    {
        /// <summary>
        /// The Model for the underlying data.
        /// </summary>
        public DataModel<D> Data
        {
            get;
            set;
        }
    }
}
