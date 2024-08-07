﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TypeCobol.LanguageServer.JsonRPC;
using TypeCobol.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;

namespace TypeCobol.LanguageServer.Robot.Common.Utilities
{
    /// <summary>
    /// Utility class for the Language Server Protocol.
    /// </summary>
    public class Protocol
    {
        /// <summary>
        /// The Kinds of JSon message
        /// </summary>
        public enum Message_Kind
        {
            Notification,
            Request,
            Response,
            Unknown
        }

        /// <summary>
        /// The delegate for a RequestType or NotificationType which parameters contains an uri information.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="jsonObject"></param>
        /// <param name="parameters">The data or parameters of the jsonObject Notification or Request.</param>
        /// <returns></returns>
        public delegate string UriExtractor<T>(T type, JObject jsonObject, out object data_params);
        /// <summary>
        /// The Dictionnary that maps Notification methods whose type contains an URI to their Uri extractors.
        /// </summary>
        private static Dictionary<string, Tuple<NotificationType, UriExtractor<NotificationType>>> NotificationsWithUriMap;
        /// <summary>
        /// The Dictionnary that maps Requests methods whose type contains an URI to their Uri extractors.
        /// </summary>
        private static Dictionary<string, Tuple<RequestType, UriExtractor<RequestType>>> RequestsWithUriMap;

        /// <summary>
        /// A Default Initialization message.
        /// </summary>
        public static readonly string DEFAULT_INITIALIZE = "{\"jsonrpc\":\"2.0\",\"id\":\"0\",\"method\":\"initialize\",\"params\":{\"processId\":-1,\"rootPath\":\"C:\\\\Program Files (x86)\\\\IBM\\\\SDP_IDZ14\",\"rootUri\":\"file:/C:/Program%20Files%20(x86)/IBM/SDP_IDZ14/\",\"capabilities\":{\"workspace\":{\"applyEdit\":true,\"didChangeConfiguration\":{\"dynamicRegistration\":true},\"didChangeWatchedFiles\":{\"dynamicRegistration\":false},\"symbol\":{\"dynamicRegistration\":true},\"executeCommand\":{\"dynamicRegistration\":true}},\"textDocument\":{\"synchronization\":{\"willSave\":true,\"willSaveWaitUntil\":true,\"dynamicRegistration\":true},\"completion\":{\"completionItem\":{\"snippetSupport\":true},\"dynamicRegistration\":true},\"hover\":{\"dynamicRegistration\":true},\"signatureHelp\":{\"dynamicRegistration\":true},\"references\":{\"dynamicRegistration\":true},\"documentHighlight\":{\"dynamicRegistration\":true},\"documentSymbol\":{\"dynamicRegistration\":true},\"formatting\":{\"dynamicRegistration\":true},\"rangeFormatting\":{\"dynamicRegistration\":true},\"onTypeFormatting\":{\"dynamicRegistration\":true},\"definition\":{\"dynamicRegistration\":true},\"codeAction\":{\"dynamicRegistration\":true},\"codeLens\":{\"dynamicRegistration\":true},\"documentLink\":{\"dynamicRegistration\":true},\"rename\":{\"dynamicRegistration\":true}}},\"trace\":\"off\"}}";

        /// <summary>
        /// Default initialized notification
        /// </summary>
        public static readonly string DEFAULT_INITIALIZED = "{\"jsonrpc\":\"2.0\",\"method\":\"initialized\"}";
        /// <summary>
        /// Default shutdown message
        /// </summary>
        public static readonly string DEFAULT_SHUTDOWN = "{\"jsonrpc\":\"2.0\",\"id\":\"2\",\"method\":\"shutdown\"}";
        /// <summary>
        /// Default exit message
        /// </summary>
        public static readonly string DEFAULT_EXIT = "{\"jsonrpc\":\"2.0\",\"method\":\"exit\"}";

