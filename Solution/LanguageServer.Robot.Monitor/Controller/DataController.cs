using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LanguageServer.Robot.Monitor.Model;
using LanguageServer.Robot.Monitor.View;

namespace LanguageServer.Robot.Monitor.Controller
{
    /// <summary>
    /// Abstract Base class of an Entity Controller
    /// </summary>
    public abstract class DataController<D> : System.Windows.Input.ICommand, INotifyPropertyChanged where D : class
    {

        // A color for the default values in view
        protected readonly System.Windows.Media.SolidColorBrush IsDefaultValueColor = System.Windows.Media.Brushes.LightGray;
        protected readonly System.Windows.Media.SolidColorBrush ResetColor = System.Windows.Media.Brushes.Black;

        /// <summary>
        /// Model Constructor
        /// </summary>
        /// <param name="model">The Model</param>
        public DataController(DataModel<D> model) : this(model, new Label())
        {
        }

        /// <summary>
        /// The Model/View Constructor
        /// </summary>
        /// <param name="model">The Model</param>
        /// <param name="view">The Detail View of the Entity</param>
        public DataController(DataModel<D> model, ContentControl view_detail)
        {
            ViewDetail = view_detail;
            View = new DataView();
            View.AttributesPanel.Children.Add(view_detail);
            Model = model;
        }

        /// <summary>
        /// Get the title of this controller.
        /// </summary>
        public abstract String Title
        {
            get;
        }

