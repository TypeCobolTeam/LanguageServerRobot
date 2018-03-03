using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeCobol.LanguageServer.Robot.Monitor.Controller
{
    public enum GenericDialogResult
    {
        //
        // Summary:
        //     The dialog box returns no result.
        None = 0,
        //
        // Summary:
        //     The result value of the Dialog box is OK.
        Ok = 1,
        //
        // Summary:
        //     The result value of the Dialog box is Cancel.
        Cancel = 2,
        //
        // Summary:
        //     The result value of the Dialog box is Yes.
        Yes = 6,
        //
        // Summary:
        //     The result value of the Dialod box is No.
        No = 7,
        //
        // Summary:
        //     The result value of the Dialod box is Retry.
        Retry = 8,
        //
        // Summary:
        //     The result value of the Dialod box is Abort.
        Abort = 9
    }
}
