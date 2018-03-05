using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TypeCobol.LanguageServer.Robot.Common.Model;

namespace TypeCobol.LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// Document Item Data View Model, a document is seen as a script model.
    /// </summary>
    public class DocumentItemViewModel : TreeViewDataViewModel<Common.Model.Script>
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="script">The associated script</param>
        /// <param name="Session">The associated parent session</param>
        public DocumentItemViewModel(Script script, SessionItemViewModel session) : base(script, session, false)
        {
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="Session">The associated script</param>
        public DocumentItemViewModel(Script script) : base(script, false)
        {            
        }

        /// <summary>
        /// The Document Name.
        /// </summary>
        public String DocumentName
        {
            get
            {
                return Data.name;
            }
            set
            {
                Data.name = value;
            }
        }

        /// <summary>
        /// Foreground property.
        /// </summary>
        public const String IsRecordingProperty = "IsRecording";            
        private bool m_IsRecording = false;
        /// <summary>
        /// Is this Item in recording mode.
        /// </summary>
        public virtual bool IsRecording
        {
            get
            {
                return m_IsRecording;
            }
            set
            {
                if (m_IsRecording != value)
                {
                    m_IsRecording = value;
                    OnPropertyChanged(IsRecordingProperty);
                }
            }
        }

        public ICommand StartScenarioCommand { get; set; }

        public ICommand StopScenarioCommand { get; set; }

        public ICommand CreateSnapshotCommand { get; set; }

        public ICommand CreateLastSaveSnapshotCommand { get; set; }

        /// <summary>
        /// Add a scenario to this document.
        /// </summary>
        /// <param name="script">The Scenario's cript</param>
        public void AddScenario(Script script)
        {
            if (script != null)
            {
                ScenarioItemViewModel scenario = new ScenarioItemViewModel(script, this);
                Children.Add(scenario);
            }
        }

        /// <summary>
        /// Accept method for a Visitor Design Pattern.
        /// </summary>
        /// <param name="visitor">The Visitor to Accept</param>
        public virtual void Accept(SessionExplorerItemViewModelVisitor visitor)
        {
            visitor.VisitDocumentItemViewModel(this);
            visitor.VisitChildren(Children);
        }
    }
}
