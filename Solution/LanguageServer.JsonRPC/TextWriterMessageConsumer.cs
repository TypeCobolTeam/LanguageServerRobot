using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// A String Message consumer based on a TextWriter.
    /// </summary>
    public class TextWriterMessageConsumer : IMessageConsumer
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public TextWriterMessageConsumer() : this(null)
        {            
        }

        /// <summary>
        /// TextWriter instance constructor
        /// </summary>
        /// <param name="writer">The Text writer instance</param>
        public TextWriterMessageConsumer(TextWriter writer)
        {
            this.Writer = writer;
            WriterLock = new object();
        }

        private object WriterLock
        {
            get; set;
        }

        /// <summary>
        /// The TextWriter getter/setter
        /// </summary>
        public TextWriter Writer
        {get; set; }

        /// <summary>
        /// General Log TextWriter
        /// </summary>
        public TextWriter LogWriter { get; set; }

        /// <summary>
        /// Protocol Log TextWriter
        /// </summary>
        public TextWriter ProtocolLogWriter { get; set; }

        /// <summary>
        /// Message Log TextWriter
        /// </summary>
        public TextWriter MessageLogWriter { get; set; }

        /// <summary>
        /// Compute a JSon header corresponding to a content length and with charset name
        /// the Encoding name of the writer instance.
        /// </summary>
        /// <param name="contentLength">The content length</param>
        /// <returns>The Json header</returns>
        internal string JsonHeader(int contentLength)
        {
            StringBuilder headerBuffer = new StringBuilder();
            headerBuffer.AppendFormat("{0}:{1}{2}", JsonMessageConstants.ContentLengthHeader, contentLength, JsonMessageConstants.CrLf);
            if (Writer != null)
            {
                if (!Writer.Encoding.EncodingName.Equals(Encoding.UTF8.EncodingName))
                {
                    headerBuffer.AppendFormat("{0}:{1}; charset={2}{3}", JsonMessageConstants.ContentTypeHeader, 
                        JsonMessageConstants.JsonMimeType, Writer.Encoding.EncodingName, JsonMessageConstants.CrLf);
                }
            }
            headerBuffer.Append(JsonMessageConstants.CrLf);
            return headerBuffer.ToString();
        }

        /// <summary>
        /// Consume a message as a Json message by completing it with a Json content header.
        /// This message assume that the message already has the jsonrc version field set.
        /// </summary>
        /// <param name="message">The text message to be consumed</param>
        public void Consume(string message)
        {
            if (Writer != null && message != null)
            {
                int contentLength = Writer.Encoding.GetByteCount(message);
                String jsonHeader = JsonHeader(contentLength);
                lock(WriterLock)
                {
                    Writer.Write(jsonHeader);
                    MessageLogWriter?.WriteLine($"{DateTime.Now} << Message sent : Content-Length={contentLength}");
                    Writer.Write(message);
                    Writer.Flush();
                    ProtocolLogWriter?.WriteLine(message);
                    ProtocolLogWriter?.WriteLine("----------");
                }
            }
        }
    }
}
