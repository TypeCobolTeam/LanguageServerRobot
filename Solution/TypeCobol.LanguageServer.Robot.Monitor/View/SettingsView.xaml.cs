using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FolderBrowserDialog = TypeCobol.LanguageServer.Robot.Monitor.Utilities.Microsoft.Win32.FolderBrowserDialog;
using UserControl = System.Windows.Controls.UserControl;

namespace TypeCobol.LanguageServer.Robot.Monitor.View
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void BrowseServerPath_OnClick(object sender, RoutedEventArgs e)
        {
            //FolderBrowserDialog browser = new FolderBrowserDialog();            
            //browser.Title = Properties.Resources.LSRMName;            
            //if (browser.ShowDialog().HasValue)
            //{
            //    ServerPath.Text = browser.SelectedPath;
            //}

            System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog();
            if (browser.ShowDialog() == DialogResult.OK)
            {
                ServerPath.Text = browser.SelectedPath;
            }
        }

        private void BrowseLSRPath_OnClick(object sender, RoutedEventArgs e)
        {
            //FolderBrowserDialog browser = new FolderBrowserDialog();                        
            //browser.Title = Properties.Resources.LSRMName;
            //if (browser.ShowDialog().HasValue)
            //{
            //    LSRPath.Text = browser.SelectedPath;
            //}

            System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog();
            if (browser.ShowDialog() == DialogResult.OK)
            {
                LSRPath.Text = browser.SelectedPath;
            }
        }

        private void BrowseScriptPath_OnClick(object sender, RoutedEventArgs e)
        {
            //FolderBrowserDialog browser = new FolderBrowserDialog();            
            //browser.Title = Properties.Resources.LSRMName;
            //if (browser.ShowDialog().HasValue)
            //{
            //    ScriptRepository.Text = browser.SelectedPath;
            //}

            System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog();
            if (browser.ShowDialog() == DialogResult.OK)
            {
                ScriptRepository.Text = browser.SelectedPath;
            }
        }
    }
}
