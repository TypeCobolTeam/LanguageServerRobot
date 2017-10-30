﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;
using LanguageServerRobot.Model;
using LanguageServerRobot.Utilities;
using Newtonsoft.Json.Linq;

namespace LanguageServerRobot.Controller
{
    /// <summary>
    /// Robot Recording Mode Controller.
    /// </summary>
    public class RecordingModeController : AbstractModeController
    {
        /// <summary>
        /// The recording State
        /// </summary>
        public enum RecordingState
        {
            /// <summary>
            /// Waiting for initialization.
            /// </summary>
            NotInitialized = 0x01 << 0,
            /// <summary>
            /// Initialized
            /// </summary>
            Initialized = 0x01 << 1,
            /// <summary>
            /// The Recording is started.
            /// </summary>
            Start = 0x01 << 2,

            /// <summary>
            /// The recording is stopped
            /// </summary>
            Stop = 0x01 << 3,

            /// <summary>
            /// Initialization request error
            /// </summary>
            InitializationError = 0x01 << 4

        };

        /// <summary>
        /// The Current State.
        /// </summary>
        public RecordingState State
        {
            get;
            protected set;
        }

        /// <summary>
        /// The Model of the current session.
        /// </summary>
        public Session SessionModel
        {
            get; protected set;
        }

        /// <summary>
        /// The JSON Object of the "initialize" request;
        /// </summary>
        public JObject JInitializeObject
        {
            get;
            private set;
        }

        /// <summary>
        /// The original "initialize" request
        /// </summary>
        protected string InitializeRequest
        {
            get;
            set;
        }

        /// <summary>
        /// The original "shutdown" request
        /// </summary>
        protected string ShutdownRequest
        {
            get;
            set;
        }

        /// <summary>
        /// The original JSON Object of "shutdown" request
        /// </summary>
        protected JObject JShutdownObject
        {
            get;
            set;
        }

        public override bool IsModeInitialized
        {
            get
            {
                return ((int)State & (int)RecordingState.Initialized) != 0 && !IsInitializationError;
            }
        }

        public override bool IsModeStarted
        {
            get
            {
                return ((int)State & (int)RecordingState.Start) != 0 && !IsModeStopped;
            }
        }

        public override bool IsModeStopped
        {
            get
            {
                return ((int)State & (int)RecordingState.Stop) != 0;
            }
        }

        private bool IsInitializationError
        {
            get
            {
                return ((int)State & (int)RecordingState.InitializationError) != 0;
            }
        }

        /// <summary>
        /// The Dictionary that give for an uri, the associated script.
        /// </summary>
        Dictionary<string, Script> ScriptMap
        { get; set; }

        /// <summary>
        /// The Map that associate a Request Id from the Client to its Uri.
        /// </summary>
        Dictionary<string, string> RequestIdUriMap;

        /// <summary>
        /// The list of valid script of the session.
        /// </summary>
        List<Script> Scripts;

        /// <summary>
        /// Empty constructor.
        /// <param name="scriptRepositoryPath">The script repository path, if null the default script repository path will be taken</param>
        /// </summary>
        public RecordingModeController(string scriptRepositoryPath = null) : base(scriptRepositoryPath)
        {            
            State = RecordingState.NotInitialized;
            RequestIdUriMap = new Dictionary<string, string> ();
        }

