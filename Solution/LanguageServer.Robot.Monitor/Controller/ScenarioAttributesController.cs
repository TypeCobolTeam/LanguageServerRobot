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
            //Call our Setup View
            SetupView();
        }

        /// <summary>
        /// Model View Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="view_detail"></param>
        public ScenarioAttributesController(ScenarioAttributesModel model, ScenarioAttributesView viewDetail) : base(model, viewDetail)
        {         
            //Call our Setup View
            SetupView();
        }

        public override string Title
        {
            get { return Properties.Resources.ScenarioEditingTitle; }            
        }

        /// <summary>
        /// Set Up only the Base View
        /// </summary>
        protected override void SetupView()
        {
            base.SetupView();
            EnableControllersView(false);
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

        /// <summary>
        /// Enable Controlleers view.
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableControllersView(bool enabled)
        {
            ViewDetail.Description.IsReadOnly = !enabled;
        }

        /// <summary>
        /// Enter in edition mode
        /// </summary>
        public override void Edit()
        {
            EnableControllersView(true);
            base.Edit();
        }

        /// <summary>
        /// Execute the Apply Action
        /// </summary>        
        /// <returns>true if OK, false otherwise</returns>
        public override bool Apply()
        {
            if (!base.Apply())
                return false;
            if (Model.Apply())
            {
                //Sorry Cancel here.
                Cancel();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Execute the Cancel Action
        /// </summary>
        public override void Cancel()
        {
            if (Model.IsModified || (Model.ValidationFlags != 0))
            {
                Model.Cancel();
                //Reset our own Model.
                base.Model = Model;
            }
            EnableControllersView(false);
            base.Cancel();
        }

        /// <summary>
        /// Execute the Save Action : Apply + Save
        /// </summary>
        /// <exception cref="">Any database exception if an error occurs.</exception>
        /// <returns>true if apply is OK, false otherwise</returns>
        public override bool Save()
        {
            if (base.Save())
            {
                return Model.Save(false);
            }
            return false;
        }

    }
}
