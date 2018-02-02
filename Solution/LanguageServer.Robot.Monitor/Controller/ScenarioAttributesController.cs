using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LanguageServer.Robot.Common.Model;
using LanguageServer.Robot.Monitor.Model;
using LanguageServer.Robot.Monitor.View;

namespace LanguageServer.Robot.Monitor.Controller
{
    /// <summary>
    /// A Controller for Scenario Editing.
    /// </summary>
    public class ScenarioAttributesController : DataController<Script>
    {
        /// <summary>
        /// Model constructor
        /// </summary>
        /// <param name="model"></param>
        public ScenarioAttributesController(ScenarioAttributesModel model) : base(model)
        {
        }

        /// <summary>
        /// Model View Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="view_detail"></param>
        public ScenarioAttributesController(ScenarioAttributesModel model, ScenarioAttributesView viewDetail) : base(model, viewDetail)
        {
        }

        public override string Title
        {
            get { return Properties.Resources.ScenarioEditingTitle; }            
        }

        /// <summary>
        /// Bind the View and the Model.
        /// </summary>
        protected override void BindViewModel()
        {
            base.BindViewModel();
            ViewDetail.DataContext = Model;
        }

        /// <summary>
        /// UnBind the View and the Model.
        /// </summary>
        protected override void UnbindViewModel()
        {
            base.UnbindViewModel();
            ViewDetail.DataContext = null;
        }

        /// <summary>
        /// The Detail View
        /// </summary>
        public new ScenarioAttributesView ViewDetail
        {
            get
            {
                return base.ViewDetail as ScenarioAttributesView;
            }
        }

        /// <summary>
        /// Our Model
        /// </summary>
        public new ScenarioAttributesModel Model
        {
            get
            {
                return base.Model as ScenarioAttributesModel;
            }
        }

    }
}
