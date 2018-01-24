using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// A Tree Viev ITem View Model for Generic Data Type
    /// D is the data type
    /// </summary>
    public class TreeViewDataViewModel<D> : TreeViewItemViewModel where D : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The associated data</param>
        /// <param name="parent">The parent</param>
        /// <param name="lazyLoadChildren">Lazy Tree Mode ?</param>
        public TreeViewDataViewModel(D data, bool lazyLoadChildren)
            : this(data, null, lazyLoadChildren)
        {
        }

        /// <summary>
        /// Costructor
        /// </summary>
        /// <param name="ctx">Database Context</param>
        /// <param name="data">The underlying data</param>
        /// <param name="parent">The parent</param>
        /// <param name="lazyLoadChildren">Lazy Tree Mode ?</param>
        public TreeViewDataViewModel(D data, TreeViewItemViewModel parent, bool lazyLoadChildren) 
            : base(parent, lazyLoadChildren)
        {
            Data = data;
        }

        /// <summary>
        /// The associated data.
        /// </summary>
        public D Data
        {
            get;
            set;
        }

        public const String ModelProperty = "ModelProperty";
        DataModel<D> m_Model;
        /// <summary>
        /// The underlying Data Model
        /// </summary>
        public DataModel<D> Model
        {
            get
            {
                return m_Model;
            }
            set
            {
                if (m_Model != value)
                {
                    if (m_Model != null)
                        m_Model.PropertyChanged -= ModelPropertyChangedEventHandler;
                    m_Model = value;
                    if (m_Model != null)
                        m_Model.PropertyChanged += ModelPropertyChangedEventHandler;
                    OnPropertyChanged(ModelProperty);
                }
            }
        }

        /// <summary>
        /// Property Changed Event Handler on The Model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ModelPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender, e);
        }

    }
}
