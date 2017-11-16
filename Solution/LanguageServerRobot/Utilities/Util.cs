using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServerRobot.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LanguageServerRobot.Utilities
{
    /// <summary>
    /// Utility class
    /// </summary>
    public class Util
    {
        /// <summary>
        /// Scrip file extension
        /// </summary>
        public static readonly String SCRIPT_FILE_EXTENSION = ".tlsp";
        /// <summary>
        /// Session file extension
        /// </summary>
        public static readonly String SESSION_FILE_EXTENSION = ".slsp";

        /// <summary>
        /// Get the identifier name corresponding to an URI. This by replace characters like: \, /, ", . by an underscore.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string UriToIdentifierName(string uri)
        {
            return uri.Replace('/', '_').Replace('\\', '_').Replace('.', '_').Replace(' ', '_').Replace(':', '_').Replace('"', '_');
        }

        private static string ScriptPath = null;
        /// <summary>
        /// Get the default output script path.
        /// </summary>
        /// <returns>The default output script path</returns>
        public static string DefaultScriptRepositorPath
        {
            get
            {
                lock (typeof(Util))
                {
                    string path = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    if (ScriptPath == null)
                    {                        
                        path = System.IO.Path.Combine(path, "LanguageServerRobot");
                        path = System.IO.Path.Combine(path, "Repository");
                        try
                        {
                            System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(path);//Create the directory
                            ScriptPath = di.FullName;
                        }
                        catch (System.IO.IOException ioe)
                        {
                            ScriptPath = path;
                        }
                        catch (System.UnauthorizedAccessException uae)
                        {
                            ScriptPath = path;
                        }
                        catch (System.ArgumentNullException ane)
                        {
                            ScriptPath = path;
                        }
                        catch (System.NotSupportedException nse)
                        {
                            ScriptPath = path;
                        }
                    }
                    return ScriptPath;
                }
            }
        }

        /// <summary>
        /// Create a directory for a session using the default Script Path.
        /// </summary>
        /// <param name="root">The root directory</param>
        /// <returns>The directory for the session if one can be created, null otherwise</returns>
        public static bool CreateSessionDirectory(out string sessionDirectoryPath, string root = null)
        {
            string path = root == null ? DefaultScriptRepositorPath : root;
            String date = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff");
            date = date.Replace(':', '_').Replace('.', '_').Replace(' ', '_').Replace('/', '_').Replace('\\', '_');
            path = System.IO.Path.Combine(path, "Session" + date);
            sessionDirectoryPath = path;
            try
            {                
                System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(path);//Create the directory
                sessionDirectoryPath = di.FullName;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if the given filepath has a Script File Extension
        /// </summary>
        /// <param name="filepath">The file path to check.</param>
        /// <returns>true if the file path as script file path extension, false otherwise</returns>
        public static bool HasScriptFileExtension(string filepath)
        {
            System.Diagnostics.Debug.Assert(filepath != null);
            return filepath.EndsWith(SCRIPT_FILE_EXTENSION);
        }

        /// <summary>
        /// Read a script file.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="script">[out] the Script model if the file path corresponds to a Script file</param>
        /// <param name="exc">[out] Any exception that might have be thrown if the read failed.</param>
        /// <returns>true if a script file has been read, false otherwise</returns>
        public static bool ReadScriptFile(string filepath, out Script script, out Exception exc)
        {
            System.Diagnostics.Debug.Assert(filepath != null);
            exc = null;
            script = null;
            if (HasScriptFileExtension(filepath))
            {//Read the file using UTF8.
                try
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                        {                            
                            string data = sr.ReadToEnd();
                            JObject jobject = JObject.Parse(data);
                            script = (Script)jobject.ToObject(typeof(Script));
                            return true;
                        }
                    }
                }
                catch(Exception e)
                {
                    exc = e;
                }
            }
            return false;       
        }
    }
}