        /// <summary>
        /// Static constructor
        /// </summary>
        static Protocol()
        {
            //-----------------------------------------------------------------
            //Construct the map of Notifications that are associated to an URI.
            //-----------------------------------------------------------------
            NotificationsWithUriMap = new Dictionary<string, Tuple<NotificationType, UriExtractor<NotificationType>>>();

            //Text Document Notifications
            NotificationsWithUriMap.Add(DidOpenTextDocumentNotification.Type.Method, new Tuple<NotificationType, UriExtractor<NotificationType> >(
                    DidOpenTextDocumentNotification.Type,
                    (NotificationType type, JObject jsonObject, out object data_params) => {
                        JToken parameters = jsonObject[String.Intern("params")];
                        data_params = null;
                        if (parameters == null)
                            return null;
                        DidOpenTextDocumentParams data = (DidOpenTextDocumentParams)(data_params = parameters.ToObject(type.ParamsType));
                        return data == null ? null : (data.textDocument == null ? null :data.textDocument.uri);
                    }
                )
            );
            NotificationsWithUriMap.Add(DidChangeTextDocumentNotification.Type.Method, new Tuple<NotificationType, UriExtractor<NotificationType>>(
                    DidChangeTextDocumentNotification.Type,
                    (NotificationType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        DidChangeTextDocumentParams data = (DidChangeTextDocumentParams)parameters.ToObject(type.ParamsType);
                        return data == null ? null : data.uri;
                    }
                )
            );

            NotificationsWithUriMap.Add(DidSaveTextDocumentNotification.Type.Method, new Tuple<NotificationType, UriExtractor<NotificationType>>(
                    DidSaveTextDocumentNotification.Type,
                    (NotificationType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        DidSaveTextDocumentParams data = (DidSaveTextDocumentParams)parameters.ToObject(type.ParamsType);
                        return data == null ? null : (data.textDocument == null ? null : data.textDocument.uri);
                    }
                )
            );

            NotificationsWithUriMap.Add(DidCloseTextDocumentNotification.Type.Method, new Tuple<NotificationType, UriExtractor<NotificationType>>(
                    DidCloseTextDocumentNotification.Type,
                    (NotificationType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        DidCloseTextDocumentParams data = (DidCloseTextDocumentParams)parameters.ToObject(type.ParamsType);
                        return data == null ? null : (data.textDocument == null ? null : data.textDocument.uri);
                    }
                )
            );

            //Diagnostic Notification
            NotificationsWithUriMap.Add(PublishDiagnosticsNotification.Type.Method, new Tuple<NotificationType, UriExtractor<NotificationType>>(
                    PublishDiagnosticsNotification.Type,
                    (NotificationType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        PublishDiagnosticsParams data = (PublishDiagnosticsParams)parameters.ToObject(type.ParamsType);
                        return data == null ? null : data.uri;
                    }
                )
            );

            //-----------------------------------------------------------------
            //Construct the map of Requests that are associated to an URI.
            //-----------------------------------------------------------------
            RequestsWithUriMap = new Dictionary<string, Tuple<RequestType, UriExtractor<RequestType>>>();
            RequestsWithUriMap.Add(CodeActionRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    CodeActionRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        CodeActionParams data = (CodeActionParams)parameters.ToObject(type.ParamsType);
                        return data == null ? null : (data.textDocument == null ? null : data.textDocument.uri);
                    }
                )
            );

