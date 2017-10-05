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
        }
    }
}
