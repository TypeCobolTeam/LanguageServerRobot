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

namespace LanguageServer.Robot.Monitor.View
{
    /// <summary>
    /// Interaction logic for SessionExplorerView.xaml
    /// </summary>
    public partial class SessionExplorerView : UserControl
    {
        RoutedPropertyChangedEventHandler<object> VetoSelectionChangedEvent;
        public SessionExplorerView()
        {
            InitializeComponent();
        }

        private void SelectionChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            if (VetoSelectionChangedEvent != null)
                VetoSelectionChangedEvent(sender, e);
            //Perform actions when SelectedItem changes
            //MessageBox.Show(((TreeViewItem)e.NewValue).Header.ToString());
        }

        /// <summary>
        /// Handler when the contextual menu is about to be shown.
        /// </summary>
        public event EventHandler<ContextMenuEventArgs> ContextMenuOpeningOpeningHandler;

        private void FrameworkElement_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (ContextMenuOpeningOpeningHandler != null)
                ContextMenuOpeningOpeningHandler(sender, e);
        }
    }

}
