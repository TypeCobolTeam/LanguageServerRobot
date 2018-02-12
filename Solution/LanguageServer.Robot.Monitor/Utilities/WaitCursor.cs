using System.Windows.Input;

namespace LanguageServer.Robot.Monitor.Utilities
{
    /// <summary>
    /// Wait cursor implementation
    /// </summary>
    public class WaitCursor : System.IDisposable
    {
        private Cursor PreviousCursor;

        public WaitCursor()
        {
            PreviousCursor = Mouse.OverrideCursor;

            Mouse.OverrideCursor = Cursors.Wait;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Mouse.OverrideCursor = PreviousCursor;
        }

        #endregion
    }
}