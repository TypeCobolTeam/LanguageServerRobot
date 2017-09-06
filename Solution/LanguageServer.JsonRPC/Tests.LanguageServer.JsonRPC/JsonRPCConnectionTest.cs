using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Tests.LanguageServer.JsonRPC
{
    [TestClass]
    public class JsonRPCConnectionTest
    {
        /// <summary>
        /// This Test, tests that one can wait for the reponse of a request, using JsonRPCConnection
        /// mechanism.
        /// </summary>
        [TestMethod]
        public void RequestWaitRespond()
        {
            //Create Message Connection instance
            MemoryStream writer = new MemoryStream();
            StreamMessageProducer producer = new StreamMessageProducer(writer);
            StreamMessageConsumer consumer = new StreamMessageConsumer(writer);
            MessageConnection connection = new MessageConnection(producer, consumer);

            //Create a Json Message Connection instance
            JsonRPCConnection rpcConnect = new JsonRPCConnection(connection);
            Task<ResponseResultOrError> Result10 = null;
            int nMsgCount = 0;
            //Delegate consumer of Producer messages.
            DelegateMessageConsumer delegator = new DelegateMessageConsumer(
                (string message) =>
                {
                    int index = message.IndexOf("Message_");
                    if (index >= 0)
                    {
                        JObject jsonObject = JObject.Parse(message);
                        // Try to read the JsonRPC message properties
                        string requestId = (string)jsonObject["id"];
                        string method = (string)jsonObject["method"];
                        JToken parameters = jsonObject["params"];
                        JToken result = jsonObject["result"];
                        JToken error = jsonObject["error"];
                        //==> insert the reponse AT THE END OF THE STREAM.
                        //SO KEEP The current position and go to end
                        long current_pos = writer.Position;
                        writer.Seek(0, SeekOrigin.End);
                        //Insert the result ==> send a replace directly in the Json rpc Connection.
                        ResponseResultOrError rre = new ResponseResultOrError();
                        StringBuilder sb = new StringBuilder("Result_");
                        for (int i = index + "Message_".Length; Char.IsDigit(message[i]); i++)
                            sb.Append(message[i]);
                        rre.result = sb.ToString();
                        TestUtilities.SendReply(requestId, rre, rpcConnect);
                        //Go back to the original position
                        writer.Seek(current_pos, SeekOrigin.Begin);
                    }
                    //We got A response ==> forward it back to the JsonRPCConnection so that he can treat it
                    if (message.IndexOf("Result_") >= 0)
                    {
                        rpcConnect.HandleMessage(message, connection);
                    }
                    else
                    {
                        nMsgCount++;
                    }
                }
                );

            RequestType myRequestType = new RequestType(
                "RequestWaitRespond",
                typeof(string),
                typeof(string),
                typeof(string)
                );
            //Infinite Dialogs            
            for (int i = 0; i < 50 ; i++)
            {
                Task<ResponseResultOrError> result = rpcConnect.SendRequest(myRequestType, "Message_" + i);
                if (i == 10)
                    Result10 = result;//We are interresting by the 10th request
                //Start listening after 20 messages
                if (i == 20)
                {
                    //IMPORTANT go to the begining of the Stream.
                    writer.Seek(0, SeekOrigin.Begin);
                    rpcConnect.Start(delegator);
                    //Wait for result 10
                    ResponseResultOrError answer10 = Result10.Result;
                    Assert.AreEqual(nMsgCount, 21);
                    Assert.AreEqual("Result_10", answer10.result);
                    break;//STOP ALL
                }
            }
        }
    }
}
