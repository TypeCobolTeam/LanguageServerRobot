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
        public SettingsController() : base(GenericDialogButton.OKCancel, new SettingsView())
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
            if (sender == base.View.OK)
            {
                if (!ValidateView())
                {
                    return;
                }
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

            return true;
        }

        public override GenericDialogResult Show(string title = null)
        {            
            if (base.Show() == GenericDialogResult.Ok)
            {
                Model.WriteToSettings();
            }
            return Result;
        }
    }
}
