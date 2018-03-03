using System;
using TypeCobol.LanguageServer.JsonRPC;

namespace TypeCobol.LanguageServer.Protocol
{
    /// <summary>
    /// The RemoteConsole interface contains all functions to interact with
    /// the developer console of VS Code.
    /// </summary>
    public class RemoteConsole
    {
        private IRPCConnection rpcConnection;

        public RemoteConsole(IRPCConnection rpcConnection)
        {
            this.rpcConnection = rpcConnection;
        }

        /// <summary>
        /// Show an error message.
        /// 
        /// @param message The message to show.
        /// </summary>
        public void Error(string message)
        {
            send(MessageType.Error, message);
        }

        /// <summary>
        /// Show an warning message.
        /// 
        /// @param message The message to show.
        /// </summary>
        public void Warn(string message)
        {
            send(MessageType.Warning, message);
        }

        /// <summary>
        /// Log a message.
        /// 
        /// @param message The message to log.
        /// </summary>
        public void Log(string message)
        {
            send(MessageType.Log, message);
        }

        private void send(MessageType type, string message)
        {
            rpcConnection.SendNotification(LogMessageNotification.Type, new LogMessageParams() { type = type, message = message });
        }
    }   
}
