using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeCobol.LanguageServer.Protocol;
using TypeCobol.LanguageServer.Robot.Common.Utilities;
using Newtonsoft.Json.Linq;

namespace TypeCobol.LanguageServer.Robot.Common.Model
{
    /// <summary>
    /// The Model that describe a Test Session
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Session's name (Optional)
        /// </summary>
        public string name { get; set; }
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
        /// The session directory.
        /// </summary>
        public string directory;
        /// <summary>
        /// Parameters of the Initialization message request request
        /// </summary>
        public string initialize { get; set; }
        /// <summary>
        /// the resulting message of the initalization request
        /// </summary>
        public string initialize_result { get; set; }
        /// <summary>
        /// The "initialize" notification from the client.
        /// </summary>
        public string initialized_notification { get; set; }
        /// <summary>
        /// Any did change configuration message. Only one configuration notification is taken in account per session.
        /// </summary>
        public string did_change_configuation { get; set; }
        /// <summary>
        /// All messages from the client that occured after the "initialize" request result.
        /// </summary>
        public List<string> client_in_initialize_messages { get; protected set; }
        /// <summary>
        /// All messages from the client that occured after the "initialized" notification ==> in the start.
        /// Are not not messages an opened document.
        /// </summary>
        public List<string> client_in_start_messages { get; protected set; }
        /// <summary>
        /// All messages from the server that occured after the "initialize" request result.
        /// </summary>
        public List<string> server_in_initialize_messages { get; protected set; }
        /// <summary>
        /// All messages from the server that occured after the "initialized" notification ==> in the start.
        /// Are not not messages an opened document.
        /// </summary>
        public List<string> server_in_start_messages { get; protected set; }
        /// <summary>
        /// The list of session's script files.
        /// </summary>
        public List<string> scripts { get; set; }
        /// <summary>
        /// The shutdown message if any
        /// </summary>
        public string shutdown { get; set; }

        /// <summary>
        /// The result of the shutdown request if any. This value can be null
        /// if the client has exited before the shtutdown result as been received.
        /// </summary>
        public string shutdown_result { get; set; }

        /// <summary>
        /// The exit message if any
        /// </summary>
        public string exit { get; set; }

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
        /// Write the session in the given FileStream using UTF8 encoding.
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
        /// Get the session file name
        /// </summary>
        /// <returns></returns>
        public string GetSessionFileName()
        {
            string filename = Util.UriToIdentifierName("TestSuite" + '_' + date);
            filename += Util.SESSION_FILE_EXTENSION;
            return directory != null ? System.IO.Path.Combine(directory, filename): filename;
        }


        /// <summary>
        /// Empty constructor
        /// </summary>
        public Session()
        {
            scripts = new List<string>();
            date = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff");
            user = Environment.UserName;            

            client_in_initialize_messages = new List<string>();
            client_in_start_messages = new List<string>();

            server_in_initialize_messages = new List<string>();
            server_in_start_messages = new List<string>();
        }
    }
}