        DataModel<D> m_Model;
        /// <summary>
        /// The Entity Model
        /// </summary>
        public DataModel<D> Model
        {
            get
            {
                return m_Model;
            }
            set
            {
                //---------------------------------------------------
                //I do this even if it is the same model that is reset
                //So this can be used to reset value when Cancelling.
                //---------------------------------------------------
                //Unbind any previews model
                UnbindViewModel();
                m_Model = value;
                //Reset the view
                SetupView();
                //Bind the View and the Model
                BindViewModel();
                //Notify property Changed
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The Entity's View.
        /// </summary>
        public ContentControl ViewDetail
        {
            get;
            set;
        }

        /// <summary>
        /// The Entity View.
        /// </summary>
        public DataView View
        {
            get;
            set;
        }

        /// <summary>
        /// Handler When a property has changed from the Model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ModelPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
            OnPropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// Method to set text color for default values
        /// </summary>
        public virtual void SetDefaultColors()
        {
            if (IsEditing)
            {
                if (Model.Name.Length < 1)
                    View.NameText.Foreground = IsDefaultValueColor;
                else
                    View.NameText.Foreground = ResetColor;
            }
        }

        /// <summary>
        /// Method to reset text color in the View
        /// </summary>
        public virtual void ResetDefaultColors()
        {
            View.NameText.Foreground = ResetColor;
        }

        /// <summary>
        /// Set Up only the Base View
        /// </summary>
        protected virtual void SetupView()
        {
            //Start Name editing
            View.NameText.IsReadOnly = true;
            View.ButtonsPanel.Visibility = Visibility.Collapsed;

            //This Controller does not use Apply but only Save.
            if (View.ButtonsPanel.Children.Contains(View.Apply))
            {
                View.ButtonsPanel.Children.Remove(View.Apply);
            }
        }

        /// <summary>
        /// Bind the View and the Model.
        /// </summary>
        protected virtual void BindViewModel()
        {
            View.DataContext = Model;
            View.Cancel.Command = this;
            View.Cancel.CommandParameter = View.Cancel;

            View.Apply.Command = this;
            View.Apply.CommandParameter = View.Apply;

            View.Save.Command = this;
            View.Save.CommandParameter = View.Save;

            //Connect Property Change event.
            if (Model != null)
                Model.PropertyChanged += ModelPropertyChangedEventHandler;
        }

        /// <summary>
        /// UnBind the View and the Model.
        /// </summary>
        protected virtual void UnbindViewModel()
        {
            View.DataContext = null;
            View.Cancel.Command = null;
            View.Cancel.CommandParameter = null;

            View.Apply.Command = null;
            View.Apply.CommandParameter = null;

            View.Save.Command = null;
            View.Save.CommandParameter = null;

            if (Model != null)
                Model.PropertyChanged -= ModelPropertyChangedEventHandler;
        }

        /// <summary>
        /// Are we in edition mode ?
        /// </summary>
        public bool IsEditing
        {
            get
            {
                return !View.NameText.IsReadOnly;
            }
        }

        /// <summary>
        /// Enter in edition mode
        /// </summary>
        public virtual void Edit()
        {
            if (!IsEditing)
            {
                View.NameText.IsReadOnly = false;
                View.ButtonsPanel.Visibility = Visibility.Visible;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Execute the Apply Action
        /// </summary>
        /// <returns>true if OK, false otherwise</returns>
        public virtual bool Apply()
        {
            String Reason = "";
            //The Name cannot be Empty
            if (!Model.IsValid(ref Reason))
            {
                MessageBox.Show(Reason, Title);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Execute the Save Action : Apply + Save
        /// </summary>
        /// <exception cref="">Any database exception if an error occurs.</exception>
        /// <returns>true if apply is OK, false otherwise</returns>
        public virtual bool Save()
        {
            return Apply();
        }

        /// <summary>
        /// Execute the Cancel Action
        /// </summary>
        public virtual void Cancel()
        {
            //End Name editing
            View.NameText.IsReadOnly = true;
            View.ButtonsPanel.Visibility = Visibility.Collapsed;
            ResetDefaultColors();
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Fire the can execute Changed Event.
        /// </summary>
        protected void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        #region ICommand Implementation
        public virtual bool CanExecute(object parameter)
        {
            if (Model == null)
                return false;
            if (parameter == View.Apply || parameter == View.Save)
            {
                if (Model.IsNew)
                {
                    SetDefaultColors();
                }
                String reason = "";
                if (!Model.IsValid(ref reason))
                {
                    View.TextNotValidDesc.Text = reason;
                    View.TextNotValidDesc.Visibility = System.Windows.Visibility.Visible;
                    return false;
                }
                else
                {
                    View.TextNotValidDesc.Text = reason;
                    View.TextNotValidDesc.Visibility = System.Windows.Visibility.Collapsed;
                    return parameter == View.Apply
                        ? Model.IsModified && IsEditing
                        : (Model.IsModified && IsEditing) || Model.NeedToBeSaved;
                }
            }
            if (parameter == View.Cancel)
                return IsEditing;
            return false;
        }

        public event EventHandler CanExecuteChanged;

        public virtual void Execute(object parameter)
        {
            if (parameter == View.Apply)
            {
                try
                {
                    if (Apply())
                    {
                        if (ApplyEvent != null)
                            ApplyEvent(this, EventArgs.Empty);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format(Properties.Resources.FailToSaveData, Title, Model.Name),
                        Title);
                }
            }
            else if (parameter == View.Save)
            {
                try
                {
                    if (Save())
                    {
                        if (SaveEvent != null)
                            SaveEvent(this, EventArgs.Empty);
                    }
                }
                catch (Exception e)
                {
                    String msg = e.Message;
                    MessageBox.Show(String.Format(Properties.Resources.FailToSaveData, Title, Model.Name),
                        Title);
                }
            }

            else if (parameter == View.Cancel)
            {
                Cancel();
                if (CancelEvent != null)
                    CancelEvent(this, EventArgs.Empty);
            }
        }
        #endregion

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

        /// <summary>
        /// Raise the Apply Event.
        /// </summary>
        protected void RaiseApplyEvent()
        {
            if (ApplyEvent != null)
                ApplyEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raise the save Event.
        /// </summary>
        protected void RaiseSaveEvent()
        {
            if (SaveEvent != null)
                SaveEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raise the Cancel Event.
        /// </summary>
        protected void RaiseCancelEvent()
        {
            if (CancelEvent != null)
                CancelEvent(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Event When Cancel has been successfully performed.
        /// </summary>
        public event EventHandler CancelEvent;
        /// <summary>
        /// Event When Apply has been successfully performed.
        /// </summary>
        public event EventHandler ApplyEvent;
        /// <summary>
        /// Event When Save has been successfully performed.
        /// </summary>
        public event EventHandler SaveEvent;
    }
}
