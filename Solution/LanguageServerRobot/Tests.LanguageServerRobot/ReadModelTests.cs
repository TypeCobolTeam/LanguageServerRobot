using System;
using System.Diagnostics;
using System.IO;
using LanguageServer.JsonRPC;
using LanguageServer.Protocol;
using LanguageServer.Robot.Model;
using LanguageServer.Robot.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Tests.LanguageServer.JsonRPC;


namespace Tests.LanguageServer.Robot
{
    /// <summary>
    /// Class for testing reading script and session models.
    /// </summary>
    [TestClass]    
    public class ReadModelTests
    {
        [TestMethod]
        public void CheckReadScript()
        {
            Script script = null;
            Exception exc = null;
            string current_dir = Directory.GetCurrentDirectory();
            string test_file = System.IO.Path.Combine(current_dir, "TestFiles\\Session0\\Script01.tlsp");
            Assert.IsTrue(Util.ReadScriptFile(test_file, out script, out exc));
            Assert.IsTrue(script.initialize == "{\"jsonrpc\":\"2.0\",\"id\":\"0\",\"method\":\"initialize\",\"params\":{\"processId\":-1,\"rootPath\":\"C:\\\\Program Files (x86)\\\\IBM\\\\SDP_IDZ14\",\"rootUri\":\"file:/C:/Program%20Files%20(x86)/IBM/SDP_IDZ14/\",\"capabilities\":{\"workspace\":{\"applyEdit\":true,\"didChangeConfiguration\":{\"dynamicRegistration\":true},\"didChangeWatchedFiles\":{\"dynamicRegistration\":false},\"symbol\":{\"dynamicRegistration\":true},\"executeCommand\":{\"dynamicRegistration\":true}},\"textDocument\":{\"synchronization\":{\"willSave\":true,\"willSaveWaitUntil\":true,\"dynamicRegistration\":true},\"completion\":{\"completionItem\":{\"snippetSupport\":true},\"dynamicRegistration\":true},\"hover\":{\"dynamicRegistration\":true},\"signatureHelp\":{\"dynamicRegistration\":true},\"references\":{\"dynamicRegistration\":true},\"documentHighlight\":{\"dynamicRegistration\":true},\"documentSymbol\":{\"dynamicRegistration\":true},\"formatting\":{\"dynamicRegistration\":true},\"rangeFormatting\":{\"dynamicRegistration\":true},\"onTypeFormatting\":{\"dynamicRegistration\":true},\"definition\":{\"dynamicRegistration\":true},\"codeAction\":{\"dynamicRegistration\":true},\"codeLens\":{\"dynamicRegistration\":true},\"documentLink\":{\"dynamicRegistration\":true},\"rename\":{\"dynamicRegistration\":true}}},\"trace\":\"off\"}}");
            Assert.IsTrue(script.session == "C:\\Users\\MAYANJE\\Source\\LSRSessions\\Session2017_11_16_11_03_45_034\\TestSuite_2017_11_16_11_03_45_028.slsp");
            Assert.IsTrue(script.initialize_result == "{\"jsonrpc\":\"2.0\",\"id\":\"0\",\"result\":{\"capabilities\":{\"textDocumentSync\":2,\"hoverProvider\":true,\"completionProvider\":{\"resolveProvider\":false,\"triggerCharacters\":[\"::\"]},\"definitionProvider\":true,\"referencesProvider\":false,\"documentHighlightProvider\":false,\"documentSymbolProvider\":false,\"workspaceSymbolProvider\":false,\"codeActionProvider\":false,\"documentFormattingProvider\":false,\"documentRangeFormattingProvider\":false,\"renameProvider\":false}}}");
            Assert.IsTrue(script.user == "MAYANJE");
            Assert.IsTrue(script.date == "2017/11/16 11:03:45 382");
            Assert.IsTrue(script.uri == "file:/C:/Users/MAYANJE/AppData/Local/Temp/tcbl/PARA13105124614291384263.cee");
            Assert.IsTrue(script.didOpen == "{\"jsonrpc\":\"2.0\",\"method\":\"textDocument/didOpen\",\"params\":{\"textDocument\":{\"uri\":\"file:/C:/Users/MAYANJE/AppData/Local/Temp/tcbl/PARA13105124614291384263.cee\",\"languageId\":\"__lsp4j_TypeCobol\",\"version\":0,\"text\":\"       IDENTIFICATION DIVISION.\\r\\n       PROGRAM-ID. HELLO.\\r\\n\\r\\n       PROCEDURE DIVISION.\\r\\n\\r\\n       A-PARA.\\r\\n           PERFORM  DISPLAY \\u0027IN A-PARA\\u0027\\r\\n           END-PERFORM.\\r\\n           PERFORM C-PARA THRU E-PARA.\\r\\n\\r\\n       B-PARA.\\r\\n           DISPLAY \\u0027IN B-PARA\\u0027.\\r\\n           STOP RUN.\\r\\n\\r\\n       C-PARA.\\r\\n           DISPLAY \\u0027IN C-PARA\\u0027.\\r\\n\\r\\n       D-PARA.\\r\\n           DISPLAY \\u0027IN D-PARA\\u0027.\\r\\n\\r\\n       E-PARA.\\r\\n           DISPLAY \\u0027IN E-PARA\\u0027.\\r\\n       END PROGRAM HELLO.\"}}}");
            Assert.IsTrue(script.didClose == "{\"jsonrpc\":\"2.0\",\"method\":\"textDocument/didClose\",\"params\":{\"textDocument\":{\"uri\":\"file:/C:/Users/MAYANJE/AppData/Local/Temp/tcbl/PARA13105124614291384263.cee\"}}}");
            Assert.IsTrue(script.IsValid);
            Assert.IsTrue(script.messages.Count == 2);
            Assert.IsTrue(script.messages[0].category == Script.MessageCategory.Client);
            Assert.IsTrue(script.messages[0].message == "{\"jsonrpc\":\"2.0\",\"id\":\"1\",\"method\":\"textDocument/completion\",\"params\":{\"textDocument\":{\"uri\":\"file:/C:/Users/MAYANJE/AppData/Local/Temp/tcbl/PARA13105124614291384263.cee\"},\"uri\":\"file:/C:/Users/MAYANJE/AppData/Local/Temp/tcbl/PARA13105124614291384263.cee\",\"position\":{\"line\":6,\"character\":19}}}");
            Assert.IsTrue(script.messages[1].category == Script.MessageCategory.Server);
            Assert.IsTrue(script.messages[1].message == "{\"jsonrpc\":\"2.0\",\"id\":\"1\",\"result\":[{\"label\":\"A-PARA\",\"kind\":18},{\"label\":\"B-PARA\",\"kind\":18},{\"label\":\"C-PARA\",\"kind\":18},{\"label\":\"D-PARA\",\"kind\":18},{\"label\":\"E-PARA\",\"kind\":18}]}");
        }

