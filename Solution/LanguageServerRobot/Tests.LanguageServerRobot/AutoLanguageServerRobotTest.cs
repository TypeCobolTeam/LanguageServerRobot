using System;
using System.Diagnostics;
using System.IO;
using LanguageServer.JsonRPC;
using LanguageServer.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Tests.LanguageServer.JsonRPC;

namespace Tests.LanguageServerRobot
{
    [TestClass]
    /// <summary>
    /// This test run a LanguageServerRobot application process, and check that the "Initialize" request has been submitted.
    /// </summary>
    public class AutoLanguageServerRobotTest
    {
        [TestMethod]
        [Ignore]
        public void AutoLanguageServerRobotProcess()
        {            
            String app = "C:\\Users\\MAYANJE\\Source\\Repos\\TypeCobol9\\LanguageServerRobot\\LanguageServerRobot\\Solution\\LanguageServerRobot\\bin\\Debug\\LanguageServerRobot.exe";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = app;
            process.StartInfo.Arguments = "-s C:\\Users\\MAYANJE\\Source\\Repos\\TypeCobol9\\TypeCobol\\bin\\Debug\\TypeCobol.LanguageServer.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            //Start the process
            try
            {
                if (!process.Start())
                {   //We didn't succed to run the Process
                    Assert.Fail();
                    return;
                }
                else
                {
                    InitializeParams p = new InitializeParams();
                    p.processId = process.Id;
                    p.rootPath = "C:\\Users\\MAYANJE\\Source\\Repos\\TypeCobol9\\LanguageServerRobot\\LanguageServerRobot\\Solution\\LanguageServerRobot\\bin\\Debug";
                    JObject jsonMessage = new JObject();
                    TestUtilities.PrepareJsonPRCMessage(jsonMessage);
                    jsonMessage["id"] = "100";
                    jsonMessage["method"] = InitializeRequest.Type.Method;
                    jsonMessage["params"] = JToken.FromObject(p);
                    string msg = jsonMessage.ToString();
                    TestUtilities.WriteJsonMessage(process.StandardInput.BaseStream, msg);
                    process.StandardInput.BaseStream.Flush();
                    Assert.AreEqual(process.StandardOutput.ReadLine(), "Content-Length:120");
                    Assert.AreEqual(process.StandardOutput.ReadLine(), "");
                    Assert.AreEqual(process.StandardOutput.ReadLine(),
                        "{\"jsonrpc\":\"2.0\",\"method\":\"window/showMessage\",\"params\":{\"type\":3,\"message\":\"TypeCobol language server was launched !\"}}Content-Length:448");
                    process.Kill();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
