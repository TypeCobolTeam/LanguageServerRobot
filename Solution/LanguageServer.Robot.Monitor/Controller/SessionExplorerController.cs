using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageServer.Robot.Common.Model;
using LanguageServer.Robot.Monitor.Model;
using LanguageServer.Robot.Monitor.View;

namespace LanguageServer.Robot.Monitor.Controller
{
    /// <summary>
    /// Controller of a Session Explorer Tree.
    /// </summary>
    public class SessionExplorerController : ICommand
    {
        /// <summary>
        /// One session Constructor
        /// </summary>
        /// <param name="project">The project to explore</param>
        public SessionExplorerController(Session session) 
            : this(new Session[]{session}, new SessionExplorerView())
        {
        }

        /// <summary>
        /// One project Constructor
        /// </summary>
        /// <param name="ctx">Database context</param>
        /// <param name="project">The project to explore</param>
        /// <param name="view">The project explorer view</param>
        public SessionExplorerController(Session session, SessionExplorerView view)
            : this(new Session[] { session }, view)
        {
        }

        /// <summary>
        /// Multi sessions Constructor
        /// </summary>
        /// <param name="sessions">All sessions to explore</param>
        public SessionExplorerController(Session[] sessions)
            : this(sessions, new SessionExplorerView())
        {
        }

        /// <summary>
        /// Multi sessions Constructor
        /// </summary>
        /// <param name="sessions">All sessions to explore</param>
        /// <param name="view">The session explorer view</param>
        public SessionExplorerController(Session[] sessions, SessionExplorerView view)
        {
            Model = new SessionExplorerModel(sessions);
            View = view;
            BindViewModel();
        }

        /// <summary>
        /// The Session Explorer Model.
        /// </summary>
        public SessionExplorerModel Model
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Project Explorer View
        /// </summary>
        public SessionExplorerView View
        {
            get;
            internal set;
        }

        #region Events
        /// <summary>
        /// Event when an element its IsSelected property changed.
        /// the Sender can be SessionItemViewModel, DocumentItemViewModel,
        /// ScenarioItemViewModel, etc ...
        /// </summary>
        public event EventHandler ElementIsSelectedPropertyChanged;

        /// <summary>
        /// Handler from a Proprty Changed Event from de Model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnModelPropertyChangeEvent(object sender, PropertyChangedEventArgs args)
        {
            if (sender is TreeViewItemViewModel && args.PropertyName.Equals(TreeViewItemViewModel.IsSelectedPropertyName))
            {
                if (ElementIsSelectedPropertyChanged != null)
                    ElementIsSelectedPropertyChanged(sender, EventArgs.Empty);
            }
        }

        #endregion

        /// <summary>
        /// Connect the View.
        /// </summary>
        protected void ConnectView()
        {
        }

        /// <summary>
        /// Bind the View and the Model
        /// </summary>
        protected void BindViewModel()
        {
            View.DataContext = Model;
            Model.PropertyChanged += OnModelPropertyChangeEvent;
        }

        /// <summary>
        /// UnBind the View and the Modle
        /// </summary>
        protected void UnbindViewModel()
        {
            View.DataContext = null;
            Model.PropertyChanged -= OnModelPropertyChangeEvent;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
