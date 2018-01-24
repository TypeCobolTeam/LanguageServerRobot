using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// Base class for a CheckList Item
    /// </summary>
    public class CheckedListItem : System.ComponentModel.INotifyPropertyChanged
    {
        public const String ID_PROPERTY = "Id";
        int m_Id;
        /// <summary>
        /// Any Unique Id
        /// </summary>
        public virtual int Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                if (m_Id != value)
                {
                    m_Id = value;
                    OnPropertyChanged(ID_PROPERTY);
                }
            }
        }

        String m_Name;
        public const String NAME_PROPERTY = "Name";
        /// <summary>
        /// The Name binding
        /// </summary>
        public virtual string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                if (m_Name != value)
                {
                    m_Name = value;
                    OnPropertyChanged(NAME_PROPERTY);
                }
            }
        }

        bool m_IsChecked;
        public const String ISCHECKED_PROPERTY = "IsChecked";
        /// <summary>
        /// Is this Item Checked
        /// </summary>
        public virtual bool IsChecked
        {
            get
            {
                return m_IsChecked;
            }
            set
            {
                if (m_IsChecked != value)
                {
                    m_IsChecked = value;
                    OnPropertyChanged(ISCHECKED_PROPERTY);
                }
            }
        }

        bool m_IsSelected;
        public const String ISSELECTED_PROPERTY = "IsSelected";
        /// <summary>
        /// Is this Item Selected
        /// </summary>
        public virtual bool IsSelected
        {
            get
            {
                return m_IsSelected;
            }
            set
            {
                if (m_IsSelected != value)
                {
                    m_IsSelected = value;
                    OnPropertyChanged(ISSELECTED_PROPERTY);
                }
            }
        }

        bool m_IsItalic;
        public const String ISITALIC_PROPERTY = "IsItalic";
        /// <summary>
        /// Is Text In Italic when displayed ?
        /// </summary>
        public bool IsItalic
        {
            get
            {
                return m_IsItalic;
            }
            set
            {
                if (m_IsItalic != value)
                {
                    m_IsItalic = value;
                    OnPropertyChanged(ISITALIC_PROPERTY);
                }
            }
        }

        bool m_IsBold;
        public const String ISBOLD_PROPERTY = "IsBold";
        /// <summary>
        /// Is Text In Bold when displayed ?
        /// </summary>
        public bool IsBold
        {
            get
            {
                return m_IsBold;
            }
            set
            {
                if (m_IsBold != value)
                {
                    m_IsBold = value;
                    OnPropertyChanged(ISBOLD_PROPERTY);
                }
            }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Raise a Property Change event
        /// </summary>
        /// <param name="propertyName">Property's name</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            var eventArgs = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, eventArgs);
        }

        /// <summary>
        /// All properties changed notification
        /// </summary>
        protected void NotifyToRefreshAllProperties()
        {
            OnPropertyChanged(String.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
