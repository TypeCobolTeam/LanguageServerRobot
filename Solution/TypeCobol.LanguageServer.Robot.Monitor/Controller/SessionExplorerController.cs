using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TypeCobol.LanguageServer.Robot.Common.Model;
using TypeCobol.LanguageServer.Robot.Monitor.Model;
using TypeCobol.LanguageServer.Robot.Monitor.View;

namespace TypeCobol.LanguageServer.Robot.Monitor.Controller
{
    /// <summary>
    /// Controller of a Session Explorer Tree.
    /// </summary>
    public class SessionExplorerController : ICommand
    {
        /// <summary>
        /// The Start Scenario command
        /// </summary>
        public class StartScenarioCommand : ICommand
        {
            /// <summary>
            /// The Session Explorer Controller.
            /// </summary>
            public SessionExplorerController Controller
            { get; internal set; }
            public StartScenarioCommand(SessionExplorerController controller)
            {
                this.Controller = controller;
            }
            public bool CanExecute(object parameter)
            {
                if (parameter is DocumentItemViewModel)
                {
                    return (parameter as DocumentItemViewModel).IsCurrent && !(parameter as DocumentItemViewModel).IsRecording;
                }
                return false;
            }

            public void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    Controller.StartScenarioHandler?.Invoke(Controller, parameter as DocumentItemViewModel);
                }
            }

