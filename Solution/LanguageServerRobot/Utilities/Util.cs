using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServerRobot.Utilities
{
    /// <summary>
    /// Utility class
    /// </summary>
    public class Util
    {        
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
        public static string CreateSessionDirectory(string root = null)
        {
            string path = root == null ? DefaultScriptRepositorPath : root;
            path = System.IO.Path.Combine(path, "Session");
            String date = System.DateTime.Now.ToString();
            date.Replace(':', '_').Replace('.', '_').Replace(' ', '_');
            path = System.IO.Path.Combine(path, "Session" + date);
            try
            {
                System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(path);//Create the directory
                return di.FullName;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
