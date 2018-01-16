﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LanguageServer.Robot.Common {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
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
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LanguageServer.Robot.Common.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
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
        ///   Looks up a localized string similar to A document with the same uri is already opened: {0}.
        /// </summary>
        public static string DuplicateDidOpenNotification {
            get {
                return ResourceManager.GetString("DuplicateDidOpenNotification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initialize request failure with message : {0}.
        /// </summary>
        public static string FailClientInitializeMessage {
            get {
                return ResourceManager.GetString("FailClientInitializeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fail to create the session directory &apos;{0}&apos;, using default directory &apos;{1}&apos;.
        /// </summary>
        public static string FailCreateSessionDirectoryUseDefaultPath {
            get {
                return ResourceManager.GetString("FailCreateSessionDirectoryUseDefaultPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fail to read the script in file: {0} \n{1}.
        /// </summary>
        public static string FailReadScriptFile {
            get {
                return ResourceManager.GetString("FailReadScriptFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fail to read the session in file: {0} \n{1}.
        /// </summary>
        public static string FailReadSessionFile {
            get {
                return ResourceManager.GetString("FailReadSessionFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fail to create the session directory : {0}.
        /// </summary>
        public static string FailtoCreateSessionDirectory {
            get {
                return ResourceManager.GetString("FailtoCreateSessionDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fail to save script &apos;{0}&apos; : &apos;{1}&apos;.
        /// </summary>
        public static string FailToSaveScript {
            get {
                return ResourceManager.GetString("FailToSaveScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot save script &apos;{0}&apos; there is no session directory..
        /// </summary>
        public static string FailToSaveScriptNoSessionDirectory {
            get {
                return ResourceManager.GetString("FailToSaveScriptNoSessionDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fail to session script &apos;{0}&apos; : &apos;{1}&apos;.
        /// </summary>
        public static string FailToSaveSession {
            get {
                return ResourceManager.GetString("FailToSaveSession", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fail to save the session there is no directory associated to it..
        /// </summary>
        public static string FailToSaveSessionNoDirectory {
            get {
                return ResourceManager.GetString("FailToSaveSessionNoDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The monitoring is activated only and only if the -robot options is pecified..
        /// </summary>
        public static string MonitoringInClientServerModeOnly {
            get {
                return ResourceManager.GetString("MonitoringInClientServerModeOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Monitoring mode is activated only and only if the -robot option is pecified..
        /// </summary>
        public static string MonitoringInRomotModeOnly {
            get {
                return ResourceManager.GetString("MonitoringInRomotModeOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no session or script to be replayed..
        /// </summary>
        public static string NoSessionOrScriptFile {
            get {
                return ResourceManager.GetString("NoSessionOrScriptFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server Initialization Error : Code = {0}; Message = &apos;{1}&apos;.
        /// </summary>
        public static string ServerInitializeError {
            get {
                return ResourceManager.GetString("ServerInitializeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An Unexpected message has been received from the client: {0}..
        /// </summary>
        public static string UnexpectedMessageFromClient {
            get {
                return ResourceManager.GetString("UnexpectedMessageFromClient", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An Unexpected Request has been sent from the server: {0}.
        /// </summary>
        public static string UnexpectedRequestFromServer {
            get {
                return ResourceManager.GetString("UnexpectedRequestFromServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An Unexpected Response has been received from the client: {0}.
        /// </summary>
        public static string UnexpectedResponseFromClient {
            get {
                return ResourceManager.GetString("UnexpectedResponseFromClient", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An Unexpected Response has been received from the server without a corresponding request: {0}.
        /// </summary>
        public static string UnexpectedResponseFromServer {
            get {
                return ResourceManager.GetString("UnexpectedResponseFromServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No &quot;did/Open&quot; notification that matches the &quot;did/Close&quot; notification: {0}.
        /// </summary>
        public static string UnmatcheDidCloseNotification {
            get {
                return ResourceManager.GetString("UnmatcheDidCloseNotification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given uri is not associated to a script: {0}.
        /// </summary>
        public static string UriNotAssociatedToAScript {
            get {
                return ResourceManager.GetString("UriNotAssociatedToAScript", resourceCulture);
            }
        }
    }
}
