using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeCobol.LanguageServer.JsonRPC
{
    /// <summary>
    /// A String Message consumer based on a Stream.
    /// </summary>
    public class StreamMessageConsumer : IMessageConsumer, IConnectionLog, IDisposable
    {
        /// <summary>
        /// Empty constructor on the Console Output
        /// </summary>
        public StreamMessageConsumer(Encoding encoding = null) : this(Console.OpenStandardOutput(), encoding)
        {
        }

        /// <summary>
        /// Stream instance constructor
        /// </summary>
        /// <param name="writer">The Stream instance</param>
        public StreamMessageConsumer(Stream writer, Encoding encoding = null)
        {
            System.Diagnostics.Contracts.Contract.Assert(writer != null);
            Encoding = encoding ?? Encoding.UTF8;
            this.Writer = writer;
            WriterLock = new object();
        }

        private object WriterLock
        {
            get; set;
        }

        /// <summary>
        /// The target encoding
        /// </summary>
        public Encoding Encoding
        {
            get;
            private set;
        }
        /// <summary>
        /// The Stream getter/setter
        /// </summary>
        public Stream Writer
        {get; private set; }

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
            if (!this.Encoding.BodyName.Equals(Encoding.UTF8.BodyName))
            {
                headerBuffer.AppendFormat("{0}:{1}; charset={2}{3}", JsonMessageConstants.ContentTypeHeader,
                JsonMessageConstants.JsonMimeType, this.Encoding.BodyName, JsonMessageConstants.CrLf);
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
            System.Diagnostics.Contracts.Contract.Assert(Writer != null);
            if (Writer != null && message != null)
            {
                int contentLength = this.Encoding.GetByteCount(message);
                String jsonHeader = JsonHeader(contentLength);
                lock(WriterLock)
                {
                    byte[] data = Encoding.ASCII.GetBytes(jsonHeader);
                    Writer.Write(data, 0, data.Length);
                    MessageLogWriter?.WriteLine($"{DateTime.Now} << Message sent : Content-Length={contentLength}");
                    data = this.Encoding.GetBytes(message);
                    Writer.Write(data, 0, data.Length);
                    Writer.Flush();
                    ProtocolLogWriter?.WriteLine(message);
                    ProtocolLogWriter?.WriteLine("----------");
                }
            }
        }
        /// <summary>
        /// Propagate Connection Log settings to this.
        /// </summary>
        /// <param name="log">The Connection Logs setting.</param>
        public void PropagateConnectionLogs(ConnectionLog log = null)
        {
            log = log ?? ConnectionLog.GetInstance();
            log.AssignTo(this);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
