﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LanguageServerRobot {
    using System;
    
    
    /// <summary>
    ///   Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LanguageServerRobot.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///   les recherches de ressources à l'aide de cette classe de ressource fortement typée.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à A document with the same uri is already opened: {0}.
        /// </summary>
        internal static string DuplicateDidOpenNotification {
            get {
                return ResourceManager.GetString("DuplicateDidOpenNotification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Initialize request failure with message : {0}.
        /// </summary>
        internal static string FailClientInitializeMessage {
            get {
                return ResourceManager.GetString("FailClientInitializeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to create the session directory &apos;{0}&apos;, using default directory &apos;{1}&apos;.
        /// </summary>
        internal static string FailCreateSessionDirectoryUseDefaultPath {
            get {
                return ResourceManager.GetString("FailCreateSessionDirectoryUseDefaultPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to read the script in file: {0} \n{1}.
        /// </summary>
        internal static string FailReadScriptFile {
            get {
                return ResourceManager.GetString("FailReadScriptFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to read the session in file: {0} \n{1}.
        /// </summary>
        internal static string FailReadSessionFile {
            get {
                return ResourceManager.GetString("FailReadSessionFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to create the session directory : {0}.
        /// </summary>
        internal static string FailtoCreateSessionDirectory {
            get {
                return ResourceManager.GetString("FailtoCreateSessionDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to save script &apos;{0}&apos; : &apos;{1}&apos;.
        /// </summary>
        internal static string FailToSaveScript {
            get {
                return ResourceManager.GetString("FailToSaveScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Cannot save script &apos;{0}&apos; there is no session directory..
        /// </summary>
        internal static string FailToSaveScriptNoSessionDirectory {
            get {
                return ResourceManager.GetString("FailToSaveScriptNoSessionDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to session script &apos;{0}&apos; : &apos;{1}&apos;.
        /// </summary>
        internal static string FailToSaveSession {
            get {
                return ResourceManager.GetString("FailToSaveSession", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to save the session there is no directory associated to it..
        /// </summary>
        internal static string FailToSaveSessionNoDirectory {
            get {
                return ResourceManager.GetString("FailToSaveSessionNoDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à There is no session or script to be replayed..
        /// </summary>
        internal static string NoSessionOrScriptFile {
            get {
                return ResourceManager.GetString("NoSessionOrScriptFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Server Initialization Error : Code = {0}; Message = &apos;{1}&apos;.
        /// </summary>
        internal static string ServerInitializeError {
            get {
                return ResourceManager.GetString("ServerInitializeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à An Unexpected message has been received from the client: {0}..
        /// </summary>
        internal static string UnexpectedMessageFromClient {
            get {
                return ResourceManager.GetString("UnexpectedMessageFromClient", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à An Unexpected Request has been sent from the server: {0}.
        /// </summary>
        internal static string UnexpectedRequestFromServer {
            get {
                return ResourceManager.GetString("UnexpectedRequestFromServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à An Unexpected Response has been received from the client: {0}.
        /// </summary>
        internal static string UnexpectedResponseFromClient {
            get {
                return ResourceManager.GetString("UnexpectedResponseFromClient", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à An Unexpected Response has been received from the server without a corresponding request: {0}.
        /// </summary>
        internal static string UnexpectedResponseFromServer {
            get {
                return ResourceManager.GetString("UnexpectedResponseFromServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à No &quot;did/Open&quot; notification that matches the &quot;did/Close&quot; notification: {0}.
        /// </summary>
        internal static string UnmatcheDidCloseNotification {
            get {
                return ResourceManager.GetString("UnmatcheDidCloseNotification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à The given uri is not associated to a script: {0}.
        /// </summary>
        internal static string UriNotAssociatedToAScript {
            get {
                return ResourceManager.GetString("UriNotAssociatedToAScript", resourceCulture);
            }
        }
    }
}