            public void RaiseCanExecuteChanged(object parameter)
            {
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(parameter, EventArgs.Empty);
                }
            }

            public event EventHandler CanExecuteChanged;
        }

        /// <summary>
        /// The Stop Scenario command
        /// </summary>
        public class StopScenarioCommand : ICommand
        {
            /// <summary>
            /// The Session Explorer Controller.
            /// </summary>
            public SessionExplorerController Controller
            { get; internal set; }
            public StopScenarioCommand(SessionExplorerController controller)
            {
                this.Controller = controller;
            }
            public bool CanExecute(object parameter)
            {
                if (parameter is DocumentItemViewModel)
                {
                    return (parameter as DocumentItemViewModel).IsCurrent && (parameter as DocumentItemViewModel).IsRecording;
                }
                return false;
            }

            public void RaiseCanExecuteChanged(object parameter)
            {
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(parameter, EventArgs.Empty);
                }
            }

            public void Execute(object parameter)
            {
                throw new NotImplementedException();
            }

            public event EventHandler CanExecuteChanged;
        }

        /// <summary>
        /// The Create Snapshot command
        /// </summary>
        public class CreateSnapshotCommand : ICommand
        {
            /// <summary>
            /// The Session Explorer Controller.
            /// </summary>
            public SessionExplorerController Controller
            { get; internal set; }
            public bool LastSaveSnapshot { get; internal set; }
            public CreateSnapshotCommand(SessionExplorerController controller, bool bLastSave)
            {
                this.Controller = controller;
                this.LastSaveSnapshot = bLastSave;
            }
            public bool CanExecute(object parameter)
            {
                if (parameter is DocumentItemViewModel)
                {
                    return (parameter as DocumentItemViewModel).IsCurrent && !(parameter as DocumentItemViewModel).IsRecording;
                }
                return false;
            }

            public void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    if (LastSaveSnapshot)
                        Controller.CreateLastSaveSnapshotHandler?.Invoke(Controller, parameter as DocumentItemViewModel);
                    else
                        Controller.CreateSnapshotHandler?.Invoke(Controller, parameter as DocumentItemViewModel);
                }
            }

            public void RaiseCanExecuteChanged(object parameter)
            {
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(parameter, EventArgs.Empty);
                }
            }
            public event EventHandler CanExecuteChanged;
        }

        public StartScenarioCommand StartScenario
        {
            get; internal set;
        }

        public StopScenarioCommand StopScenario
        {
            get; internal set;
        }

        public CreateSnapshotCommand CreateSnapshot
        {
            get; internal set;
        }

        public CreateSnapshotCommand CreateLastSaveSnapshot
        {
            get; internal set;
        }

        /// <summary>
        /// Start Scenario Handler
        /// </summary>
        public event EventHandler<DocumentItemViewModel> StartScenarioHandler;

        /// <summary>
        /// Stop Scenario Handler
        /// </summary>
        public event EventHandler<DocumentItemViewModel> StopScenarioHandler;

        /// <summary>
        /// Create Snapshot Handler
        /// </summary>
        public event EventHandler<DocumentItemViewModel> CreateSnapshotHandler;

        /// <summary>
        /// Create Last Save Snapshot Handler
        /// </summary>
        public event EventHandler<DocumentItemViewModel> CreateLastSaveSnapshotHandler;

        /// <summary>
        /// View Constructor
        /// </summary>
        public SessionExplorerController(SessionExplorerView view)
            : this(new Session[0], view)
        {
        }

        /// <summary>
        /// One project Constructor
        /// </summary>
        /// <param name="project">The project to explore</param>
        /// <param name="view">The session explorer view</param>
        public SessionExplorerController(Session session, SessionExplorerView view)
            : this(new Session[] { session }, view)
        {
        }

        /// <summary>
        /// Multi sessions Constructor
        /// </summary>
        /// <param name="sessions">All sessions to explore</param>
        /// <param name="view">The session explorer view</param>
        public SessionExplorerController(Session[] sessions, SessionExplorerView view)
        {
            StartScenario = new StartScenarioCommand(this);
            StopScenario = new StopScenarioCommand(this);
            CreateSnapshot = new CreateSnapshotCommand(this, false);
            CreateLastSaveSnapshot = new CreateSnapshotCommand(this, true);
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
                ElementIsSelectedPropertyChanged?.Invoke(sender, EventArgs.Empty);
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
        /// Add a New session
        /// </summary>
        /// <param name="session"></param>
        public void AddSession(Session session)
        {
            Model.AddSession(session);
        }

        /// <summary>
        /// Add sessions
        /// </summary>
        /// <param name="sessions">Session enumerator</param>
        public void AddSessions(IEnumerable<Session> sessions)
        {
            Model.AddSessions(sessions);
        }

        /// <summary>
        /// Add the a document script model in to a session model
        /// </summary>
        /// <param name="session">The session whose model will receive the document model</param>
        /// <param name="script">The Document's script from which a document model will be created.</param>
        /// <returns>The created Document Modle if any, null otherwise.</returns>
        public DocumentItemViewModel AddDocument(Session session, Script script)
        {
            SessionItemViewModel sessionModel = Model[session];
            DocumentItemViewModel documentModel = sessionModel?.AddDocument(script);
            if (documentModel != null)
            {   //Connect controller command to the model.
                documentModel.StartScenarioCommand = StartScenario;
                documentModel.StopScenarioCommand = StopScenario;
                documentModel.CreateSnapshotCommand = CreateSnapshot;
                documentModel.CreateLastSaveSnapshotCommand = CreateLastSaveSnapshot;
            }
            return documentModel;
        }

        /// <summary>
        /// The Current Document Item View Model
        /// </summary>
        public DocumentItemViewModel CurrentDocument
        {
            get;
            internal set;
        }

        /// <summary>
        /// Set the active document.
        /// </summary>
        /// <param name="session">The Session to which belong the script</param>
        /// <param name="document">The document's script to set as the active one</param>
        public void SetActiveDocument(Session session, Script document)
        {
            System.Diagnostics.Debug.Assert(session != null);
            System.Diagnostics.Debug.Assert(document != null);

            SessionItemViewModel sessionModel = Model[session];
            DocumentItemViewModel documentModel = sessionModel[document];
            if (CurrentDocument != null && CurrentDocument != documentModel)
                CurrentDocument.IsCurrent = false;
            if (documentModel != null)
            {
                CurrentDocument = documentModel;
                CurrentDocument.IsCurrent = true;
            }
        }

        /// <summary>
        /// Set the document as invalid.
        /// </summary>
        /// <param name="session">The Session to which belong the script</param>
        /// <param name="document">The document's script to set as invalid</param>
        public void SetInvalidDocument(Session session, Script document)
        {
            System.Diagnostics.Debug.Assert(session != null);
            System.Diagnostics.Debug.Assert(document != null);

            SessionItemViewModel sessionModel = Model[session];
            DocumentItemViewModel documentModel = sessionModel[document];
            if (documentModel != null)
            {
                documentModel.IsValid = false;
            }
        }

        /// <summary>
        /// Update Commands State
        /// </summary>
        public void UpdateCommandState()
        {
            StartScenario?.RaiseCanExecuteChanged(this); ;
            StopScenario?.RaiseCanExecuteChanged(this);
            CreateSnapshot?.RaiseCanExecuteChanged(this);
            CreateLastSaveSnapshot?.RaiseCanExecuteChanged(this);
        }

        /// <summary>
        /// Bind the View and the Model
        /// </summary>
        protected void BindViewModel()
        {
            View.DataContext = Model;
            View.ContextMenuOpeningOpeningHandler += View_ContextMenuOpeningOpeningHandler;
            Model.PropertyChanged += OnModelPropertyChangeEvent;
        }
        /// <summary>
        /// Handler when the Contextual menu is about to be shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_ContextMenuOpeningOpeningHandler(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            UpdateCommandState();
        }

        /// <summary>
        /// UnBind the View and the Modle
        /// </summary>
        protected void UnbindViewModel()
        {
            View.DataContext = null;
            View.ContextMenuOpeningOpeningHandler -= View_ContextMenuOpeningOpeningHandler;
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
