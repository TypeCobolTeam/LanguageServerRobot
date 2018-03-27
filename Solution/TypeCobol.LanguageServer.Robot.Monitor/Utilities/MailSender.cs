using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using TypeCobol.LanguageServer.Robot.Common.Model;

namespace TypeCobol.LanguageServer.Robot.Monitor.Utilities
{
    public class MailSender
    {        
        public static void SendScriptMail(Script snapshot)
        {
            try
            {
                var currentUserMail = UserPrincipal.Current.EmailAddress;
                var mail = new MailMessage();
                var smtpClient = new SmtpClient();
                string errorTemplate = Properties.Resources.MailScenarioTemplate;

                smtpClient.Host = Properties.Settings.Default.SmtpServer;
                smtpClient.Port = int.Parse(Properties.Settings.Default.SmtpPort);
                //smtpClient.UseDefaultCredentials =
                //    bool.Parse(_AppConfig.AppSettings.Settings["SmtpUseDefaultCredential"].Value);
                smtpClient.UseDefaultCredentials = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                var receiver = Properties.Settings.Default.MailReceiver;
                mail.To.Add(new MailAddress(receiver));
                mail.From = new MailAddress(currentUserMail);
                //mail.Subject = _AppConfig.AppSettings.Settings["MailSubject"].Value;
                mail.Subject = "TypeCobol.LSRM";

                errorTemplate = errorTemplate.Replace("{DateTime}", DateTime.Now.ToString());

                errorTemplate = errorTemplate.Replace("{User}", currentUserMail.Split('@')[0].Replace('.', ' '));
                errorTemplate = errorTemplate.Replace("{Source}", Properties.Resources.LSRMName);

                System.IO.StringWriter sw = new System.IO.StringWriter();
                snapshot.Write(sw);
                sw.Flush();
                string text = sw.ToString();
                errorTemplate = errorTemplate.Replace("{Snapshot}", text);

                mail.IsBodyHtml = true;
                mail.Body = errorTemplate;

                smtpClient.Send(mail);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
