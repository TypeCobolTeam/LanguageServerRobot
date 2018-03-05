using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TypeCobol.LanguageServer.Robot.Monitor.View;
using Newtonsoft.Json.Linq;

namespace TypeCobol.LanguageServer.Robot.Monitor.Controller
{
    /// <summary>
    /// The Controller of a Json tree
    /// </summary>
    public class JSonTreeController : GenericDialogController
    {
        /// <summary>
        /// Default controler
        /// </summary>
        public JSonTreeController() : base(GenericDialogButton.OK, new JSonTreeViewer())
        {
        }

        /// <summary>
        /// JToken Default controler
        /// </summary>
        public JSonTreeController(JToken token) : base(GenericDialogButton.OK, new JSonTreeViewer())
        {
            View = (JSonTreeViewer) base.UserControl;
            SetJSonContent(token);
        }

        /// <summary>
        /// The View
        /// </summary>
        public new JSonTreeViewer View
        {
            get;
            private set;
        }

        /// <summary>
        /// Set the TreeView content to hold the given JToken object
        /// </summary>
        /// <param name="token">The JToken object.</param>
        public void SetJSonContent(JToken token)
        {
            var children = new List<JToken>();
            if (token != null)
            {
                children.Add(token);
            }
            View.TreeView.ItemsSource = null;
            View.TreeView.Items.Clear();
            View.TreeView.ItemsSource = children;
        }        
    }
}
