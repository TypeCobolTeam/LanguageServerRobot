using System;
using System.IO;
using TypeCobol.LanguageServer.JsonRPC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Tests.LanguageServer.JsonRPC
{
    [TestClass]
    public class StreamMessageConsumerTest
    {
        /// <summary>
        /// Test the consuming of a single message by the Stream Message Consumer instance.
        /// </summary>
        [TestMethod]
        public void ConsumeHelloMessage()
        {
            JObject jsonMessage = new JObject();
            TestUtilities.PrepareJsonPRCMessage(jsonMessage);
            jsonMessage["test"] = "Hello Stream Message Consumer";
            MemoryStream in_stream = new MemoryStream();
            string message = jsonMessage.ToString();            
            TestUtilities.WriteJsonMessage(in_stream, message);
            MemoryStream out_stream = new MemoryStream();
            StreamMessageConsumer textwriter_message = new StreamMessageConsumer(out_stream);
            textwriter_message.Consume(message);
            Assert.AreEqual(in_stream.Length, out_stream.Length);
            Assert.AreEqual(in_stream.Position, out_stream.Position);
            byte[] data_in = in_stream.GetBuffer();
            byte[] data_out = out_stream.GetBuffer();
            for (int i = 0; i < in_stream.Length; i++)
            {
                Assert.AreEqual(data_in[i], data_out[i]);
            }            
        }
    }
}