        [TestMethod]
        public void CheckReadSession()
        {
            Session session = null;
            Exception exc = null;
            string current_dir = Directory.GetCurrentDirectory();
            string test_file = System.IO.Path.Combine(current_dir, "TestFiles\\Session0\\TestSuite01.slsp");
            Assert.IsTrue(Util.ReadSessionFile(test_file, out session, out exc));
            Assert.IsTrue(session.directory == "C:\\Users\\MAYANJE\\Source\\LSRSessions\\Session2017_11_16_11_03_45_034");
            Assert.IsTrue(session.user == "MAYANJE");
            Assert.IsTrue(session.date == "2017/11/16 11:03:45 028");
            Assert.IsTrue(session.initialize == "{\"jsonrpc\":\"2.0\",\"id\":\"0\",\"method\":\"initialize\",\"params\":{\"processId\":-1,\"rootPath\":\"C:\\\\Program Files (x86)\\\\IBM\\\\SDP_IDZ14\",\"rootUri\":\"file:/C:/Program%20Files%20(x86)/IBM/SDP_IDZ14/\",\"capabilities\":{\"workspace\":{\"applyEdit\":true,\"didChangeConfiguration\":{\"dynamicRegistration\":true},\"didChangeWatchedFiles\":{\"dynamicRegistration\":false},\"symbol\":{\"dynamicRegistration\":true},\"executeCommand\":{\"dynamicRegistration\":true}},\"textDocument\":{\"synchronization\":{\"willSave\":true,\"willSaveWaitUntil\":true,\"dynamicRegistration\":true},\"completion\":{\"completionItem\":{\"snippetSupport\":true},\"dynamicRegistration\":true},\"hover\":{\"dynamicRegistration\":true},\"signatureHelp\":{\"dynamicRegistration\":true},\"references\":{\"dynamicRegistration\":true},\"documentHighlight\":{\"dynamicRegistration\":true},\"documentSymbol\":{\"dynamicRegistration\":true},\"formatting\":{\"dynamicRegistration\":true},\"rangeFormatting\":{\"dynamicRegistration\":true},\"onTypeFormatting\":{\"dynamicRegistration\":true},\"definition\":{\"dynamicRegistration\":true},\"codeAction\":{\"dynamicRegistration\":true},\"codeLens\":{\"dynamicRegistration\":true},\"documentLink\":{\"dynamicRegistration\":true},\"rename\":{\"dynamicRegistration\":true}}},\"trace\":\"off\"}}");
            Assert.IsTrue(session.initialize_result == "{\"jsonrpc\":\"2.0\",\"id\":\"0\",\"result\":{\"capabilities\":{\"textDocumentSync\":2,\"hoverProvider\":true,\"completionProvider\":{\"resolveProvider\":false,\"triggerCharacters\":[\"::\"]},\"definitionProvider\":true,\"referencesProvider\":false,\"documentHighlightProvider\":false,\"documentSymbolProvider\":false,\"workspaceSymbolProvider\":false,\"codeActionProvider\":false,\"documentFormattingProvider\":false,\"documentRangeFormattingProvider\":false,\"renameProvider\":false}}}");
            Assert.IsTrue(session.initialized_notification == "{\"jsonrpc\":\"2.0\",\"method\":\"initialized\"}");
            Assert.IsTrue(session.shutdown == "{\"jsonrpc\":\"2.0\",\"id\":\"2\",\"method\":\"shutdown\"}");
            Assert.IsTrue(session.exit == "{\"jsonrpc\":\"2.0\",\"method\":\"exit\"}");

            Assert.IsTrue(session.client_in_initialize_messages.Count == 0);
            Assert.IsTrue(session.client_in_start_messages.Count == 2);
            Assert.IsTrue(session.client_in_start_messages[0] == "{\"jsonrpc\":\"2.0\",\"method\":\"$/setTraceNotification\",\"params\":\"false\"}");
            Assert.IsTrue(session.client_in_start_messages[1] == "{\"jsonrpc\":\"2.0\",\"method\":\"workspace/didChangeConfiguration\",\"params\":{\"settings\":[\"C:\\\\Users\\\\MAYANJE\\\\Source\\\\Repos\\\\TypeCobol10\\\\TypeCobol\\\\bin\\\\Debug\\\\TypeCobol.CLI.exe\",\"-1\",\"-s\",\"C:\\\\TypeCobol\\\\Sources\\\\##Latest_Release##\\\\skeletons.xml\",\"-e\",\"rdz\",\"-c\",\"C:\\\\TypeCobol\\\\Copys\",\"-y\",\"C:\\\\TypeCobol\\\\Sources\\\\##Latest_Release##\\\\Intrinsic\\\\Intrinsic.txt\",\"-t\",\"--dependencies\\u003dC:\\\\TypeCobol\\\\Sources\\\\##Latest_Release##\\\\Dependencies\\\\*.tcbl\",\"--autoremarks\",\"-md\",\"40\"]}}");

            Assert.IsTrue(session.server_in_initialize_messages.Count == 0);

            Assert.IsTrue(session.server_in_start_messages.Count == 3);
            Assert.IsTrue(session.server_in_start_messages[0] == "{\"jsonrpc\":\"2.0\",\"method\":\"textDocument/publishDiagnostics\",\"params\":{\"uri\":\"file:///C:/Users/MAYANJE/AppData/Local/Temp/tcbl/PARA13105124614291384263.cee\",\"diagnostics\":[]}}");
            Assert.IsTrue(session.server_in_start_messages[1] == "{\"jsonrpc\":\"2.0\",\"method\":\"window/logMessage\",\"params\":{\"type\":4,\"message\":\"Opened source file : C:\\\\Users\\\\MAYANJE\\\\AppData\\\\Local\\\\Temp\\\\tcbl\\\\PARA13105124614291384263.cee\"}}");
            Assert.IsTrue(session.server_in_start_messages[2] == "{\"jsonrpc\":\"2.0\",\"method\":\"window/logMessage\",\"params\":{\"type\":4,\"message\":\"Closed source file : C:\\\\Users\\\\MAYANJE\\\\AppData\\\\Local\\\\Temp\\\\tcbl\\\\PARA13105124614291384263.cee\"}}");

            Assert.IsTrue(session.scripts.Count == 1);
            Assert.IsTrue(session.scripts[0] == "C:\\Users\\MAYANJE\\Source\\LSRSessions\\Session2017_11_16_11_03_45_034\\file__C__Users_MAYANJE_AppData_Local_Temp_tcbl_PARA13105124614291384263_cee_2017_11_16_11_03_45_382.tlsp");
        }

    }
}
