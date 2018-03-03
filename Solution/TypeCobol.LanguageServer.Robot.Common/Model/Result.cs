using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeCobol.LanguageServer.Robot.Common.Utilities;
using Newtonsoft.Json.Linq;

namespace TypeCobol.LanguageServer.Robot.Common.Model
{
    /// <summary>
    /// The result Model of a test for one script.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Is The result successfull ? true if yes, false otherwise.
        /// </summary>
        public bool success
        {
            get; set;
        }
        /// <summary>
        /// The Uri corresponding to the source stript.
        /// </summary>
        public string uri { get; set; }

        /// <summary>
        /// Index of the first different message in the result_messages list, if success = false.
        /// </summary>
        public int[] diff_index
        {
            get; set;
        }

        /// <summary>
        /// Any exception if the result is success is false because of an applicatio, exception or any error.
        /// </summary>
        public string exception
        {
            get; set;
        }

        /// <summary>
        /// All resulting messages (Request, notifications, responses) in sending and reception order,
        /// Present only if the test failed.
        /// </summary>
        public List<Script.Message> result_messages { get; protected set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Result()
        {
            
        }
        /// <summary>
        /// Result constructor
        /// </summary>
        /// <param name="SourceScript">The Source Script</param>
        /// <param name="ResultScript">The resulting script.</param>
        public Result(Script SourceScript, Script ResultScript)
        {
            diff_index = new int[]{ -1};
            result_messages = null;
            uri = SourceScript.uri;
            success = Equals(ResultScript, SourceScript);
        }
        /// <summary>
        /// Check if messages contained in this scripts are equals to messages of the other script.
        /// </summary>
        /// <param name="other">The other script is in fact the source script.</param>
        /// <returns>true if both messages are equals, false otherwise.</returns>
        private bool Equals(Script result, Script other)
        {
            if (other == null  || result == null)
                return false;
            List<int> diffindexes = new List<int>();
            bool bSucess = true;
            for (int i = 0; i < result.messages.Count && i < other.messages.Count; i++)
            {
                if ((result.messages[i].category != other.messages[i].category) ||
                    (result.messages[i].message != other.messages[i].message))
                {   //Store the index of the first different message.
                    diffindexes.Add(i);                    
                    result_messages = result.messages;
                    bSucess = false;
                }
            }
            if (result.messages.Count != other.messages.Count)
            {
                diffindexes.Add(Math.Min(result.messages.Count, other.messages.Count));                
                result_messages = result.messages;
                bSucess = false;
            }
            if (!bSucess)
                diff_index = diffindexes.ToArray();
            return bSucess;
        }

        /// <summary>
        /// Write the result in the given FileStream using UTF8 encoding.
        /// </summary>
        public void Write(System.IO.FileStream stream)
        {
            //Avoid wrinting the uri
            string save_uri = this.uri;
            try
            {
                this.uri = null;
                System.Diagnostics.Contracts.Contract.Assert(stream != null);
                System.Diagnostics.Contracts.Contract.Requires(stream.CanWrite);
                JObject jobject = JObject.FromObject(this);
                string text = jobject.ToString();
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                stream.Write(bytes, 0, bytes.Length);
            }
            finally
            {
                this.uri = save_uri;
            }            
        }
    }
}
