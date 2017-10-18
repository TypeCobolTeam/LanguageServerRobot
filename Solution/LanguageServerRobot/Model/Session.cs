using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.Protocol;

namespace LanguageServerRobot.Model
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
        /// The shutdown message
        /// </summary>
        public string shutdown { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Session()
        {
            scripts = new List<string>();
            date = System.DateTime.Today.ToString();
            user = Environment.UserName;

            client_in_initialize_messages = new List<string>();
            client_in_start_messages = new List<string>();

            server_in_initialize_messages = new List<string>();
            server_in_start_messages = new List<string>();
        }
    }
}
