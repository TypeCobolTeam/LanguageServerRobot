using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TypeCobol.LanguageServer.Robot.Monitor.View
{
    /// <summary>
    /// Logique d'interaction pour GenericDialogBox.xaml
    /// </summary>
    public partial class GenericDialogView : Window
    {
        public GenericDialogView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// OK Button
        /// </summary>
        public Button OK
        {
            get;
            set;
        }

        /// <summary>
        /// Cancel Button
        /// </summary>
        public Button Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// Yes Button
        /// </summary>
        public Button Yes
        {
            get;
            set;
        }

        /// <summary>
        /// No Button
        /// </summary>
        public Button No
        {
            get;
            set;
        }

        /// <summary>
        /// Retry Button
        /// </summary>
        public Button Abort
        {
            get;
            set;
        }

        /// <summary>
        /// Retry Button
        /// </summary>
        public Button Retry
        {
            get;
            set;
        }

        private void GenericDialogViewView_Loaded(object sender, RoutedEventArgs e)
        {
  
        }
    }
}
