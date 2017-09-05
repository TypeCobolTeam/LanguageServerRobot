using System;
using System.IO;
using LanguageServer.JsonRPC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Tests.LanguageServer.JsonRPC
{
    [TestClass]
    public class StreamMessageProducerTest
    {
        /// <summary>
        /// Test the listening/producing of a single message from the Stream Message Producer
        /// </summary>
        [TestMethod]
        public void ProduceHelloMessage()
        {
            //Create the JSon message
            JObject jsonMessage = new JObject();
            TestUtilities.PrepareJsonPRCMessage(jsonMessage);
            jsonMessage["test"] = "Hello Stream Message Producer";
            //Createa RW Memory Stream and write in it a Json message
            MemoryStream stream = new MemoryStream();
            string message = jsonMessage.ToString();
            TestUtilities.WriteJsonMessage(stream, message);
            // Seek to the begining so that the producer can read the message
            stream.Seek(0, SeekOrigin.Begin);

            //Create the producer to read in the stream.
            StreamMessageProducer stream_message = new StreamMessageProducer(stream);
            //Create a delegate message consumer read by the producer
            string result_msg = null;
            DelegateMessageConsumer consumer = new DelegateMessageConsumer(
                (string msg) =>
                {
                    result_msg = msg;
                }
                );
            //Let the producer listening
            var task = stream_message.Listen(consumer);
            //Test resulting message.
            Assert.AreEqual(message, result_msg);
        }
    }
}
