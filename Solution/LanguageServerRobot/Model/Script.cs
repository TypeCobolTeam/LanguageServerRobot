using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServerRobot.Utilities;
using Newtonsoft.Json.Linq;

namespace LanguageServerRobot.Model
{
    /// <summary>
    /// The Model that describe a Script associated toa document.
    /// </summary>
    public class Script
    {
        /// <summary>
        /// Enumeration of the categories to which can belong a message.
        /// </summary>
        public enum MessageCategory
        {
            /// <summary>
            /// A message sent by the client
            /// </summary>
            Client,
            /// <summary>
            /// A Message received from the server
            /// </summary>
            Server,
            /// <summary>
            /// A result (response) received from the server.
            /// </summary>
            Result
        }

        public class Message
        {            
            /// <summary>
            /// The message's category.
            /// </summary>
            public MessageCategory category
            { get; set; }
            /// <summary>
            /// The message
            /// </summary>
            public string message
            { get; set; }

            /// <summary>
            /// Empty constructor
            /// </summary>
            public Message()
            {

            }
            /// <summary>
            /// Cosntructor
            /// </summary>
            /// <param name="category"></param>
            /// <param name="message"></param>
            public Message(MessageCategory category, string message)
            {
                System.Diagnostics.Contracts.Contract.Requires(message != null);
                this.category = category;
                this.message = message;
            }
        }

        /// <summary>
        /// full path to the parent session file
        /// </summary>
        public string session
        { get; set; }
        /// <summary>
        /// Session creator user name
        /// </summary>
        public string user { get; set; }
        /// <summary>
        /// Session's date
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// Session description
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// The Uri corresponding to this stript.
        /// </summary>
        public string uri { get; set; }
        /// <summary>
        /// Parameters of the Initialization message request request
        /// </summary>
        public string initialize { get; set; }
        /// <summary>
        /// the resulting message of the initalization request
        /// </summary>
        public string initialize_result { get; set; }
        /// <summary>
        /// the "textDocument/didOpen" notification that opened the document
        /// </summary>
        public string didOpen { get; set; }

        /// <summary>
        /// All messages (Request, notifications, responses) in sending and reception order.
        /// </summary>
        public List<Message> messages { get; protected set; }
        /// <summary>
        /// the "textDocument/didOpen" notification that opened the document
        /// </summary>
        public string didClose { get; set; }

        /// <summary>
        /// Add a message
        /// </summary>
        /// <param name="category">The Message's category</param>
        /// <param name="message">The message</param>
        public void AddMessage(MessageCategory category, string message)
        {
            JObject jsonObject = null;
            Protocol.MessageKind(message, out jsonObject);
            AddMessage(category, message, jsonObject);
        }

        /// <summary>
        /// Adda a message
        /// </summary>
        /// <param name="category">The Message's category</param>
        /// <param name="message">The message</param>
        /// <param name="jsonObject">The Json object corresponding to the message</param>
        public void AddMessage(MessageCategory category, string message, JObject jsonObject)
        {
            Protocol.Message_Kind kind = Protocol.MessageKind(jsonObject);
            AddMessage(category, kind, message, jsonObject);

        }

        /// <summary>
        /// Adda a message
        /// </summary>
        /// <param name="category">The Message's category</param>
        /// <param name="kind">Message's kind</param>
        /// <param name="message">The message</param>
        /// <param name="jsonObject">The Json object corresponding to the message</param>
        public void AddMessage(MessageCategory category, Protocol.Message_Kind kind, string message, JObject jsonObject)
        {
            System.Diagnostics.Contracts.Contract.Assert(message != null);
            System.Diagnostics.Contracts.Contract.Assert(jsonObject != null);
            System.Diagnostics.Contracts.Contract.Assume(kind == Protocol.MessageKind(jsonObject));
            System.Diagnostics.Contracts.Contract.Requires(category == MessageCategory.Client || category == MessageCategory.Server || category == MessageCategory.Result);
            System.Diagnostics.Contracts.Contract.Requires(kind == Protocol.Message_Kind.Request || kind == Protocol.Message_Kind.Notification || kind == Protocol.Message_Kind.Response);
            // client ==> (Request || Notification) && !Result
            // Server || Result ==> (Notification || Result) && !Request            
            System.Diagnostics.Contracts.Contract.Requires((kind == Protocol.Message_Kind.Response && (category == MessageCategory.Server || category == MessageCategory.Result)) ||
                (kind == Protocol.Message_Kind.Request && category == MessageCategory.Client) ||
                (kind == Protocol.Message_Kind.Notification && (category == MessageCategory.Client || category == MessageCategory.Server)));

            Message msg = new Message(category, message);
            messages.Add(msg);
        }

        /// <summary>
        /// Determine if this script is a valid script. A Valid script is a script that have received
        /// a didOpen and a didClose notification.
        /// </summary>
        public bool IsValid
        {
            get
            {
                JObject jdidOpen = null;
                JObject jdidClose = null;
                return Protocol.IsDidOpenTextDocumentNotification(this.didOpen, out jdidOpen) && Protocol.IsDidCloseTextDocumentNotification(this.didClose, out jdidClose);
            }
        }
        /// <summary>
        /// Dump to the Debug Output Stream
        /// </summary>
        public void DebugDump()
        {
            JObject jobject = JObject.FromObject(this);
            string text = jobject.ToString();
            System.Diagnostics.Debug.WriteLine(text);
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Script() : this(null)
        {            
        }

        /// <summary>
        /// Constructor with uri
        /// </summary>
        /// <param name="uri">The script uri</param>
        public Script(string uri)
        {
            this.uri = uri;
            user = Environment.UserName;
            date = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff");
            messages = new List<Message>();
        }

    }
}
