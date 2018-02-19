using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LanguageServer.Robot.Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.Current.MainWindow = this;
            ConnectCommands();
            (App.Current as LanguageServer.Robot.Monitor.App).BindView();
        }

        /// <summary>
        /// Connect Windows Commands
        /// </summary>
        private void ConnectCommands()
        {
            MenuItemQuit.CommandParameter = MenuItemQuit;
            MenuItemQuit.Command = App.Current as LanguageServer.Robot.Monitor.App;
            MenuItemSettings.CommandParameter = MenuItemSettings;
            MenuItemSettings.Command = App.Current as LanguageServer.Robot.Monitor.App;
            MenuPlayScenario.CommandParameter = MenuPlayScenario;
            MenuPlayScenario.Command = App.Current as LanguageServer.Robot.Monitor.App;
            MenuDisplayScenario.CommandParameter = MenuDisplayScenario;
            MenuDisplayScenario.Command = App.Current as LanguageServer.Robot.Monitor.App;
        }
    }
}
