using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LanguageServer.Robot.Common.Utilities;
using Newtonsoft.Json.Linq;

namespace LanguageServer.Robot.Common.Model
{
    /// <summary>
    /// The Model that describe a Script associated toa document.
    /// </summary>
    public class Script : IEquatable<Script>, ICloneable
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
            public MessageCategory category { get; set; }

            /// <summary>
            /// The message
            /// </summary>
            public string message { get; set; }

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
        /// Script's name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// full path to the parent session file
        /// </summary>
        public string session { get; set; }

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
        /// Any did change configuration message. Only one configuration notification is taken in account per script of a sesion.
        /// </summary>
        public string did_change_configuation { get; set; }

        /// <summary>
        /// the "textDocument/didOpen" notification that opened the document
        /// </summary>
        public string didOpen { get; set; }

        /// <summary>
        /// All messages (Request, notifications, responses) in sending and reception order.
        /// </summary>
        public List<Message> messages { get; internal set; }

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
            Utilities.Protocol.MessageKind(message, out jsonObject);
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
            Utilities.Protocol.Message_Kind kind = Utilities.Protocol.MessageKind(jsonObject);
            AddMessage(category, kind, message, jsonObject);

        }

        /// <summary>
        /// Adda a message
        /// </summary>
        /// <param name="category">The Message's category</param>
        /// <param name="kind">Message's kind</param>
        /// <param name="message">The message</param>
        /// <param name="jsonObject">The Json object corresponding to the message</param>
        public void AddMessage(MessageCategory category, Utilities.Protocol.Message_Kind kind, string message,
            JObject jsonObject)
        {
            System.Diagnostics.Contracts.Contract.Assert(message != null);
            System.Diagnostics.Contracts.Contract.Assert(jsonObject != null);
            System.Diagnostics.Contracts.Contract.Assume(kind == Utilities.Protocol.MessageKind(jsonObject));
            System.Diagnostics.Contracts.Contract.Requires(category == MessageCategory.Client ||
                                                           category == MessageCategory.Server ||
                                                           category == MessageCategory.Result);
            System.Diagnostics.Contracts.Contract.Requires(kind == Utilities.Protocol.Message_Kind.Request ||
                                                           kind == Utilities.Protocol.Message_Kind.Notification ||
                                                           kind == Utilities.Protocol.Message_Kind.Response);
            // client ==> (Request || Notification) && !Result
            // Server || Result ==> (Notification || Result) && !Request            
            System.Diagnostics.Contracts.Contract.Requires((kind == Utilities.Protocol.Message_Kind.Response &&
                                                            (category == MessageCategory.Server ||
                                                             category == MessageCategory.Result)) ||
                                                           (kind == Utilities.Protocol.Message_Kind.Request &&
                                                            category == MessageCategory.Client) ||
                                                           (kind == Utilities.Protocol.Message_Kind.Notification &&
                                                            (category == MessageCategory.Client ||
                                                             category == MessageCategory.Server)));

            Message msg = new Message(category, message);
            lock (this)
            {
                messages.Add(msg);
            }
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
                return Utilities.Protocol.IsDidOpenTextDocumentNotification(this.didOpen, out jdidOpen) &&
                       Utilities.Protocol.IsDidCloseTextDocumentNotification(this.didClose, out jdidClose);
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
        /// Write the script in the given FileStream using UTF8 encoding.
        /// </summary>
        public void Write(System.IO.FileStream stream)
        {
            System.Diagnostics.Contracts.Contract.Assert(stream != null);
            System.Diagnostics.Contracts.Contract.Requires(stream.CanWrite);
            JObject jobject = JObject.FromObject(this);
            string text = jobject.ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Check if messages contained in this scripts are equals to messages of the other script.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Script other)
        {
            if (other == null)
                return false;
            if ((didOpen != other.didOpen) || (didClose != other.didClose))
                return false;
            if (messages.Count != other.messages.Count)
                return false;
            for (int i = 0; i < messages.Count; i++)
            {
                if ((messages[i].category != other.messages[i].category) ||
                    (messages[i].message != other.messages[i].message))
                    return false;
            }
            return true;
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

        /// <summary>
        /// Clone this script.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Script cloned = new Script();
            cloned.Copy(this);
            return cloned;
        }

        /// <summary>
        /// Copy the Script attribute from another script.
        /// </summary>
        /// <param name="from">The Other script to copy attributes from.</param>
        /// <param name="bDeepCopy">True if a new List of message must be created, false otherwise.</param>
        public void Copy(Script from, bool bDeepCopy = false)
        {
            this.name = from.name;
            this.session = from.session;
            this.user = from.user;
            this.date = from.date;
            this.description = from.description;
            this.uri = from.uri;
            this.initialize = from.initialize;
            this.initialize_result = from.initialize_result;
            this.did_change_configuation = from.did_change_configuation;
            this.didOpen = from.didOpen;
            if (bDeepCopy)
            {
                this.messages.Clear();
                lock (from)
                {
                    this.messages.AddRange(from.messages);
                }
            }
            else
            {
                this.messages = from.messages;
            }
            this.didClose = from.didClose;
        }
    }
}
