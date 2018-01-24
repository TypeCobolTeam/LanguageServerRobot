using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.Robot.Common.Model
{
    /// <summary>
    /// A monotoring message
    /// </summary>
    [Serializable]
    public abstract class Message
    {
        [Serializable]
        public enum MessageKind
        {
            //A LSR or LSRM Command
            Command,
            //A LSP message:
            Lsp,
            //A Message Acknowledgment
            Acknowledgment
        }

        /// <summary>
        /// Message Kind constructor
        /// </summary>
        /// <param name="kind">Message's kind</param>
        protected Message(MessageKind kind)
        {
            Kind = kind;
        }

        /// <summary>
        /// Message's Kind.
        /// </summary>
        public MessageKind Kind
        {
            get;
            private set;
        }
        /// <summary>
        /// A LSP Message
        /// </summary>
        [Serializable]
        public class LspMessage : Message
        {
            /// <summary>
            /// From who the message came.
            /// </summary>
            [Serializable]
            public enum MessageFrom
            {
                /// <summary>
                /// A message sent by the client
                /// </summary>
                Client,
                /// <summary>
                /// A Message received from the server
                /// </summary>
                Server,
            }

            /// <summary>
            /// Empty constructor
            /// </summary>
            public LspMessage() : base(MessageKind.Lsp)
            {

            }

            /// <summary>
            /// From message constructor
            /// </summary>
            /// <param name="from"></param>
            /// <param name="message"></param>
            public LspMessage(MessageFrom from, string message) : base(MessageKind.Lsp)
            {
                From = from;
                Message = message;
            }
            /// <summary>
            /// The LSP Message
            /// </summary>
            public string Message
            {
                get;
                set;
            }

            /// <summary>
            /// LSP Message's origin
            /// </summary>
            public MessageFrom From
            {
                get;
                set;
            }
        }
    }
}
