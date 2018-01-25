using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.Robot.Common.Model;

namespace LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// The Session explorer model.
    /// </summary>
    public class SessionExplorerModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public SessionExplorerModel() : this(new Session[0])
        {
        }

        /// <summary>
        /// Session Constructor
        /// </summary>
        /// <param name="session">The session</param>
        public SessionExplorerModel(Session session) : this(new Session[]{session})
        {
        }

        /// <summary>
        /// Multi-sessions Constructor
        /// </summary>
        public SessionExplorerModel(Session[] sessions)
        {
            m_sessions = new ObservableCollection<SessionItemViewModel>(
                (from session in sessions
                 select new SessionItemViewModel(session))
                .ToList());
            //Connect Childen Property Change Propagation to US.            
            foreach (var session in Sessions)
            {
                session.PropertyChanged += PropagatePropertyChangedEventHandler;
            }
        }

        /// <summary>
        /// Getter on the Project Item Model at the specified index.
        /// </summary>
        public SessionItemViewModel this[int index]
        {
            get
            {
                return Sessions[index];
            }
        }

        /// <summary>
        /// Getter on the Project Item Model at the specified index.
        /// </summary>
        public SessionItemViewModel this[Session session]
        {
            get
            {
                return Sessions != null ? Sessions.FirstOrDefault(S => S.Data == session) : null;
            }
        }

        /// <summary>
        /// Add a New session
        /// </summary>
        /// <param name="session"></param>
        public void AddSession(Session session)
        {
            if (session != null)
            {
                if (Sessions == null)
                {
                    m_sessions = new ObservableCollection<SessionItemViewModel>();
                }
                m_sessions.Add(new SessionItemViewModel(session));
            }
        }

        /// <summary>
        /// In fact all available sessions
        /// </summary>
        private ObservableCollection<SessionItemViewModel> m_sessions;
        /// <summary>
        /// All available sessions
        /// </summary>
        public ObservableCollection<SessionItemViewModel> Sessions
        {
            get { return m_sessions; }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Internal fonction to propagate Property Change Event's from Children.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropagatePropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            PropertyChanged(sender, e);
        }

        /// <summary>
        /// Raise a Property Change event
        /// </summary>
        /// <param name="propertyName">Property's name</param>
        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            var eventArgs = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, eventArgs);
        }
    }
}
