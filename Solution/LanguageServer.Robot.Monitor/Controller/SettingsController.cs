using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LanguageServer.Robot.Monitor.Model;
using LanguageServer.Robot.Monitor.View;

namespace LanguageServer.Robot.Monitor.Controller
{
    /// <summary>
    /// Settings Dialog controller
    /// </summary>
    public class SettingsController : GenericDialogController
    {
        /// <summary>
        /// Cosntructor.
        /// </summary>
        public SettingsController() : base(GenericDialogButton.YesNoCancel, new SettingsView())
        {
            Model = new SettingsModel();
            View = (SettingsView) base.UserControl;
            BindViewModel();
        }

        /// <summary>
        /// The Model
        /// </summary>
        public SettingsModel Model
        {
            get; set;
        }

        public SettingsView View
        {
            get;
            private set;
        }

        /// <summary>
        /// Bind the View and the Model.
        /// </summary>
        private void BindViewModel()
        {
            View.DataContext = Model;
            base.View.Yes.Content = "OK";
            base.View.No.Content = "Reset";            
        }

        /// <summary>
        /// UnBind the View and the Model.
        /// </summary>
        private void unBindViewModel()
        {
            View.DataContext = null;
        }

        protected override void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == base.View.Yes)
            {
                if (!ValidateView())
                {
                    return;
                }
            }
            if (sender == base.View.No)
            {
                Properties.Settings.Default.Reset();
                Model.ReadFromSettings();
                View.DataContext = null;
                View.DataContext = Model;
                return;
            }
            base.Button_Click(sender, e);
        }

        private bool ValidateView()
        {
            DirectoryInfo di = new DirectoryInfo(View.ServerPath.Text);
            if (!di.Exists)
            {
                MessageBox.Show(MyWindow, string.Format(Properties.Resources.InvalidServerPath, View.ServerPath.Text),
                    Properties.Resources.LSRMName, MessageBoxButton.OK, MessageBoxImage.Hand);
                return false;
            }
            di = new DirectoryInfo(View.ServerPath.Text);
            if (!di.Exists)
            {
                MessageBox.Show(MyWindow, string.Format(Properties.Resources.InvalidLSRPath, View.LSRPath.Text),
                    Properties.Resources.LSRMName, MessageBoxButton.OK, MessageBoxImage.Hand);
                return false;
            }
            di = new DirectoryInfo(View.ScriptRepository.Text);
            if (!di.Exists)
            {
                MessageBox.Show(MyWindow, string.Format(Properties.Resources.InvalidScriptRepositoryPath, View.ScriptRepository.Text),
                    Properties.Resources.LSRMName, MessageBoxButton.OK, MessageBoxImage.Hand);
                return false;
            }
            Model.ServerPath = View.ServerPath.Text;
            Model.LSRPath = View.LSRPath.Text;
            Model.ScriptRepositoryPath = View.ScriptRepository.Text;
            Model.LSRReplayArguments = View.LSRReplayArguments.Text;
            Model.BatchTemplate = View.BatchTemplate.Text;
            return true;
        }

        public override GenericDialogResult Show(string title = null)
        {            
            if (base.Show() == GenericDialogResult.Yes)
            {
                Model.WriteToSettings();
            }
            return Result;
        }
    }
}