        /// <summary>
        /// Get the Script corresponding to the given uri.
        /// </summary>
        /// <param name="uri">The uri to get the string</param>
        /// <returns>The corresponding script if any, null otherwise</returns>
        private Script this[string uri]
        {
            get
            {
                System.Diagnostics.Contracts.Contract.Assert(uri != null);
                if (ScriptMap == null)
                    return null;
                return ScriptMap.ContainsKey(uri) ? ScriptMap[uri] : null;
            }
            set
            {
                System.Diagnostics.Contracts.Contract.Assert(uri != null);
                System.Diagnostics.Contracts.Contract.Assert(value != null);
                Script script = this[uri];
                System.Diagnostics.Contracts.Contract.Assume(script == null || script == value);
                if (ScriptMap == null)
                    ScriptMap = new Dictionary<string, Script>();
                ScriptMap[uri] = value;
            }
        }
        /// <summary>
        /// Handle message incoming from the client side.
        /// </summary>
        /// <param name="message">The client side message</param>
        public override void FromClient(string message)
        {
            System.Diagnostics.Contracts.Contract.Assert(message != null);
            JObject jsonObject = null;
            bool consumed = false;
            switch(State)
            {
                case RecordingState.NotInitialized:
                    {
                        System.Diagnostics.Contracts.Contract.Ensures(SessionModel == null && JInitializeObject == null);
                        if (SessionModel == null && JInitializeObject == null)
                        {
                            if (Protocol.IsInitializeRequest(message, out jsonObject))
                            {   //Save the original initialization object.
                                JInitializeObject = jsonObject;
                                InitializeRequest = message;
                                consumed = true;
                            }
                        }
                        if (!consumed)
                        {
                            LogNotConsumedMessage(message);
                        }
                    }
                    break;
                case RecordingState.Initialized:
                    {
                        if (Protocol.IsNotification(message, out jsonObject))
                        {
                            if (Protocol.IsInitializedNotification(message, out jsonObject))
                            {   //Notification from the Client that it has take in account the "initialize" result from the server.
                                //==> We can start both Client and Server are OK.
                                State |= RecordingState.Start;
                                SessionModel.initialized_notification = message;
                                consumed = true;
                            }
                        }
                        if (!consumed)
                        {
                            SessionModel.client_in_initialize_messages.Add(message);
                        }
                    }
                    break;
                case RecordingState.Initialized | RecordingState.Start:
                    {
                        string uri = null;                        
                        if (Protocol.IsNotification(message, out jsonObject))
                        {//1)Detect any new script for a document ==> detect didOpen notification
                            if (Protocol.IsDidOpenTextDocumentNotification(jsonObject))
                            {
                                StartScript(message, jsonObject);
                                consumed = true;
                            }
                            else if (Protocol.IsDidCloseTextDocumentNotification(jsonObject))
                            {
                                StopScript(message, jsonObject);
                                consumed = true;
                            }
                            else if (Protocol.IsMessageWithUri(jsonObject, out uri))
                            {
                                consumed = RecordScriptMessage(Script.MessageCategory.Client, Protocol.Message_Kind.Notification, message, uri, jsonObject);
                            }
                        }
                        else if (Protocol.IsRequest(jsonObject))
                        {
                            if (Protocol.IsMessageWithUri(message, out uri, out jsonObject))
                            {
                                consumed = RecordScriptMessage(Script.MessageCategory.Client, Protocol.Message_Kind.Request, message, uri, jsonObject);
                            }
                            else if (Protocol.IsShutdownRequest(jsonObject))
                            {
                                JShutdownObject = jsonObject;
                                ShutdownRequest = message;
                                consumed = true;
                            }
                        }
                        else if (Protocol.IsErrorResponse(jsonObject))
                        {//Hum...A response receive from the Client this cannot happend ==> Log it.
                            LogUnexpectedMessage(Resource.UnexpectedResponseFromClient, message);
                        }
                    }
                    if (!consumed)
                    {
                        SessionModel.client_in_start_messages.Add(message);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handle message comming from the server side.
        /// </summary>
        /// <param name="message">The Server side message.</param>
        public override void FromServer(string message)
        {
            System.Diagnostics.Contracts.Contract.Assert(message != null);
            JObject jsonObject = null;
            bool consumed = false;
            switch (State)
            {
                case RecordingState.NotInitialized:
                    {
                        System.Diagnostics.Contracts.Contract.Ensures(SessionModel == null && JInitializeObject != null);
                        if (SessionModel == null && JInitializeObject != null)
                        {
                            if (Protocol.IsResponse(message, out jsonObject))
                            {
                                string requestId = Protocol.GetRequestId(JInitializeObject);
                                string responseIde = Protocol.GetRequestId(jsonObject);
                                if (requestId != null && responseIde != null && requestId.Equals(responseIde))
                                {//Ok Initalization result
                                    InitializeSession(message, jsonObject);
                                    consumed = true;
                                }
                            }
                        }
                        if (!consumed)
                        {
                            LogNotConsumedMessage(message);
                        }
                    }
                    break;
                case RecordingState.Initialized:
                    if (!consumed)
                    {
                        SessionModel.server_in_initialize_messages.Add(message);
                    }
                    break;
                case RecordingState.Initialized | RecordingState.Start:
                    {
                        string uri = null;
                        if (Protocol.IsNotification(message, out jsonObject))
                        {
                            if (Protocol.IsMessageWithUri(message, out uri, out jsonObject))
                            {
                                consumed = RecordScriptMessage(Script.MessageCategory.Server, Protocol.Message_Kind.Notification, message, uri, jsonObject);
                            }
                        }
                        else if (Protocol.IsResponseAndNotError(jsonObject))
                        {
                            string id = Protocol.GetRequestId(jsonObject);
                            if (JShutdownObject != null)
                            {
                                string id_shutdown = Protocol.GetRequestId(JShutdownObject);
                                if (id.Equals(id_shutdown))
                                {//Shutdown response
                                    StopSession(message, jsonObject);
                                    consumed = true;
                                }
                            }
                            if (!consumed)
                            {
                                if (RequestIdUriMap.ContainsKey(id))
                                {//So this is another response for a request ==> just record it
                                    uri = RequestIdUriMap[id];
                                    consumed = RecordScriptMessage(Script.MessageCategory.Server, Protocol.Message_Kind.Response, message, uri, jsonObject);
                                }
                                else
                                {//Hum...There is a response from the server without a registered request
                                    LogUnexpectedMessage(Resource.UnexpectedResponseFromServer, message);
                                }
                            }
                        }
                        else if (Protocol.IsErrorResponse(jsonObject))
                        {
                            string id = Protocol.GetRequestId(jsonObject);
                            if (RequestIdUriMap.ContainsKey(id))
                            {
                                uri = RequestIdUriMap[id];
                                consumed = RecordScriptMessage(Script.MessageCategory.Result, Protocol.Message_Kind.Response, message, uri, jsonObject);
                            }
                            else
                            {//Hum...There is a response from the server without a registered request
                                LogUnexpectedMessage(Resource.UnexpectedResponseFromServer, message);
                            }
                        }
                        else
                        {//It's a Request from the server??? cannot happend a Server cannot send a request to a client.
                         // Log this ==>
                            LogUnexpectedMessage(Resource.UnexpectedRequestFromServer, message);
                        }
                    }
                    if (!consumed)
                    {
                        SessionModel.server_in_start_messages.Add(message);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Initialization of a Recording Session
        /// </summary>
        /// <param name="message">The LSP initialization result</param>
        /// <param name="jsonObject">The Initialization JSon result</param>
        private void InitializeSession(string message, JObject jsonObject)
        {
            //Maybe initailization failed            
            if (Protocol.IsErrorResponse(jsonObject))
            {
                State |= RecordingState.InitializationError;
            }
            else
            {
                State &= ~RecordingState.NotInitialized;
                State |= RecordingState.Initialized;                                
                SessionModel = new Session();
                string sessionDirectoryPath = null;
                if (!Util.CreateSessionDirectory(out sessionDirectoryPath, this.ScriptRepositoryPath))                
                {
                    string defaultDirectoryPath;
                    if (Util.CreateSessionDirectory(out defaultDirectoryPath))
                    {
                        LogWriter?.WriteLine(string.Format(Resource.FailCreateSessionDirectoryUseDefaultPath, sessionDirectoryPath, defaultDirectoryPath));
                        SessionModel.directory = defaultDirectoryPath;
                    }
                    else
                    {
                        LogWriter?.WriteLine(string.Format(Resource.FailtoCreateSessionDirectory, sessionDirectoryPath));
                    }
                }
                else
                {
                    SessionModel.directory = sessionDirectoryPath;
                }
                SessionModel.initialize = InitializeRequest;
                SessionModel.initialize_result = message;
            }
        }

        /// <summary>
        /// Starts a script
        /// </summary>
        /// <param name="message">The original "textDocument/didOpen" notification</param>
        /// <param name="jsonObject">The Json object corresponding to the "textDocument/didOpen" notification</param>
        private void StartScript(string message, JObject jsonObject)
        {
            string uri = null;
            Protocol.IsMessageWithUri(jsonObject, out uri);
            System.Diagnostics.Contracts.Contract.Assume(uri != null);
            Script script = this[uri];
            if (script != null)
            {//Hum...A script with the same uri is already opened??? ==> Log This
                LogWriter?.WriteLine(string.Format(Resource.DuplicateDidOpenNotification, uri));
            }
            else
            {
                script = new Script(uri);
                script.didOpen = message;
                this[uri] = script;
            }
        }

        /// <summary>
        /// Record a message for a script.
        /// </summary>
        /// <param name="category">The actegory of message (Client, Server, Result)</param>
        /// <param name="kind">The kind of message : (Notification, Request)</param>
        /// <param name="message">The original message</param>
        /// <param name="uri"><The Script's uri/param>
        /// <param name="jsonObject">The JSon object corresponding to the message</param>
        /// <returns></returns>
        private bool RecordScriptMessage(Script.MessageCategory category, Protocol.Message_Kind kind, string message, string uri, JObject jsonObject)
        {
            System.Diagnostics.Contracts.Contract.Assert(message != null);
            System.Diagnostics.Contracts.Contract.Requires(uri != null);
            Script script = this[uri];
            if (script == null)
            {//Hum...this Uri is not associated to a script
                LogWriter?.WriteLine(string.Format(Resource.UriNotAssociatedToAScript, uri));
                return false;
            }
            else
            {   //Record the message
                if (category == Script.MessageCategory.Client && kind == Protocol.Message_Kind.Request)
                {//This is a request from the client ==> so remember its ID and its associated Uri.
                    string id = Protocol.GetRequestId(jsonObject);
                    RequestIdUriMap[id] = uri;
                }
                else if (category == Script.MessageCategory.Result && kind == Protocol.Message_Kind.Response)
                {//Rmove the reference to the request
                    string id = Protocol.GetRequestId(jsonObject);
                    RequestIdUriMap.Remove(id);
                }
                script.AddMessage(category, kind, message, jsonObject);
                return true;
            }            
        }

        /// <summary>
        /// Stop a script and save it
        /// </summary>
        /// <param name="message">The original "textDocument/didClose" notification</param>
        /// <param name="jsonObject">The Json object corresponding to the "textDocument/didClose" notification</param>
        private void StopScript(string message, JObject jsonObject)
        {
            string uri = null;
            Protocol.IsMessageWithUri(jsonObject, out uri);
            System.Diagnostics.Contracts.Contract.Assume(uri != null);
            Script script = this[uri];
            if (script == null)
            {//Hum...No script with the same uri??? ==> Log This
                base.ProtocolLogWriter?.WriteLine(string.Format(Resource.UnmatcheDidCloseNotification, message));
            }
            else
            {
                script.didClose = message;
                if (script.IsValid)
                {//We have a valid script.
                    script.uri = uri;
                    //Store the script in the list of valid script.
                    if (Scripts == null)
                        Scripts = new List<Script>();
                    Scripts.Add(script);
                    //Remove the uri in the entry Map, because one can reopen the document, thus creates a new script.
                    ScriptMap.Remove(uri);
                    SaveScript(script);
#if DEBUG
                    script.DebugDump();
#endif
                }
            }
        }

        /// <summary>
        /// Save a script in a file using UTF-8 encoding.
        /// </summary>
        /// <param name="script">The script to be saved</param>
        /// <param name="scriptFile">The script firl path</param>
        private bool SaveScript(Script script)
        {
            System.Diagnostics.Contracts.Contract.Assume(script.IsValid);
            System.Diagnostics.Contracts.Contract.Requires(script.uri != null);
            string scriptFile = Util.UriToIdentifierName(script.uri + '_' + script.date);
            scriptFile += Util.SCRIPT_FILE_EXTENSION;
            if (SessionModel.directory == null)
            {
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveScriptNoSessionDirectory, scriptFile));
                return false;
            }
            scriptFile = System.IO.Path.Combine(SessionModel.directory, scriptFile);
            bool bResult = false;
            script.session = SessionModel.GetSessionFileName();
            script.initialize = SessionModel.initialize;
            script.initialize_result = SessionModel.initialize_result;
            SessionModel.scripts.Add(scriptFile);
            try
            {
                using (FileStream stream = System.IO.File.Create(scriptFile))
                {
                    script.Write(stream);
                    bResult = true;
                }
            }
            catch (System.UnauthorizedAccessException uae)
            {
                //     The caller does not have the required permission.-or- path specified a file that
                //     is read-only.
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveScript, scriptFile, uae.Message));
            }
            catch (System.ArgumentNullException ane)
            {
                //     path is null.
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveScript, scriptFile, ane.Message));
            }
            catch (System.ArgumentException ae)
            {
                //     path is a zero-length string, contains only white space, or contains one or more
                //     invalid characters as defined by System.IO.Path.InvalidPathChars.
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveScript, scriptFile, ae.Message));
            }
            catch (System.IO.PathTooLongException ptle)
            {
                //     The specified path, file name, or both exceed the system-defined maximum length.
                //     For example, on Windows-based platforms, paths must be less than 248 characters,
                //     and file names must be less than 260 characters.
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveScript, scriptFile, ptle.Message));
            }
            catch (System.IO.DirectoryNotFoundException dnfe)
            {
                //     The specified path is invalid (for example, it is on an unmapped drive).
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveScript, scriptFile, dnfe.Message));
            }
            catch (System.IO.IOException ioe)
            {
                //     An I/O error occurred while creating the file.
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveScript, scriptFile, ioe.Message));
            }
            catch (System.NotSupportedException nse)
            {
                //     path is in an invalid format.
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveScript, scriptFile, nse.Message));
            }
            return bResult;
        }

        /// <summary>
        /// Stops the current session and save it.
        /// </summary>
        /// <param name="message">The "shutdown" message</param>
        /// <param name="jsonObject">The JSON object corresponding to the shutdown message.</param>
        private void StopSession(string message, JObject jsonObject)
        {
            System.Diagnostics.Contracts.Contract.Assert(SessionModel != null);
            SessionModel.shutdown = ShutdownRequest;
            SaveSession();
#if DEBUG
            SessionModel.DebugDump();
#endif
            //No session anymore
            SessionModel = null;
        }

        /// <summary>
        /// Save the curreent session
        /// </summary>
        /// <returns>true if the session has been saved false otherwise</returns>
        private bool SaveSession()
        {
            System.Diagnostics.Contracts.Contract.Assume(SessionModel != null);
            if (SessionModel.directory == null)
            {
                LogWriter?.WriteLine(Resource.FailToSaveSessionNoDirectory);
                return false;
            }
            bool bResult = false;
            string sessionFile = SessionModel.GetSessionFileName();
            try
            {
                using (FileStream stream = System.IO.File.Create(sessionFile))
                {
                    SessionModel.Write(stream);
                    bResult = true;
                }
            }
            catch (System.UnauthorizedAccessException uae)
            {
                //     The caller does not have the required permission.-or- path specified a file that
                //     is read-only.
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveSession, sessionFile, uae.Message));
            }
            catch (System.ArgumentNullException ane)
            {
                //     path is null.
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveSession, sessionFile, ane.Message));
            }
            catch (System.ArgumentException ae)
            {
                //     path is a zero-length string, contains only white space, or contains one or more
                //     invalid characters as defined by System.IO.Path.InvalidPathChars.
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveSession, sessionFile, ae.Message));
            }
            catch (System.IO.PathTooLongException ptle)
            {
                //     The specified path, file name, or both exceed the system-defined maximum length.
                //     For example, on Windows-based platforms, paths must be less than 248 characters,
                //     and file names must be less than 260 characters.
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveSession, sessionFile, ptle.Message));
            }
            catch (System.IO.DirectoryNotFoundException dnfe)
            {
                //     The specified path is invalid (for example, it is on an unmapped drive).
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveSession, sessionFile, dnfe.Message));
            }
            catch (System.IO.IOException ioe)
            {
                //     An I/O error occurred while creating the file.
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveSession, sessionFile, ioe.Message));
            }
            catch (System.NotSupportedException nse)
            {
                //     path is in an invalid format.
                LogWriter?.WriteLine(string.Format(Resource.FailToSaveSession, sessionFile, nse.Message));
            }
            return true;
        }
    }
}

