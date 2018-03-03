using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TypeCobol.LanguageServer.Robot.Common.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TypeCobol.LanguageServer.Robot.Common.Utilities
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
        /// Result file extension
        /// </summary>
        public static readonly String RESULT_FILE_EXTENSION = ".rlsp";
        /// <summary>
        /// Result Sub directory
        /// </summary>
        public static readonly String RESULT_SUB_DIRECTORY = "Results";


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
                        path = System.IO.Path.Combine(path, "TypeCobol.LanguageServerRobot");
                        path = System.IO.Path.Combine(path, "Repository");
                        try
                        {
                            System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(path);//Create the directory
                            ScriptPath = di.FullName;
                        }
                        catch (System.IO.IOException /*ioe*/)
                        {
                            ScriptPath = path;
                        }
                        catch (System.UnauthorizedAccessException /*uae*/)
                        {
                            ScriptPath = path;
                        }
                        catch (System.ArgumentNullException /*ane*/)
                        {
                            ScriptPath = path;
                        }
                        catch (System.NotSupportedException /*nse*/)
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
            catch (Exception /*e*/)
            {
                return false;
            }
        }

        /// <summary>
        /// Ensure that the result output directory for a script uri exists.
        /// </summary>
        /// <param name="script_path">The file path of the source script</param>
        /// <returns></returns>
        public static bool EnsureResultDirectoryExists(string script_path, out string result_dir)
        {
            result_dir = null;
            FileInfo fi = new FileInfo(script_path);
            DirectoryInfo di = fi.Directory;
            string result_path = System.IO.Path.Combine(di.FullName, RESULT_SUB_DIRECTORY);
            try
            {
                System.IO.DirectoryInfo rdi = System.IO.Directory.CreateDirectory(result_path);//Create the directory
                result_dir = rdi.FullName;
                return true;
            }
            catch (Exception /*e*/)
            {
                return false;
            }
        }

        /// <summary>
        /// Get the result file name corresponding to an uri.
        /// </summary>
        /// <param name="script_path">The file path of the source script</param>
        /// <returns></returns>
        public static string GetResultFileName(string script_path)
        {
            FileInfo fi = new FileInfo(script_path);
            string extension = fi.Extension;
            string resultName = (extension != null && extension.Length > 0) ? fi.Name.Substring(0, fi.Name.Length - extension.Length) : fi.Name + '.';
            return resultName + RESULT_FILE_EXTENSION;
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

        /// <summary>
        /// Determines if the given filepath has a Session File Extension
        /// </summary>
        /// <param name="filepath">The file path to check.</param>
        /// <returns>true if the file path as session file path extension, false otherwise</returns>
        public static bool HasSessionFileExtension(string filepath)
        {
            System.Diagnostics.Debug.Assert(filepath != null);
            return filepath.EndsWith(SESSION_FILE_EXTENSION);
        }
        /// <summary>
        /// Read a session file.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="session">[out] the Session model if the file path corresponds to a Session file</param>
        /// <param name="exc">[out] Any exception that might have be thrown if the read failed.</param>
        /// <returns>true if a session file has been read, false otherwise</returns>
        public static bool ReadSessionFile(string filepath, out Session session, out Exception exc)
        {
            System.Diagnostics.Debug.Assert(filepath != null);
            exc = null;
            session = null;
            if (HasSessionFileExtension(filepath))
            {//Read the file using UTF8.
                try
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                        {
                            string data = sr.ReadToEnd();
                            JObject jobject = JObject.Parse(data);
                            session = (Session)jobject.ToObject(typeof(Session));
                            return true;
                        }
                    }
                }
                catch (Exception e)
                {
                    exc = e;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if the given filepath has a Result File Extension
        /// </summary>
        /// <param name="filepath">The file path to check.</param>
        /// <returns>true if the file path has resut file path extension, false otherwise</returns>
        public static bool HasResultFileExtension(string filepath)
        {
            System.Diagnostics.Debug.Assert(filepath != null);
            return filepath.EndsWith(RESULT_FILE_EXTENSION);
        }

        /// <summary>
        /// Read a result file.
        /// </summary>
        /// <param name="filepath">The file path of the result</param>
        /// <param name="result">[out] the Result model if the file path corresponds to a Script file</param>
        /// <param name="exc">[out] Any exception that might have be thrown if the read failed.</param>
        /// <returns>true if a result file has been read, false otherwise</returns>
        public static bool ReadResultFile(string filepath, out Result result, out Exception exc)
        {
            System.Diagnostics.Debug.Assert(filepath != null);
            exc = null;
            result = null;
            if (HasResultFileExtension(filepath))
            {//Read the file using UTF8.
                try
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                        {
                            string data = sr.ReadToEnd();
                            JObject jobject = JObject.Parse(data);
                            result = (Result)jobject.ToObject(typeof(Result));
                            return true;
                        }
                    }
                }
                catch (Exception e)
                {
                    exc = e;
                }
            }
            return false;
        }

        /// <summary>
        /// Create a Name for a Pipe.
        /// </summary>
        /// <returns></returns>
        public static string CreatePipeName()
        {
            String date = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff");
            date = date.Replace(':', '_').Replace('.', '_').Replace(' ', '_').Replace('/', '_').Replace('\\', '_');
            return "LSR_" + date;
        }

        /// <summary>
        /// Signature for a message Box time out
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="wLanguageId"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern uint MessageBoxTimeoutW(IntPtr hwnd,
             [MarshalAs(UnmanagedType.LPWStr)]  String text,
             [MarshalAs(UnmanagedType.LPWStr)] String title,
             [MarshalAs(UnmanagedType.U4)] uint type,
             Int16 wLanguageId,
             Int32 milliseconds);
    }
}
