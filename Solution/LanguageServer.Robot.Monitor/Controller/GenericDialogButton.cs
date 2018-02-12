using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.Robot.Monitor.Controller
{
    public enum GenericDialogButton
    {
        // Résumé :
        //     La zone de message contient un bouton OK.
        OK = 0,
        //
        // Résumé :
        //     La zone de message contient les boutons OK et Annuler.
        OKCancel = 1,
        //
        // Résumé :
        //     La zone de message contient les boutons Abandonner, Réessayer et Ignorer.
        AbortRetryIgnore = 2,
        //
        // Résumé :
        //     La zone de message contient les boutons Oui, Non et Annuler.
        YesNoCancel = 3,
        //
        // Résumé :
        //     La zone de message contient les boutons Oui et Non.
        YesNo = 4,
        //
        // Résumé :
        //     La zone de message contient les boutons Réessayer et Annuler.
        RetryCancel = 5
    }
}
