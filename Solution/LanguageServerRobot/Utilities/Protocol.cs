using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;
using LanguageServer.Protocol;
using Newtonsoft.Json.Linq;

namespace LanguageServerRobot.Utilities
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

            RequestsWithUriMap = new Dictionary<string, Tuple<RequestType, UriExtractor<RequestType>>>();
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
                string requestId = (string)jsonObject["id"];
                string method = (string)jsonObject["method"];

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
                JToken result = jsonObject["result"];
                JToken error = jsonObject["error"];
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
            return MessageKind(jsonObject) == Message_Kind.Response;
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
        /// Determine is the given JSon object correspond to a response.
        /// </summary>
        /// <param name="jsonObject">The Json Object to check</param>
        /// <returns>true if the Json object is a response, false otherwise.</returns>
        public static bool IsResponse(JObject jsonObject)
        {
            return MessageKind(jsonObject) == Message_Kind.Response;
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
                return jsonObject[String.Intern("method")].Equals(LanguageServer.Protocol.InitializeRequest.Type.Method);
            }
            return false;
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
            uri = null;
            if (jsonObject != null)
            {

            }
            return false;
        }

    }
}
