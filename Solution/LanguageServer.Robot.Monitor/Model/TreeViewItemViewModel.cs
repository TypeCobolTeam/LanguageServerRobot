using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// This Model is used to represent Nodes in a TreeView Control.
    /// It support Lazy Loading of children.
    /// </summary>
    public class TreeViewItemViewModel : INotifyPropertyChanged
    {
        private static readonly TreeViewItemViewModel dummyItem = new TreeViewItemViewModel();
        private readonly ObservableCollection<TreeViewItemViewModel> children;
        private readonly TreeViewItemViewModel parent;
        private bool isExpanded;
        private bool isSelected;
        /// <summary>
        /// Event's when children are loaded in a Lazy Load Mode.
        /// </summary>
        private event EventHandler ChildrenLoadedEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Parent</param>
        /// <param name="lazyLoadChildren">Lazy loading ?</param>
        public TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            this.parent = parent;
            this.children = new ObservableCollection<TreeViewItemViewModel>();
            if (lazyLoadChildren)
                this.children.Add(dummyItem);
            //Connect property change propagation to the parent.
            if (parent != null)
                PropertyChanged += parent.PropertyChanged;
        }

        /// <summary>
        /// Empty Constructor
        /// </summary>
        private TreeViewItemViewModel()
        {
        }

        /// <summary>
        /// Have Children been loaded ?
        /// </summary>
        public bool HasNotBeenCompleted
        {
            get { return this.children.Count == 1 && this.children.First() == dummyItem; }
        }

        public const string NamePropertyName = "Name";
        String m_name;
        /// <summary>
        /// The Name
        /// </summary>        
        public virtual String Name
        {
            get
            {
                return m_name;
            }
            set
            {
                if (value != m_name)
                {
                    m_name = value;
                    OnPropertyChanged(NamePropertyName);
                }
            }
        }

        /// <summary>
        /// IsExpanded property name
        /// </summary>
        public const string IsExpandedPropertyName = "IsExpanded";

        /// <summary>
        /// Expansion behavior.
        /// </summary>
        public bool IsExpanded
        {
            get { return this.isExpanded; }
            set
            {
                if (value != this.isExpanded)
                {
                    this.isExpanded = value;
                    OnPropertyChanged(IsExpandedPropertyName);
                }

                if (this.isExpanded && this.parent != null)
                    parent.isExpanded = true;

                CompleteChildren();
            }
        }

        /// <summary>
        /// Children Completion
        /// </summary>
        public void CompleteChildren()
        {
            if (this.HasNotBeenCompleted)
            {
                this.Children.Remove(dummyItem);
                this.LoadChildren();
                if (ChildrenLoadedEvent != null)
                {
                    ChildrenLoadedEvent(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// IsSelected property name
        /// </summary>
        public const string IsSelectedPropertyName = "IsSelected";
        /// <summary>
        /// Selection Behavior.
        /// </summary>
        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                if (value != this.isSelected)
                {
                    this.isSelected = value;

                    CompleteChildren();

                    OnPropertyChanged(IsSelectedPropertyName);
                }
            }
        }

        public const String IsCheckedPropertyName = "IsChecked";
        private bool m_IsChecked = false;
        /// <summary>
        /// Is This Item Checked.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return m_IsChecked;
            }
            set
            {
                if (value != m_IsChecked)
                {
                    m_IsChecked = value;
                    OnPropertyChanged(IsCheckedPropertyName);
                }
            }
        }

        public const String IsValidProperty = "IsValid";
        private bool m_IsValid = true;
        /// <summary>
        /// Is Item Valid.
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                return m_IsValid;
            }
            set
            {
                if (m_IsValid != value)
                {
                    m_IsValid = value;
                    OnPropertyChanged(IsValidProperty);
                    OnPropertyChanged(ForeGroundProperty);
                    OnPropertyChanged(FontWeightProperty);
                }
            }
        }

        /// <summary>
        /// Foreground property.
        /// </summary>
        public const String ForeGroundProperty = "ForeGround";
        public Brush ForeGround => IsValid ? (IsCurrent ? Brushes.DarkGreen : Brushes.Black) : Brushes.Red;

        /// <summary>
        /// FontWeight property.
        /// </summary>
        public const String FontWeightProperty = "FontWeight";
        public FontWeight FontWeight => IsCurrent && IsValid ? FontWeights.Bold : FontWeights.Normal;

        public FontStyle FontStyle => FontStyles.Normal;
        public const String IsCurrentProperty = "IsCurrent";
        private bool m_IsCurrent = false;
        /// <summary>
        /// Is Item The current item.
        /// </summary>
        public virtual bool IsCurrent
        {
            get
            {
                return m_IsCurrent;
            }
            set
            {
                if (m_IsCurrent != value)
                {
                    m_IsCurrent = value;
                    OnPropertyChanged(IsCurrentProperty);
                    OnPropertyChanged(ForeGroundProperty);
                    OnPropertyChanged(FontWeightProperty);
                }
            }
        }

        /// <summary>
        /// Look for a direct parent of the given type.
        /// </summary>
        /// <param name="type">The parent typ</param>
        /// <returns>The direct parent of the given type if any, null otherwise</returns>
        public TreeViewItemViewModel GetParent(Type type)
        {
            TreeViewItemViewModel parent = Parent;
            while (parent != null && parent.GetType() != type)
                parent = parent.parent;
            return parent;
        }

        /// <summary>
        /// Parent
        /// </summary>
        public TreeViewItemViewModel Parent => parent;

        /// <summary>
        /// Children
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children => children;

        /// <summary>
        /// Load Children
        /// </summary>
        protected virtual void LoadChildren()
        {

        }

        /// <summary>
        /// Accept method for a Visitor Design Pattern.
        /// </summary>
        /// <param name="visitor">The Visitor to Accept</param>
        public virtual void Accept(ITreeViewItemViewModelVisitor visitor)
        {
            visitor.Visit(this);
            //Accept the visitor by all Children
            foreach (var child in Children)
            {
                child.Accept(visitor);
            }
        }

        /// <summary>
        /// The Delegate signature
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true if the item is accepted, false otherwise. If the item is not accepted the Visit continue.</returns>
        public delegate bool Acceptor(TreeViewItemViewModel item);
        /// <summary>
        /// Visit all Children of the Given type.
        /// </summary>
        /// <param name="type">The type of item to visit</param>
        /// <param name="acceptor">The acceptor on visitation</param>
        /// <returns>false if an item was not accepted.</returns>
        public bool AcceptorVisitor(Type type, Acceptor acceptor)
        {
            if (Children == null)
                return true;
            foreach (var child in Children)
            {
                if (child.GetType() == type)
                {
                    if (acceptor(child))
                    {
                        if (!child.AcceptorVisitor(type, acceptor))
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Raise a Property Change event
        /// </summary>
        /// <param name="propertyName">Property's name</param>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            var eventArgs = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, eventArgs);
        }

        /// <summary>
        /// Raise a Property Change event
        /// </summary>
        /// <param name="propertyName">Property's name</param>
        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged == null)
            {
                return;
            }
            PropertyChanged(sender, e);
        }


        /// <summary>
        /// All properties changed notification
        /// </summary>
        public void NotifyToRefreshAllProperties()
        {
            OnPropertyChanged(String.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
