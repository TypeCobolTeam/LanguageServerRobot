using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    public interface IRPCConnection
    {
        /// <summary>
        /// Register a description of all the notification methods supported by the RPC connection
        /// </summary>
        void RegisterNotificationMethod(NotificationType notificationType, NotificationHandler notificationHandler);

        /// <summary>
        /// Register a description of all the request methods supported by the RPC connection
        /// </summary>
        void RegisterRequestMethod(RequestType requestType, RequestHandler requestHandler);

        /// <summary>
        /// Send a notification to the client
        /// </summary>
        void SendNotification(NotificationType notificationType, object parameters);

        /// <summary>
        /// Send an async request to the client and await for the response or error
        /// </summary>
        Task<ResponseResultOrError> SendRequest(RequestType requestType, object parameters);

        /// <summary>
        /// Write a trace in the connection log file
        /// </summary>
        void WriteConnectionLog(string trace);
    }
}
