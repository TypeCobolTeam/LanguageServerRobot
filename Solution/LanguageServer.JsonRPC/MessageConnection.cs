using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// This class implements the Producer/Consumer message communication DataFlow concept.
    /// </summary>
    public class MessageConnection : IMessageConnection
    {
        /// <summary>
        /// Constructor with Stream instance as InputStream Reader(producer's stream)
        /// and a TextWriter as writter (consumer's writer).
        /// </summary>
        /// <param name="reader">The Input Stream reader</param>
        /// <param name="writer">The TextWriter instance</param>
        public MessageConnection(Stream reader, TextWriter writer)
        {

        }

        /// <summary>
        /// Constructor with a producer instance (reader) and a consumer instance (writer)
        /// </summary>
        /// <param name="reader">The producer instance reader</param>
        /// <param name="writer">The consumer instance writer</param>
        public MessageConnection(StreamMessageProducer producer, TextWriterMessageConsumer consumer)
        {
            if (producer == null)
                throw new NullReferenceException("producer is null");
            if (consumer == null)
                throw new NullReferenceException("consumer is null");
            Producer = producer;
            Consumer = consumer;
        }

        /// <summary>
        /// Request a shutdown of the server after handling the current or the next message
        /// </summary>
        public bool ShutdownAfterNextMessage
        {
            get { return Producer.ShutdownAfterNextMessage; }
            set { Producer.ShutdownAfterNextMessage = value; }
        }
        public StreamMessageProducer Producer { get; private set; }
        public TextWriterMessageConsumer Consumer { get; private set; }

        public void SendMessage(string message)
        {
            Consumer.Consume(message); ;
        }

        public void WriteConnectionLog(string trace)
        {
            throw new NotImplementedException();
        }
    }
}
