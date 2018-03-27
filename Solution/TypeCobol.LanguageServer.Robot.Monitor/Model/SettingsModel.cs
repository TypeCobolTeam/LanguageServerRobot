using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace TypeCobol.LanguageServer.Robot.Monitor.Model
{
    /// <summary>
    /// The settings Model
    /// </summary>
    public class SettingsModel
    {
        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public SettingsModel()
        {
            ReadFromSettings();
        }

        /// <summary>
        /// The Server Path
        /// </summary>
        public string ServerPath
        {
            get; set;
        }

        /// <summary>
        /// The language Server Robot Path
        /// </summary>
        public string LSRPath
        {
            get; set;
        }

        /// <summary>
        /// The Script/Session repository Path
        /// </summary>
        public String ScriptRepositoryPath
        {
            get; set;
        }

        public string BatchTemplate
        {
            get; set;
        }

        public string LSRReplayArguments
        {
            get; set;
        }

        public string MailReceiver
        {
            get; set;
        }

        public string SmtpServer
        {
            get; set;
        }

        public string SmtpPort
        {
            get; set;
        }

        /// <summary>
        /// Read Default Values
        /// </summary>
        public void ReadFromDefault()
        {
            Properties.Settings.Default.Reset();
            ReadFromSettings();
            //ServerPath = Properties.Resources.DefaultServerPath;
            //LSRPath = Properties.Resources.DefaultLSRPath;
            //ScriptRepositoryPath = Properties.Resources.DefaultScriptRepositoryPath;
            //BatchTemplate = Properties.Resources.DefaultBatchTemplate;
        }
        /// <summary>
        /// Read the model from application setting values
        /// </summary>
        public void ReadFromSettings()
        {
            ServerPath = Properties.Settings.Default.ServerPath;
            LSRPath = Properties.Settings.Default.LSRPath;
            ScriptRepositoryPath = Properties.Settings.Default.ScriptPath;
            LSRReplayArguments = Properties.Settings.Default.LSRReplayArguments;
            BatchTemplate = Properties.Settings.Default.BatchTemplate;
            MailReceiver = Properties.Settings.Default.MailReceiver;
            SmtpServer = Properties.Settings.Default.SmtpServer;
            SmtpPort = Properties.Settings.Default.SmtpPort;
        }

        /// <summary>
        /// Write the model to Application setting value
        /// </summary>
        public void WriteToSettings()
        {
            Properties.Settings.Default.ServerPath = ServerPath;
            Properties.Settings.Default.LSRPath = LSRPath;
            Properties.Settings.Default.ScriptPath = ScriptRepositoryPath;
            Properties.Settings.Default.LSRReplayArguments = LSRReplayArguments;
            Properties.Settings.Default.BatchTemplate = BatchTemplate;
            Properties.Settings.Default.MailReceiver = MailReceiver;
            Properties.Settings.Default.SmtpServer = SmtpServer;
            Properties.Settings.Default.SmtpPort = SmtpPort;
            Properties.Settings.Default.Save();
        }
    }
}
