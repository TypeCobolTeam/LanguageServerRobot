﻿using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Tests.LanguageServer.JsonRPC
{
    [TestClass]
    public class MessageConnectionTest
    {
        /// <summary>
        /// Test that uses a MessageConnection instance.
        /// The Producer will read In and the Consumer will write Out.
        /// </summary>
        [TestMethod]
        public void CircularMessageConnection()
        {
            // The stack of
            System.Collections.Generic.Queue<string > send_messages = new System.Collections.Generic.Queue< string >();
            MemoryStream writer = new MemoryStream();
            StreamMessageProducer producer = new StreamMessageProducer(writer);
            StreamMessageConsumer consumer = new StreamMessageConsumer(writer);
            MessageConnection connection = new MessageConnection(producer, consumer);
            bool gotExit = false;
            int nReadMessage = 0;
            DelegateMessageConsumer delegator = new DelegateMessageConsumer(
                (string message) =>
                {
                    Assert.IsTrue(send_messages.Count > 0);
                    string sent_message = send_messages.Dequeue();
                    Assert.AreEqual(sent_message, message);
                    if (sent_message.IndexOf("?exit_it?") > 0)
                    {
                        gotExit = true;
                        connection.ShutdownAfterNextMessage = true;
                    }
                    nReadMessage++;
                }
                );
            //Enqueue and Send a set of test messages.
            for (int i = 0; i < 10; i++)
            {
                JObject jsonMessage = new JObject();
                TestUtilities.PrepareJsonPRCMessage(jsonMessage);
                jsonMessage["Msg"] = i == 5 ? "?exit_it?" : "Message " + i;
                string source_message = jsonMessage.ToString();
                send_messages.Enqueue(source_message);
                connection.SendMessage(source_message);
            }
            //Play all messages
            writer.Seek(0, SeekOrigin.Begin);
            //Start the Connection
            var mytask = connection.Start(delegator);
            WaitTask(mytask);
            Assert.IsTrue(nReadMessage == 6);
            Assert.IsTrue(gotExit);
        }

        public async void WaitTask(Task<bool> task)
        {
            bool b = await task.ConfigureAwait(false);
        }
    }
}