            RequestsWithUriMap.Add(CodeLensRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    CodeLensRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        TextDocumentIdentifier data = (TextDocumentIdentifier)parameters.ToObject(type.ParamsType);
                        return data == null ? null : data.uri;
                    }
                )
            );

            RequestsWithUriMap.Add(CompletionRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    CompletionRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        TextDocumentPosition data = (TextDocumentPosition)parameters.ToObject(type.ParamsType);
                        return data == null ? null : data.uri;
                    }
                )
            );

            RequestsWithUriMap.Add(DocumentHighlightRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    DocumentHighlightRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        TextDocumentPosition data = (TextDocumentPosition)parameters.ToObject(type.ParamsType);
                        return data == null ? null : data.uri;
                    }
                )
            );

            RequestsWithUriMap.Add(DocumentSymbolRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    DocumentSymbolRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        TextDocumentIdentifier data = (TextDocumentIdentifier)parameters.ToObject(type.ParamsType);
                        return data == null ? null : data.uri;
                    }
                )
            );

            //Document Formating Request
            RequestsWithUriMap.Add(DocumentFormattingRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    DocumentFormattingRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        DocumentFormattingParams data = (DocumentFormattingParams)parameters.ToObject(type.ParamsType);
                        return data == null ? null : (data.textDocument == null ? null : data.textDocument.uri);
                    }
                )
            );

            RequestsWithUriMap.Add(DocumentRangeFormattingRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    DocumentRangeFormattingRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        DocumentRangeFormattingParams data = (DocumentRangeFormattingParams)parameters.ToObject(type.ParamsType);
                        return data == null ? null : (data.textDocument == null ? null : data.textDocument.uri);
                    }
                )
            );

            RequestsWithUriMap.Add(DocumentOnTypeFormattingRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    DocumentOnTypeFormattingRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        DocumentOnTypeFormattingParams data = (DocumentOnTypeFormattingParams)parameters.ToObject(type.ParamsType);
                        return data == null ? null : (data.textDocument == null ? null : data.textDocument.uri);
                    }
                )
            );

            //Goto Definition
            RequestsWithUriMap.Add(DefinitionRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    DefinitionRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        TextDocumentPosition data = (TextDocumentPosition)parameters.ToObject(type.ParamsType);
                        return data == null ? null : data.uri;
                    }
                )
            );

            //Hover Request
            RequestsWithUriMap.Add(HoverRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    HoverRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        TextDocumentPosition data = (TextDocumentPosition)parameters.ToObject(type.ParamsType);
                        return data == null ? null : data.uri;
                    }
                )
            );

            //Reference Request
            RequestsWithUriMap.Add(ReferencesRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    ReferencesRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        TextDocumentPosition data = (TextDocumentPosition)parameters.ToObject(type.ParamsType);
                        return data == null ? null : data.uri;
                    }
                )
            );

            //Rename
            RequestsWithUriMap.Add(RenameRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    RenameRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        RenameParams data = (RenameParams)parameters.ToObject(type.ParamsType);
                        return data == null ? null : (data.textDocument == null ? null : data.textDocument.uri);
                    }
                )
            );

            //Signature Help
            RequestsWithUriMap.Add(SignatureHelpRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    SignatureHelpRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null)
                            return null;
                        TextDocumentPosition data = (TextDocumentPosition)parameters.ToObject(type.ParamsType);
                        return data == null ? null : data.uri;
                    }
                )
            );

            //Workspace Apply Edit
            RequestsWithUriMap.Add(WorkspaceApplyEditRequest.Type.Method, new Tuple<RequestType, UriExtractor<RequestType>>(
                    WorkspaceApplyEditRequest.Type,
                    (RequestType type, JObject jsonObject, out object data_params) => {
                        data_params = null;
                        JToken parameters = jsonObject[String.Intern("params")];
                        ApplyWorkspaceEditParams applyWorkspaceEditParams = parameters.ToObject<ApplyWorkspaceEditParams>();
                        return applyWorkspaceEditParams?.edit?.changes?.FirstOrDefault().Key; // TODO Handle requests affecting multiple files !
                    }
                )
            );
        }

        /// <summary>
        /// Get the Json representation of a message
        /// </summary>
        /// <param name="message">The message to get the Json representation</param>
        /// <returns>The JObject instanceof representing the message if any, null otherwise.</returns>
        public static JObject ToJson(string message)
        {
            JObject jsonObject = message != null ? JObject.Parse(message) : null;
            return jsonObject;
        }

        /// <summary>
        /// Get the kind of message corresponding to the given message.
        /// </summary>
        /// <param name="message">The message's to get the kind.</param>
        /// <param name="jsonObject">{out] the Json object corresponding to the kind.</param>
        /// <returns>The MessageKind enumerate value</returns>
        public static Message_Kind MessageKind(string message, out JObject jsonObject)
        {
            return MessageKind(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Get the kind of message corresponding to the given Json object
        /// </summary>
        /// <param name="jsonObject">The Json object to get the message kind.</param>
        /// <returns>The MessageKind enumerate value</returns>
        public static Message_Kind MessageKind(JObject jsonObject)
        {
            if (jsonObject != null)
            {
                string requestId = (string)jsonObject[String.Intern("id")];
                string method = (string)jsonObject[String.Intern("method")];

                // Check message type
                // -- Notification --
                if (requestId == null && method != null)
                {
                    return Message_Kind.Notification;
                }
                // -- Request --
                if (requestId != null && method != null)
                {
                    return Message_Kind.Request;
                }
                JToken result = jsonObject[String.Intern("result")];
                JToken error = jsonObject[String.Intern("error")];
                // -- Response --
                if (requestId != null && (result != null || error != null))
                {
                    return Message_Kind.Response;
                }
            }
            return Message_Kind.Unknown;
        }

        /// <summary>
        /// Determines if the given message correspond to a Request message.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the corresponding Json Object if any, null otherwise</param>
        /// <returns>true if the message is a request message, false otherwise</returns>
        public static bool IsRequest(string message, out JObject jsonObject)
        {            
            return IsRequest(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine is the given JSon object correspond to a request.
        /// </summary>
        /// <param name="jsonObject">The Json Object to check</param>
        /// <returns>true if the Json object is a request, false otherwise.</returns>
        public static bool IsRequest(JObject jsonObject)
        {
            return MessageKind(jsonObject) == Message_Kind.Request;
        }

        /// <summary>
        /// Determines if the given message correspond to a notification message.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the corresponding Json Object if any, null otherwise</param>
        /// <returns>true if the message is a notification message, false otherwise</returns>
        public static bool IsNotification(string message, out JObject jsonObject)
        {
            return IsNotification(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine is the given JSon object correspond to a notification.
        /// </summary>
        /// <param name="jsonObject">The Json Object to check</param>
        /// <returns>true if the Json object is a notification, false otherwise.</returns>
        public static bool IsNotification(JObject jsonObject)
        {
            return MessageKind(jsonObject) == Message_Kind.Notification;
        }

        /// <summary>
        /// Determines if the given message correspond to a response message.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the corresponding Json Object if any, null otherwise</param>
        /// <returns>true if the message is a response message, false otherwise</returns>
        public static bool IsResponse(string message, out JObject jsonObject)
        {
            return IsResponse(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine fs the given JSon object correspond to a response.
        /// </summary>
        /// <param name="jsonObject">The Json Object to check</param>
        /// <returns>true if the Json object is a response, false otherwise.</returns>
        public static bool IsResponse(JObject jsonObject)
        {
            return MessageKind(jsonObject) == Message_Kind.Response;
        }

        public static bool IsErrorResponse(JObject jsonObject)
        {
            System.Diagnostics.Contracts.Contract.Requires(IsResponse(jsonObject));            
            return jsonObject[String.Intern("error")] != null;
        }

        /// <summary>
        /// Determines if the given message correspond to a response message that is not an error.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the corresponding Json Object if any, null otherwise</param>
        /// <returns>true if the message is a response message that is not an error, false otherwise</returns>
        public static bool IsResponseAndNotError(string message, out JObject jsonObject)
        {
            return IsResponseAndNotError(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine if the given jsonObject represents a response that is not an error.
        /// </summary>
        /// <param name="jsonObject">The Json object to check</param>
        /// <returns>true if the given Json Object is a response that is not an error, false otherwise.</returns>
        public static bool IsResponseAndNotError(JObject jsonObject)
        {            
            return IsResponse(jsonObject) && jsonObject[String.Intern("error")] == null;
        }

        /// <summary>
        /// Get the Id corresponding toa JSON request or response.
        /// </summary>
        /// <param name="jsonObject">The JSon request or response</param>
        /// <returns>The Request or Response Id.</returns>
        public static string GetRequestId(JObject jsonObject)
        {
            System.Diagnostics.Contracts.Contract.Requires(IsRequest(jsonObject) || IsResponse(jsonObject));
            string requestId = (string)jsonObject[String.Intern("id")];
            return requestId;
        }

        /// <summary>
        /// Get the Id corresponding toa JSON notification, request or response.
        /// </summary>
        /// <param name="jsonObject">The JSon notification, request or response</param>
        /// <returns>The Notification, Request or Response method.</returns>
        public static string GetMessageMethod(JObject jsonObject)
        {
            System.Diagnostics.Contracts.Contract.Requires(IsNotification(jsonObject) || IsRequest(jsonObject) || IsResponse(jsonObject));
            string method = (string)jsonObject[String.Intern("method")];
            return method;
        }

        /// <summary>
        /// Get the result of a response
        /// </summary>
        /// <param name="jsonObject">The JSOn object of the response</param>
        /// <returns>The result if any, null otherwise.</returns>
        public static JToken GetResponseResult(JObject jsonObject)
        {
            System.Diagnostics.Contracts.Contract.Requires(IsResponse(jsonObject));
            JToken result = (string)jsonObject[String.Intern("result")];
            return result;
        }

        /// <summary>
        /// Get the error object of a response
        /// </summary>
        /// <param name="jsonObject">The JSOn object of the response</param>
        /// <returns>The error object if any, null otherwise.</returns>
        public static JToken GetResponseError(JObject jsonObject)
        {
            System.Diagnostics.Contracts.Contract.Requires(IsResponse(jsonObject));
            JToken error = jsonObject[String.Intern("error")];
            return error;
        }

        /// <summary>
        /// Determine if the given message is a message associated to an uri.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="uri">[out] the message's uri if any, null otherwise</param>
        /// <param name="jsonObject"></param>
        /// <returns>True if the message is a message with an uri, false otherwise</returns>
        public static bool IsMessageWithUri(string message, out string uri, out JObject jsonObject)
        {//So basically we must maintains the list of all Request and Notification that target
            return IsMessageWithUri(jsonObject = ToJson(message), out uri);
        }

        /// <summary>
        /// Determine if the given Json object correspond to message that is associated to an uri.
        /// </summary>
        /// <param name="jsonObject">The Json object to check</param>
        /// <param name="uri">[out] The associate duri if any, null otherwise</param>
        /// <returns>true if the Json object is associated to an uri, false otherwise.</returns>
        public static bool IsMessageWithUri(JObject jsonObject, out string uri)
        {
            bool bResult = false;
            uri = null;
            if (jsonObject != null)
            {
                Message_Kind kind = MessageKind(jsonObject);
                switch(kind)
                {
                    case Message_Kind.Notification:
                        {
                            string method = GetMessageMethod(jsonObject);
                            bResult = NotificationsWithUriMap.ContainsKey(method);
                            if (bResult)
                            {
                                object parameter = null;
                                uri = NotificationsWithUriMap[method].Item2(NotificationsWithUriMap[method].Item1, jsonObject, out parameter);
                            }
                        }
                        break;                     
                    case Message_Kind.Request:
                        {
                            string method = GetMessageMethod(jsonObject);
                            bResult = RequestsWithUriMap.ContainsKey(method);
                            if (bResult)
                            {
                                object parameter = null;
                                uri = RequestsWithUriMap[method].Item2(RequestsWithUriMap[method].Item1, jsonObject, out parameter);
                            }
                        }
                        break;
                    default:
                        break;
                }
                try
                {
                    if (!bResult && (kind == Message_Kind.Notification || kind == Message_Kind.Request))
                    {//Check if we have a json object with a "params" field whose first child is a TextDocumentIdentifier
                        JToken parameters = jsonObject[String.Intern("params")];
                        if (parameters == null) return bResult;
                        JToken textDocField = parameters[String.Intern("textDocument")];
                        if (textDocField == null) return bResult;
                        var txtDocId = (TextDocumentIdentifier)textDocField.ToObject(typeof(TextDocumentIdentifier));
                        uri = txtDocId?.uri;
                        bResult = uri != null;
                    }
                }
                catch (Exception e )
                {//Ignore this exception the attempt has failed
                    //Console.WriteLine(e);
                }
            }
            return bResult;
        }

        /// <summary>
        /// Determine if the given message represents a LanguageServer "initialize" request.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the parsed JSon Object if any, null otherwise</param>
        /// <returns>true if the message is a initialize message, false otherwise.</returns>
        public static bool IsInitializeRequest(string message, out JObject jsonObject)
        {
            return IsInitializeRequest(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine if the given JSon objects corresponds to the "initialize" request.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static bool IsInitializeRequest(JObject jsonObject)
        {
            if (IsRequest(jsonObject))
            {
                return ((string)jsonObject[String.Intern("method")]).Equals(LanguageServer.Protocol.InitializeRequest.Type.Method);
            }
            return false;
        }

        /// <summary>
        /// Determines if the given message corresponds to the workspace/didChangeConfiguration notification.
        /// </summary>
        /// <param name="message">The message to be checked</param>
        /// <param name="jsonObject">[out] The json object corresponding to the message if any</param>
        /// <returns>true if yes, false otherwise</returns>
        public static bool IsDidChangeConfigurationNotification(string message, out JObject jsonObject)
        {
            return IsDidChangeConfigurationNotification(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determines if the given Json object corresponds to the workspace/didChangeConfiguration notification.
        /// </summary>
        /// <param name="jsonObject">The Json object to be checked</param>
        /// <returns>true if yes, false otherwise</returns>
        public static bool IsDidChangeConfigurationNotification(JObject jsonObject)
        {
            if (IsNotification(jsonObject))
            {
                return ((string)jsonObject[String.Intern("method")]).Equals(LanguageServer.Protocol.DidChangeConfigurationNotification.Type.Method);
            }
            return false;
        }
        
        /// <summary>
        /// Determine if the given message represents a LanguageServer "initialized" notification.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the parsed JSon Object if any, null otherwise</param>
        /// <returns>true if the message is a initialized notification, false otherwise.</returns>
        public static bool IsInitializedNotification(string message, out JObject jsonObject)
        {
            return IsInitializedNotification(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine if the given JSon objects corresponds to the "initialized" notification.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static bool IsInitializedNotification(JObject jsonObject)
        {
            if (IsNotification(jsonObject))
            {
                return ((string)jsonObject[String.Intern("method")]).Equals("initialized");
            }
            return false;
        }

        /// <summary>
        /// Determine if the given message represents a LanguageServer "shutdown" request.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the parsed JSon Object if any, null otherwise</param>
        /// <returns>true if the message is a shutdown message, false otherwise.</returns>
        public static bool IsShutdownRequest(string message, out JObject jsonObject)
        {
            return IsShutdownRequest(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine if the given JSon object corresponds to the "shutdown" request.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns>true if the Json object is a shutdown message, false otherwise.</returns>
        public static bool IsShutdownRequest(JObject jsonObject)
        {
            if (IsRequest(jsonObject))
            {
                return ((string)jsonObject[String.Intern("method")]).Equals(LanguageServer.Protocol.ShutdownRequest.Type.Method);
            }
            return false;
        }

        /// <summary>
        /// Determine if the given message represents a LanguageServer "exit" notification.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the parsed JSon Object if any, null otherwise</param>
        /// <returns>true if the message is an exit notification, false otherwise.</returns>
        public static bool IsExitNotification(string message, out JObject jsonObject)
        {
            return IsExitNotification(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine if the given JSon object corresponds to the "exit" notification.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns>true if the Json object is an exit notification, false otherwise.</returns>
        public static bool IsExitNotification(JObject jsonObject)
        {
            if (IsNotification(jsonObject))
            {
                return ((string)jsonObject[String.Intern("method")]).Equals(LanguageServer.Protocol.ExitNotification.Type.Method);
            }
            return false;
        }

        /// <summary>
        /// Determine if the given message represents a LanguageServer "textDocument/didOpen" notification.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the parsed JSon Object if any, null otherwise</param>
        /// <returns>true if the message is a "textDocument/didOpen" message, false otherwise.</returns>
        public static bool IsDidOpenTextDocumentNotification(string message, out JObject jsonObject)
        {
            return IsDidOpenTextDocumentNotification(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine if the given JSon objects corresponds to the "textDocument/didOpen" request.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static bool IsDidOpenTextDocumentNotification(JObject jsonObject)
        {
            if (IsNotification(jsonObject))
            {
                return ((string)jsonObject[String.Intern("method")]).Equals(LanguageServer.Protocol.DidOpenTextDocumentNotification.Type.Method);
            }
            return false;

        }

        /// <summary>
        /// Determine if the given message represents a LanguageServer "textDocument/didClose" notification.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the parsed JSon Object if any, null otherwise</param>
        /// <returns>true if the message is a "textDocument/didClose" message, false otherwise.</returns>
        public static bool IsDidCloseTextDocumentNotification(string message, out JObject jsonObject)
        {
            return IsDidCloseTextDocumentNotification(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine if the given JSon objects corresponds to the "textDocument/didClose" request.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static bool IsDidCloseTextDocumentNotification(JObject jsonObject)
        {
            if (IsNotification(jsonObject))
            {
                return ((string)jsonObject[String.Intern("method")]).Equals(LanguageServer.Protocol.DidCloseTextDocumentNotification.Type.Method);
            }
            return false;
        }

        /// <summary>
        /// Determine if the given message represents a LanguageServer "textDocument/didSave" notification.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the parsed JSon Object if any, null otherwise</param>
        /// <returns>true if the message is a "textDocument/didSave" message, false otherwise.</returns>
        public static bool IsDidSaveTextDocumentNotification(string message, out JObject jsonObject)
        {
            return IsDidSaveTextDocumentNotification(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine if the given JSon objects corresponds to the "textDocument/didSave" request.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns>true if yes, false otherwise</returns>
        public static bool IsDidSaveTextDocumentNotification(JObject jsonObject)
        {
            if (IsNotification(jsonObject))
            {
                return ((string)jsonObject[String.Intern("method")]).Equals(LanguageServer.Protocol.DidSaveTextDocumentNotification.Type.Method);
            }
            return false;
        }

        /// <summary>
        /// Determine if the given message represents a LanguageServer "textDocument/didChange" notification.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the parsed JSon Object if any, null otherwise</param>
        /// <returns>true if the message is a "textDocument/didSave" message, false otherwise.</returns>
        public static bool IsDidChangeTextDocumentNotification(string message, out JObject jsonObject)
        {
            return IsDidChangeTextDocumentNotification(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine if the given JSon objects corresponds to the "textDocument/didChange" request.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns>true if yes, false otherwise</returns>
        public static bool IsDidChangeTextDocumentNotification(JObject jsonObject)
        {
            if (IsNotification(jsonObject))
            {
                return ((string)jsonObject[String.Intern("method")]).Equals(LanguageServer.Protocol.DidChangeTextDocumentNotification.Type.Method);
            }
            return false;
        }

        /// <summary>
        /// Determine if the given message represents a LanguageServer "window/showMessage" notification.
        /// </summary>
        /// <param name="message">The message to check</param>
        /// <param name="jsonObject">[out] the parsed JSon Object if any, null otherwise</param>
        /// <returns>true if the message is a "window/showMessage" notification, false otherwise.</returns>
        public static bool IsShowMessageNotification(string message, out JObject jsonObject)
        {
            return IsShowMessageNotification(jsonObject = ToJson(message));
        }

        /// <summary>
        /// Determine if the given JSon object corresponds to the "window/showMessage" notification.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns>true if the Json object is a "window/showMessage" notification, false otherwise.</returns>
        public static bool IsShowMessageNotification(JObject jsonObject)
        {
            if (IsNotification(jsonObject))
            {
                return ((string)jsonObject[String.Intern("method")]).Equals(LanguageServer.Protocol.ShowMessageNotification.Type.Method);
            }
            return false;
        }

        /// <summary>
        /// Normalize an uri to match the C# string representation of a File
        /// </summary>
        /// <param name="uri">The file uri to normalize.</param>
        /// <returns>the normalized uri if succesfull, the original uri otherwise.</returns>
        public static String NormalizeFileURI(String uri)
        {
            try
            {// Normalize the uri to match the Java representation of an URI
                System.Uri new_uri = null;
                new_uri = new System.Uri(uri);
                string path = new_uri.AbsolutePath;
                System.Uri file_uri = (new System.UriBuilder(new_uri)).Uri;
                return file_uri.ToString();
            }
            catch (Exception /*e*/)
            {
            }
            return uri;
        }

        /// <summary>
        /// Load a file which shall contains an LSP initialize request.
        /// </summary>
        /// <param name="initializeFile">The json initialze request file</param>
        /// <param name="initializeMessage">[out]The json initialze request as string</param>
        /// <param name="jsonMessage">[out]The json initialze request as a json object</param>
        /// <param name="exc">[out]Any Exception that might occured</param>
        /// <returns>true if the file contains an initialize message, false otherwise</returns>
        public static bool LoadInitializeRequest(string initializeFile, out string initializeMessage, out JObject jsonMessage, out Exception exc)
        {
            System.Diagnostics.Debug.Assert(initializeFile != null);
            exc = null;
            initializeMessage = null;
            jsonMessage = null;
            {//Read the file using UTF8.
                try
                {
                    using (FileStream fs = new FileStream(initializeFile, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                        {
                            initializeMessage = sr.ReadToEnd();
                            jsonMessage = JObject.Parse(initializeMessage);
                            return IsInitializeRequest(initializeMessage, out jsonMessage);
                        }
                    }
                }
                catch (Exception e)
                {
                    exc = e;
                }
            }
            return false;
        }

        /// <summary>
        /// Load a file which shall contains an LSP workspace/didChangeConfiguration notification.
        /// </summary>
        /// <param name="configFile">The json configuration request file</param>
        /// <param name="configMessage">[out]The json configuration request as string</param>
        /// <param name="jsonMessage">[out]The json configuration request as a json object</param>
        /// <param name="exc">[out]Any Exception that might occured</param>
        /// <returns>true if the file contains an configuration message, false otherwise</returns>
        public static bool LoadConfigurationNotification(string configFile, out string configMessage, out JObject jsonMessage, out Exception exc)
        {
            System.Diagnostics.Debug.Assert(configFile != null);
            exc = null;
            configMessage = null;
            jsonMessage = null;
            {//Read the file using UTF8.
                try
                {
                    using (FileStream fs = new FileStream(configFile, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                        {
                            configMessage = sr.ReadToEnd();
                            jsonMessage = JObject.Parse(configMessage);
                            return IsDidChangeConfigurationNotification(configMessage, out jsonMessage);
                        }
                    }
                }
                catch (Exception e)
                {
                    exc = e;
                }
            }
            return false;
        }

    }
}
