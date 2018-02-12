using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using LanguageServer.Protocol;
using LanguageServer.Robot.Monitor.View;

namespace LanguageServer.Robot.Monitor.Controller
{
    public class GenericDialogController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="buttons"></param>
        /// <param name="control"></param>
        public GenericDialogController(GenericDialogButton buttons, Control control)
        {
            Result = GenericDialogResult.None;
            Buttons = buttons;
            UserControl = control;
            View = new GenericDialogView();
            SetupView();
            ConnectView();
        }
        /// <summary>
        /// The User Control
        /// </summary>
        public Control UserControl
        {
            get;
            protected set;
        }

        /// <summary>
        /// The View
        /// </summary>
        public GenericDialogView View
        {
            get;
            protected set;
        }

        /// <summary>
        /// Buttons Styles
        /// </summary>
        public GenericDialogButton Buttons
        {
            get;
            protected set;
        }

        public GenericDialogResult Result {
            get;
            private set;            
        }

        /// <summary>
        /// Add The given button
        /// </summary>
        /// <param name="b"></param>
        protected void AddButton(Button b)
        {
            b.HorizontalAlignment = HorizontalAlignment.Stretch;
            b.VerticalAlignment = VerticalAlignment.Stretch;
            b.Width = System.Double.NaN;
            b.Height = System.Double.NaN;
            b.Click += Button_Click;
            View.ButtonPanel.Children.Add(b);
        }

        protected virtual void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == View.OK)
            {
                Result = GenericDialogResult.Ok;
            }
            else if (sender == View.Cancel)
            {
                Result = GenericDialogResult.Cancel;
            }
            else if (sender == View.Abort)
            {
                Result = GenericDialogResult.Abort;
            }
            else if (sender == View.Retry)
            {
                Result = GenericDialogResult.Retry;
            }
            else if (sender == View.Yes)
            {
                Result = GenericDialogResult.Yes;
            }
            else if (sender == View.No)
            {
                Result = GenericDialogResult.No;
            }
            MyWindow.Close();
        }

        /// <summary>
        /// Vie< Setup
        /// </summary>
        private void SetupView()
        {
            View.ContentPanel.Children.Add(UserControl);
            switch (Buttons)
            {
                case GenericDialogButton.OK:
                    View.OK = new Button();
                    View.OK.Content = "OK";
                    AddButton(View.OK);
                    break;
                case GenericDialogButton.OKCancel:
                    View.OK = new Button();
                    View.OK.Content = "OK";
                    AddButton(View.OK);
                    View.Cancel = new Button();
                    View.Cancel.Content = "Cancel";
                    AddButton(View.Cancel);
                    break;
                case GenericDialogButton.AbortRetryIgnore:
                    View.Abort = new Button();
                    View.Abort.Content = "Abort";
                    AddButton(View.Abort);
                    View.Retry = new Button();
                    View.Retry.Content = "Retry";
                    AddButton(View.Retry);
                    View.Cancel = new Button();
                    View.Cancel.Content = "Cancel";
                    AddButton(View.Cancel);
                    break;
                case GenericDialogButton.YesNoCancel:
                    View.Yes = new Button();
                    View.Yes.Content = "Yes";
                    AddButton(View.Yes);
                    View.No = new Button();
                    View.No.Content = "No";
                    AddButton(View.No);
                    View.Cancel = new Button();
                    View.Cancel.Content = "Cancel";
                    AddButton(View.Cancel);
                    break;
                case GenericDialogButton.YesNo:
                    View.Yes = new Button();
                    View.Yes.Content = "Yes";
                    AddButton(View.Yes);
                    View.No = new Button();
                    View.No.Content = "No";
                    AddButton(View.No);
                    break;
                case GenericDialogButton.RetryCancel:
                    View.Retry = new Button();
                    View.Retry.Content = "Retry";
                    AddButton(View.Retry);
                    View.Cancel = new Button();
                    View.Cancel.Content = "Cancel";
                    AddButton(View.Cancel);
                    break;
            }
        }
        void DialogViewLoaded(object sender, RoutedEventArgs e)
        {
            SetupView();
        }

        protected virtual void ConnectView()
        {
        }

        /// <summary>
        /// Shows the Dialog
        /// </summary>
        /// <param name="title"></param>
        public virtual GenericDialogResult Show(string title = null)
        {
            DockPanel dock = new DockPanel();
            dock.Children.Add(View);
            Window window = MyWindow = new Window
            {
                Title = title ?? Properties.Resources.LSRMName,
                Content = dock
            };                        
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.VerticalContentAlignment = VerticalAlignment.Top;
            window.HorizontalContentAlignment = HorizontalAlignment.Left;

            window.ShowDialog();
            return Result;
        }

        protected Window MyWindow;
    }
}
