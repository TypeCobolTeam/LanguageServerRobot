using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// Class of an insatnce that produces message on a Stream.
    /// </summary>
    public class StreamMessageProducer : IMessageProducer
    {
        /// <summary>
        /// Configure the Producer on the Console Input Stream
        /// </summary>
        public StreamMessageProducer()
        {
            Console.InputEncoding = Encoding.UTF8;
            InputStream = Console.OpenStandardInput();
        }

        /// <summary>
        /// Configure the Producer on the given Input Stream
        /// <param name="stream">The Input Stream</param>
        /// </summary>
        public StreamMessageProducer(Stream stream)
        {            
            InputStream = stream;
        }

        /// <summary>
        /// Message Buffer max size.
        /// </summary>
        private const int BufferSize = 8192;

        /// <summary>
        /// The Input Stream
        /// </summary>
        public Stream InputStream
        {
            get;
            set;
        }
        /// <summary>
        /// The message encoding used
        /// </summary>
        public Encoding MessageEncoding
        {get; set;}

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

        private int _ShutdownAfterNextMessage;

        /// <summary>
        /// Is the Server request an Exit
        /// </summary>
        public bool ShutdownAfterNextMessage
        {
            get
            {
                return System.Threading.Interlocked.Exchange(ref _ShutdownAfterNextMessage, _ShutdownAfterNextMessage) == 1;
            }
            set
            {
                System.Threading.Interlocked.Exchange(ref _ShutdownAfterNextMessage, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Structure of Messages Headers
        /// </summary>
        protected class Headers
        {
            public int contentLength;
            public string charset;
            public Headers()
            {
                Reset();
            }

            public void Reset()
            {
                this.contentLength = -1;
                this.charset = Encoding.UTF8.BodyName;
            }
        }

        /// <summary>
        /// Parse the Message Header Line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="headers"></param>
        /// <returns></returns>

        protected void ParseHeader(String line, Headers headers)
        {
            int sepIndex = line.IndexOf(':');
            if (sepIndex >= 0)
            {
                String key = line.Substring(0, sepIndex).Trim();
                switch (key)
                {
                    case JsonMessageConstants.ContentLengthHeader:
                        Int32.TryParse(line.Substring(sepIndex + 1).Trim(), out headers.contentLength);
                        break;
                    case JsonMessageConstants.ContentTypeHeader:
                        {
                            int charsetIndex = line.IndexOf("charset=");
                            if (charsetIndex >= 0)
                                headers.charset = line.Substring(charsetIndex + 8).Trim();
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Handle a Received message int Input Stream
        /// </summary>
        /// <param name="messageConsumer">The Message consumer</param>
        /// <param name="headers">The Message Header</param>
        /// <param name="buffer">The read buffer</param>
        /// <returns>true if the message has been handled, false otherwise</returns>
        protected bool HandleMessage(IMessageConsumer messageConsumer, Headers headers, byte[] buffer)
        {
            // If the server could not find the content length of the message
            // it is impossible to detect where the message ends : write a fatal 
            // error message and exit the loop
            if (headers.contentLength == 0)
            {
                LogWriter?.WriteLine($"{DateTime.Now} !! Fatal error : message without Content-Length header");
                return false;
            }
            else
            {
                MessageLogWriter?.WriteLine(
                    $"{DateTime.Now} >> Message received : Content-Length={headers.contentLength}");
            }

            // Read Http message body
            using (MemoryStream stream = new MemoryStream(headers.contentLength))
            {
                while (headers.contentLength > 0)
                {
                    int nbCharsToRead = headers.contentLength > buffer.Length ? buffer.Length : headers.contentLength;
                    int nbCharsRead = InputStream.Read(buffer, 0, nbCharsToRead);
                    stream.Write(buffer, 0, nbCharsRead);
                    headers.contentLength -= nbCharsRead;
                }
                Encoding encoding = headers.charset == Encoding.UTF8.BodyName ? Encoding.UTF8 : Encoding.GetEncoding(headers.charset);
                if (encoding == null)
                {
                    LogWriter?.WriteLine(
                        $"{DateTime.Now} >> Fail to get encoding : {headers.charset} --> using default encoding UTF-8");
                    encoding = Encoding.UTF8;
                }
                string message = encoding.GetString(stream.ToArray()); ;
                ProtocolLogWriter?.WriteLine(message);
                ProtocolLogWriter?.WriteLine("----------");

                // Handle incoming message and optionnaly send reply
                messageConsumer.Consume(message);
            }
            return true;
        }
        /// <summary>
        /// Infinite message loop : start listening for incoming Json messages on Input Stream.
        /// Each message received is passed to an IMessageConsumer on the current thread of the loop.
        /// This method blocks the current thread until Listen() is called. 
        /// </summary>
        public void Listen(IMessageConsumer messageConsumer)
        {
            LogWriter?.WriteLine($"{DateTime.Now} -- Server startup");

            StringBuilder headerBuilder = null;
            bool newLine = false;
            Headers headers = new Headers();
            // Infinite message loop
            using (InputStream)
            {
                byte[] buffer = new byte[BufferSize];
                for (;;)
                {
                    // Receive and handle one message
                    try
                    {
                        // Read Http message headers                  
                        int c = InputStream.ReadByte();
                        if (c == -1)
                            break;//End of input stream has been reached???
                        else
                        {
                            if (c == '\n')
                            {
                                if (newLine)
                                {
                                    // Two consecutive newlines have been read, which signals the start of the message content
                                    if (headers.contentLength < 0)
                                    {
                                        LogWriter?.WriteLine(
                                            $"{DateTime.Now} !! Fatal error : message without Content-Length header");
                                        break;
                                    }
                                    else
                                    {
                                        bool result = HandleMessage(messageConsumer, headers, buffer);
                                        if (!result)
                                            break;
                                        newLine = false;
                                    }
                                    headers.Reset();
                                }
                                else if (headerBuilder != null)
                                {
                                    // A single newline ends a header line
                                    ParseHeader(headerBuilder.ToString(), headers);
                                    headerBuilder = null;
                                }
                                newLine = true;
                            }
                            else if (c != '\r')
                            {
                                // Add the character to the current header line
                                if (headerBuilder == null)
                                    headerBuilder = new StringBuilder();
                                headerBuilder.Append((char)c);
                                newLine = false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogWriter?.WriteLine($"{DateTime.Now} !! Exception : {e.Message}");
                    }

                    // Exit the loop after message handling if a shutdown of the server has been requested
                    if (ShutdownAfterNextMessage)
                    {
                        break;
                    }
                }
            }

            LogWriter?.WriteLine($"{DateTime.Now} -- Server shutdown");
        }
    }
}
