﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TypeCobol.LanguageServer.Robot.Common {
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
    public class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TypeCobol.LanguageServer.Robot.Common.Resource", typeof(Resource).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
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
        public static string DuplicateDidOpenNotification {
            get {
                return ResourceManager.GetString("DuplicateDidOpenNotification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Initialize request failure with message : {0}.
        /// </summary>
        public static string FailClientInitializeMessage {
            get {
                return ResourceManager.GetString("FailClientInitializeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to create the session directory &apos;{0}&apos;, using default directory &apos;{1}&apos;.
        /// </summary>
        public static string FailCreateSessionDirectoryUseDefaultPath {
            get {
                return ResourceManager.GetString("FailCreateSessionDirectoryUseDefaultPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to read the script in file: {0} \n{1}.
        /// </summary>
        public static string FailReadScriptFile {
            get {
                return ResourceManager.GetString("FailReadScriptFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to read the session in file: {0} \n{1}.
        /// </summary>
        public static string FailReadSessionFile {
            get {
                return ResourceManager.GetString("FailReadSessionFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to create the session directory : {0}.
        /// </summary>
        public static string FailtoCreateSessionDirectory {
            get {
                return ResourceManager.GetString("FailtoCreateSessionDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to load the json &quot;workspace/didChangeConfiguration&quot; notification file..
        /// </summary>
        public static string FailToLoadConfigFile {
            get {
                return ResourceManager.GetString("FailToLoadConfigFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to load the json &quot;initialize&quot; request file.
        /// </summary>
        public static string FailToLoadInitFile {
            get {
                return ResourceManager.GetString("FailToLoadInitFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to save script &apos;{0}&apos; : &apos;{1}&apos;.
        /// </summary>
        public static string FailToSaveScript {
            get {
                return ResourceManager.GetString("FailToSaveScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Cannot save script &apos;{0}&apos; there is no session directory..
        /// </summary>
        public static string FailToSaveScriptNoSessionDirectory {
            get {
                return ResourceManager.GetString("FailToSaveScriptNoSessionDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to session script &apos;{0}&apos; : &apos;{1}&apos;.
        /// </summary>
        public static string FailToSaveSession {
            get {
                return ResourceManager.GetString("FailToSaveSession", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Fail to save the session there is no directory associated to it..
        /// </summary>
        public static string FailToSaveSessionNoDirectory {
            get {
                return ResourceManager.GetString("FailToSaveSessionNoDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Language Server Robot.
        /// </summary>
        public static string LSRTitle {
            get {
                return ResourceManager.GetString("LSRTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à LangugeServer Robot Monitor connection timeout.
        /// </summary>
        public static string MonitorConnectionTimeout {
            get {
                return ResourceManager.GetString("MonitorConnectionTimeout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à The monitoring is activated only and only if the -robot options is pecified..
        /// </summary>
        public static string MonitoringInClientServerModeOnly {
            get {
                return ResourceManager.GetString("MonitoringInClientServerModeOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à The Monitoring mode is activated only and only if the -robot option is pecified..
        /// </summary>
        public static string MonitoringInRomotModeOnly {
            get {
                return ResourceManager.GetString("MonitoringInRomotModeOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Close this message to start before {0} minutes!....
        /// </summary>
        public static string MsgPressOkToStartBeforeMinutes {
            get {
                return ResourceManager.GetString("MsgPressOkToStartBeforeMinutes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à There is no session or script to be replayed..
        /// </summary>
        public static string NoSessionOrScriptFile {
            get {
                return ResourceManager.GetString("NoSessionOrScriptFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Server Initialization Error : Code = {0}; Message = &apos;{1}&apos;.
        /// </summary>
        public static string ServerInitializeError {
            get {
                return ResourceManager.GetString("ServerInitializeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à An Unexpected message has been received from the client: {0}..
        /// </summary>
        public static string UnexpectedMessageFromClient {
            get {
                return ResourceManager.GetString("UnexpectedMessageFromClient", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à An Unexpected Request has been sent from the server: {0}.
        /// </summary>
        public static string UnexpectedRequestFromServer {
            get {
                return ResourceManager.GetString("UnexpectedRequestFromServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à An Unexpected Response has been received from the client: {0}.
        /// </summary>
        public static string UnexpectedResponseFromClient {
            get {
                return ResourceManager.GetString("UnexpectedResponseFromClient", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à An Unexpected Response has been received from the server without a corresponding request: {0}.
        /// </summary>
        public static string UnexpectedResponseFromServer {
            get {
                return ResourceManager.GetString("UnexpectedResponseFromServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à No &quot;did/Open&quot; notification that matches the &quot;did/Close&quot; notification: {0}.
        /// </summary>
        public static string UnmatcheDidCloseNotification {
            get {
                return ResourceManager.GetString("UnmatcheDidCloseNotification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à The given uri is not associated to a script: {0}.
        /// </summary>
        public static string UriNotAssociatedToAScript {
            get {
                return ResourceManager.GetString("UriNotAssociatedToAScript", resourceCulture);
            }
        }
    }
}