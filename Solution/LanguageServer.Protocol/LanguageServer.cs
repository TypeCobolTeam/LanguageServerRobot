using System;
using System.Collections.Generic;
using LanguageServer.JsonRPC;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// Base class for all language servers
    /// </summary>
    public class LanguageServer
    {
        public LanguageServer(IRPCConnection rpcConnection)
        {
            this.rpcConnection = rpcConnection;
            rpcConnection.RegisterRequestMethod(CodeActionRequest.Type, CallCodeAction);
            rpcConnection.RegisterRequestMethod(CodeLensRequest.Type, CallCodeLens);
            rpcConnection.RegisterRequestMethod(CodeLensResolveRequest.Type, CallCodeLensResolve);
            rpcConnection.RegisterRequestMethod(CompletionRequest.Type, CallCompletion);
            rpcConnection.RegisterRequestMethod(CompletionResolveRequest.Type, CallCompletionResolve);
            rpcConnection.RegisterRequestMethod(DocumentHighlightRequest.Type, CallDocumentHighlight);
            rpcConnection.RegisterRequestMethod(DocumentSymbolRequest.Type, CallDocumentSymbol);
            rpcConnection.RegisterRequestMethod(DocumentFormattingRequest.Type, CallDocumentFormatting);
            rpcConnection.RegisterRequestMethod(DocumentOnTypeFormattingRequest.Type, CallDocumentOnTypeFormatting);
            rpcConnection.RegisterRequestMethod(DocumentRangeFormattingRequest.Type, CallDocumentRangeFormatting);
            rpcConnection.RegisterRequestMethod(DefinitionRequest.Type, CallDefinition);
            rpcConnection.RegisterRequestMethod(HoverRequest.Type, CallHoverRequest);
            rpcConnection.RegisterRequestMethod(InitializeRequest.Type, CallInitialize);
            rpcConnection.RegisterRequestMethod(ReferencesRequest.Type, CallReferences);
            rpcConnection.RegisterRequestMethod(RenameRequest.Type, CallRename);
            rpcConnection.RegisterRequestMethod(ShutdownRequest.Type, CallShutdown);
            rpcConnection.RegisterRequestMethod(SignatureHelpRequest.Type, CallSignatureHelp);
            rpcConnection.RegisterRequestMethod(WorkspaceSymbolRequest.Type, CallWorkspaceSymbol);
            rpcConnection.RegisterNotificationMethod(DidChangeConfigurationNotification.Type, CallDidChangeConfiguration);
            rpcConnection.RegisterNotificationMethod(ExitNotification.Type, CallExit);
            rpcConnection.RegisterNotificationMethod(DidChangeWatchedFilesNotification.Type, CallDidChangeWatchedFiles);
            rpcConnection.RegisterNotificationMethod(DidChangeTextDocumentNotification.Type, CallDidChangeTextDocument);
            rpcConnection.RegisterNotificationMethod(DidCloseTextDocumentNotification.Type, CallDidCloseTextDocument);
            rpcConnection.RegisterNotificationMethod(DidOpenTextDocumentNotification.Type, CallDidOpenTextDocument);
            rpcConnection.RegisterNotificationMethod(DidSaveTextDocumentNotification.Type, CallDidSaveTextDocument);

            RemoteConsole = new RemoteConsole(rpcConnection);
            RemoteWindow = new RemoteWindow(rpcConnection);
        }

        // RPC server used to send Remote Procedure Calls to the client
        public IRPCConnection rpcConnection { get; private set; }

        // --- Generic notification and request handlers ---

        private ResponseResultOrError CallCodeAction(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                List<Command> result = OnCodeAction((CodeActionParams)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch(Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallCodeLens(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                List<CodeLens> result = OnCodeLens((TextDocumentIdentifier)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallCodeLensResolve(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                CodeLens result =  OnCodeLensResolve((CodeLens)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallCompletion(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                IList<CompletionItem> result = OnCompletion((TextDocumentPosition)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallCompletionResolve(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                CompletionItem result = OnCompletionResolve((CompletionItem)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallDocumentHighlight(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                List<DocumentHighlight> result = OnDocumentHighlight((TextDocumentPosition)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallDocumentSymbol(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                List<SymbolInformation> result = OnDocumentSymbol((TextDocumentIdentifier)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallDocumentFormatting(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                List<TextEdit> result = OnDocumentFormatting((DocumentFormattingParams)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallDocumentOnTypeFormatting(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                List<TextEdit> result = OnDocumentOnTypeFormatting((DocumentOnTypeFormattingParams)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallDocumentRangeFormatting(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                List<TextEdit> result = OnDocumentRangeFormatting((DocumentRangeFormattingParams)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallDefinition(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                Definition result =  OnDefinition((TextDocumentPosition)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallHoverRequest(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                Hover result =  OnHover((TextDocumentPosition)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallInitialize(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                InitializeResult result = OnInitialize((InitializeParams)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message, data = new InitializeError() { retry = false } };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallReferences(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                List<Location> result = OnReferences((ReferenceParams)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallRename(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                WorkspaceEdit result = OnRename((RenameParams)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private bool shutdownReceived = false;

        private ResponseResultOrError CallShutdown(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                shutdownReceived = true;
                OnShutdown();
                resultOrError = new ResponseResultOrError() { result = null };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallSignatureHelp(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                SignatureHelp result = OnSignatureHelp((TextDocumentIdentifier)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private ResponseResultOrError CallWorkspaceSymbol(RequestType requestType, object parameters)
        {
            ResponseResultOrError resultOrError = null;
            try
            {
                List<SymbolInformation> result = OnWorkspaceSymbol((WorkspaceSymbolParams)parameters);
                resultOrError = new ResponseResultOrError() { result = result };
            }
            catch (Exception e)
            {
                resultOrError = new ResponseResultOrError() { code = (int)ErrorCodes.InternalError, message = e.Message };
            }
            return resultOrError;
        }

        private void CallDidChangeConfiguration(NotificationType notificationType, object parameters)
        {
            try
            {
                OnDidChangeConfiguration((DidChangeConfigurationParams)parameters);
            }
            catch(Exception e)
            {
                RemoteConsole.Error(String.Format("Error while handling notification {0} : {1}", notificationType.Method, e.Message));
            }
        }

        private void CallExit(NotificationType notificationType, object parameters)
        {
            try
            {
                OnExit();
            }
            catch (Exception e)
            {
                RemoteConsole.Error(String.Format("Error while handling notification {0} : {1}", notificationType.Method, e.Message));
            }
            finally
            {
                if (shutdownReceived)
                {
                    Environment.Exit(0);
                }
                else {
                    Environment.Exit(1);
                }
            }
        }

        private void CallDidChangeWatchedFiles(NotificationType notificationType, object parameters)
        {
            try
            {
                OnDidChangeWatchedFiles((DidChangeWatchedFilesParams)parameters);
            }
            catch (Exception e)
            {
                RemoteConsole.Error(String.Format("Error while handling notification {0} : {1}", notificationType.Method, e.Message));
            }
        }

        private void CallDidChangeTextDocument(NotificationType notificationType, object parameters)
        {
            try
            {
                OnDidChangeTextDocument((DidChangeTextDocumentParams)parameters);
            }
            catch (Exception e)
            {
                RemoteConsole.Error(String.Format("Error while handling notification {0} : {1}", notificationType.Method, e.Message));
                throw e;
            }
        }

        private void CallDidCloseTextDocument(NotificationType notificationType, object parameters)
        {
            try
            {
                OnDidCloseTextDocument((DidCloseTextDocumentParams)parameters);
            }
            catch (Exception e)
            {
                RemoteConsole.Error(String.Format("Error while handling notification {0} : {1}", notificationType.Method, e.Message));
            }
        }

        private void CallDidOpenTextDocument(NotificationType notificationType, object parameters)
        {
            try
            {
                OnDidOpenTextDocument((DidOpenTextDocumentParams)parameters);
            }
            catch (Exception e)
            {
                RemoteConsole.Error(String.Format("Error while handling notification {0} : {1}", notificationType.Method, e.Message));
            }
        }

        private void CallDidSaveTextDocument(NotificationType notificationType, object parameters)
        {
            try
            {
                OnDidSaveTextDocument((DidSaveTextDocumentParams)parameters);
            }
            catch (Exception e)
            {
                RemoteConsole.Error(String.Format("Error while handling notification {0} : {1}", notificationType.Method, e.Message));
            }
        }

        // --- Fully typed methods to overload in derived classes ---

        /// <summary>
        /// The initialize method is sent from the client to the server.
        /// It is send once as the first method after starting up the
        /// worker.The requests parameter is of type [InitializeParams](#InitializeParams)
        /// the response if of type [InitializeResult](#InitializeResult) of a Thenable that
        /// resolves to such.
        /// </summary>
        public virtual InitializeResult OnInitialize(InitializeParams parameters)
        {
            var capabilities = new ServerCapabilities();
            capabilities.textDocumentSync = TextDocumentSyncKind.None;
            capabilities.hoverProvider = false;
            capabilities.completionProvider = null;
            capabilities.signatureHelpProvider = null;
            capabilities.definitionProvider = false;
            capabilities.referencesProvider = false;
            capabilities.documentHighlightProvider = false;
            capabilities.documentSymbolProvider = false;
            capabilities.workspaceSymbolProvider = false;
            capabilities.codeActionProvider = false;
            capabilities.codeLensProvider = null;
            capabilities.documentFormattingProvider = false;
            capabilities.documentRangeFormattingProvider = false;
            capabilities.documentOnTypeFormattingProvider = null;
            capabilities.renameProvider = false;

            var result = new InitializeResult();
            result.capabilities = capabilities;
            return result;
        }

        /// <summary>
        /// A shutdown request is sent from the client to the server.
        /// It is send once when the client decides to shutdown the
        /// server.The only notification that is sent after a shudown request
        /// is the exit event.
        /// </summary>
        public virtual void OnShutdown()
        { }

        /// <summary>
        /// The exit event is sent from the client to the server to
        /// ask the server to exit its process.
        /// </summary>
        public virtual void OnExit()
        { }

        /// <summary>
        /// The log message notification is send from the server to the client to ask
        /// the client to log a particular message.
        /// </summary>
        public RemoteConsole RemoteConsole { get; private set; }

        /// <summary>
        /// The show message notification is sent from a server to a client to ask
        /// the client to display a particular message in the user interface.
        /// </summary>
        public RemoteWindow RemoteWindow { get; private set; }

        /// <summary>
        /// The configuration change notification is sent from the client to the server
        /// when the client's configuration has changed. The notification contains
        /// the changed configuration as defined by the language client.
        /// </summary>
        public virtual void OnDidChangeConfiguration(DidChangeConfigurationParams parameters)
        { }

        /// <summary>
        /// The watched files notification is sent from the client to the server when
        /// the client detects changes to file watched by the lanaguage client.
        /// </summary>
        public virtual void OnDidChangeWatchedFiles(DidChangeWatchedFilesParams parameters)
        { }

        /// <summary>
        /// The document open notification is sent from the client to the server to signal
        /// newly opened text documents.The document's truth is now managed by the client
        /// and the server must not try to read the document's truth using the document's
        /// uri.
        /// </summary>
        public virtual void OnDidOpenTextDocument(DidOpenTextDocumentParams parameters)
        { }

        /// <summary>
        /// The document change notification is sent from the client to the server to signal
        /// changes to a text document.
        /// </summary>
        public virtual void OnDidChangeTextDocument(DidChangeTextDocumentParams parameters)
        { }

        /// <summary>
        /// The document close notification is sent from the client to the server when
        /// the document got closed in the client.The document's truth now exists
        /// where the document's uri points to (e.g. if the document's uri is a file uri
        /// the truth now exists on disk).
        /// </summary>
        public virtual void OnDidCloseTextDocument(DidCloseTextDocumentParams parameters)
        { }

        /// <summary>
        /// The document save notification is sent from the client to the server when the document for saved in the client.
        /// </summary>
        public virtual void OnDidSaveTextDocument(DidSaveTextDocumentParams parameters)
        { }

        /// <summary>
        /// Diagnostics notification are sent from the server to the client to signal
        /// results of validation runs.
        /// </summary>
        public virtual void SendDiagnostics(PublishDiagnosticsParams parameters)
        {
            rpcConnection.SendNotification(PublishDiagnosticsNotification.Type, parameters);
        }

       

        /// <summary>
        /// Request to request hover information at a given text document position. The request's
        /// parameter is of type[TextDocumentPosition](#TextDocumentPosition) the response is of
        /// type[Hover](#Hover) or a Thenable that resolves to such.
        /// </summary>
        public virtual Hover OnHover(TextDocumentPosition parameters)
        {
            return null;
        }

        /// <summary>
        /// Request to request completion at a given text document position. The request's
        /// parameter is of type[TextDocumentPosition](#TextDocumentPosition) the response
        /// is of type[CompletionItem[]](#CompletionItem) or a Thenable that resolves to such.
        /// </summary>
        public virtual List<CompletionItem> OnCompletion(TextDocumentPosition parameters)
        {
            return null;
        }

        /// <summary>
        /// Request to resolve additional information for a given completion item.The request's
        /// parameter is of type[CompletionItem](#CompletionItem) the response
        /// is of type[CompletionItem](#CompletionItem) or a Thenable that resolves to such.
        /// </summary>
        public virtual CompletionItem OnCompletionResolve(CompletionItem parameters)
        {
            return null;
        }

        /// <summary>
        /// Signature help represents the signature of something
        /// callable. There can be multiple signature but only one
        /// active and only one active parameter.
        /// </summary>
        public virtual SignatureHelp OnSignatureHelp(TextDocumentIdentifier parameters)
        {
            return null;
        }

        /// <summary>
        /// A request to resolve the defintion location of a symbol at a given text
        /// document position.The request's parameter is of type [TextDocumentPosition]
        /// (#TextDocumentPosition) the response is of type [Definition](#Definition) or a
        /// Thenable that resolves to such.
        /// </summary>
        public virtual Definition OnDefinition(TextDocumentPosition parameters)
        {
            throw new ArgumentException("No definition");            
        }

        /// <summary>
        /// A request to resolve project-wide references for the symbol denoted
        /// by the given text document position.The request's parameter is of
        /// type[ReferenceParams](#ReferenceParams) the response is of type
        /// [Location[]](#Location) or a Thenable that resolves to such.
        /// </summary>
        public virtual List<Location> OnReferences(ReferenceParams parameters)
        {
            return null;
        }

        /// <summary>
        /// Request to resolve a [DocumentHighlight](#DocumentHighlight) for a given
        /// text document position.The request's parameter is of type [TextDocumentPosition]
        /// (#TextDocumentPosition) the request reponse is of type [DocumentHighlight[]]
        /// (#DocumentHighlight) or a Thenable that resolves to such.
        /// </summary>
        public virtual List<DocumentHighlight> OnDocumentHighlight(TextDocumentPosition parameters)
        {
            return null;
        }

        /// <summary>
        /// A request to list all symbols found in a given text document. The request's
        /// parameter is of type[TextDocumentIdentifier](#TextDocumentIdentifier) the
        /// response is of type[SymbolInformation[]](#SymbolInformation) or a Thenable
        /// that resolves to such.
        /// </summary>
        public virtual List<SymbolInformation> OnDocumentSymbol(TextDocumentIdentifier parameters)
        {
            return null;
        }

        /// <summary>
        /// A request to list project-wide symbols matching the query string given
        /// by the[WorkspaceSymbolParams](#WorkspaceSymbolParams). The response is
        /// of type[SymbolInformation[]](#SymbolInformation) or a Thenable that
        /// resolves to such.
        /// </summary>
        public virtual List<SymbolInformation> OnWorkspaceSymbol(WorkspaceSymbolParams parameters)
        {
            return null;
        }

        /// <summary>
        /// A request to provide commands for the given text document and range.
        /// </summary>
        public virtual List<Command> OnCodeAction(CodeActionParams parameters)
        {
            return null;
        }

        /// <summary>
        /// A request to provide code lens for the given text document.
        /// </summary>
        public virtual List<CodeLens> OnCodeLens(TextDocumentIdentifier parameters)
        {
            return null;
        }

        /// <summary>
        /// A request to resolve a command for a given code lens.
        /// </summary>
        public virtual CodeLens OnCodeLensResolve(CodeLens parameters)
        {
            return null;
        }

        /// <summary>
        /// A request to to format a whole document.
        /// </summary>
        public virtual List<TextEdit> OnDocumentFormatting(DocumentFormattingParams parameters)
        {
            return null;
        }

        /// <summary>
        /// A request to to format a range in a document.
        /// </summary>
        public virtual List<TextEdit> OnDocumentRangeFormatting(DocumentRangeFormattingParams parameters)
        {
            return null;
        }

        /// <summary>
        /// A request to format a document on type.
        /// </summary>
        public virtual List<TextEdit> OnDocumentOnTypeFormatting(DocumentOnTypeFormattingParams parameters)
        {
            return null;
        }

        /// <summary>
        /// A request to rename a symbol.
        /// </summary>
        public virtual WorkspaceEdit OnRename(RenameParams parameters)
        {
            return null;
        }
    }
}
