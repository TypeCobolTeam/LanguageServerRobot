using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Tests.LanguageServer.JsonRPC
{
    /// <summary>
    /// Class for Tests related to the ProcessMessageConnection tests
    /// </summary>
    [TestClass]
    public class ProcessMessageConnectionTest
    {
        const string TypeCovolLanguageServer = "C:\\TypeCobol\\Sources\\##Latest_Release##\\TypeCobol.LanguageServer.exe";
        bool bKilled = false;
        bool bExited = false;
        [TestMethod]
        [Ignore]
        public void RunCTypeCobolLangServerLatestReleaseAndKill()
        {
            ProcessMessageConnection connection = new ProcessMessageConnection(TypeCovolLanguageServer, exithandler);
            bKilled = false;
            bExited = false;
            Task killer = new Task(() => { System.Threading.Thread.Sleep(5000); bKilled = true;  connection.Process?.Kill(); });
            killer.Start();
            Task<bool> task = Start(connection);
            bool bResult = task.Result;
            Assert.IsTrue(bKilled);
        }

        private async Task<bool> Start(ProcessMessageConnection connection)
        {
            DelegateMessageConsumer consumer = new DelegateMessageConsumer(
                (string message) =>
                {
                    System.Console.Error.WriteLine(message);
                }
                );
            return await connection.Start(consumer);
        }

        public async void WaitTask(Task<bool> task)
        {
            bool b = await task.ConfigureAwait(false);
        }


        private void exithandler(object sender, EventArgs e)
        {
            Assert.IsTrue(bKilled);
        }
    }
}

