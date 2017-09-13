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
    public class MessageConnection : IMessageConnection, IConnectionLog
    {
        /// <summary>
        /// A Connection exception class.
        /// </summary>
        public class ConnectionException : Exception
        {
            /// <summary>
            /// The Exception error enumeration.
            /// </summary>
            public enum Error
            {
                /**
                 * The connection is closed.
                 */
                Closed,
                /**
                 * The connection got disposed.
                 */
                Disposed,
                /**
                 * The connection is already in listening mode.
                 */
                AlreadyListening,
            }

            /// <summary>
            /// Error code constructor
            /// </summary>
            /// <param name="error">Error code</param>
            public ConnectionException(Error error)
            {
                ErrorCode = error;
            }

            /// <summary>
            /// Error code + message constructor.
            /// </summary>
            /// <param name="error"></param>
            /// <param name="message"></param>
            public ConnectionException(Error error, String message) : base(message)
            {
                ErrorCode = error;
            }

            public Error ErrorCode
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// Default Message Connection constructor using current application
        /// Standard Input/Ouput Streams.
        /// </summary>
        public MessageConnection() : this(new StreamMessageProducer(), new StreamMessageConsumer())
        {

        }
        /// <summary>
        /// Constructor with Stream instance as InputStream Reader(producer's stream)
        /// and a TextWriter as writter (consumer's writer).
        /// </summary>
        /// <param name="reader">The Input Stream reader</param>
        /// <param name="writer">The Output Stream instance</param>
        public MessageConnection(Stream reader, Stream writer)
            : this(reader != null ? new StreamMessageProducer(reader) : null, writer != null ? new StreamMessageConsumer(writer) : null)
        {
        }

        /// <summary>
        /// Constructor with a producer instance (reader) and a consumer instance (writer)
        /// </summary>
        /// <param name="reader">The producer instance reader</param>
        /// <param name="writer">The consumer instance writer</param>
        public MessageConnection(StreamMessageProducer producer, StreamMessageConsumer consumer)
        {
            if (producer == null)
                throw new NullReferenceException("producer is null");
            if (consumer == null)
                throw new NullReferenceException("consumer is null");
            Producer = producer;
            Consumer = consumer;
            this.State = ConnectionState.New;
        }

        /// <summary>
        /// Dummy constructor used by subclasses for initializinf nothing.
        /// </summary>
        /// <param name="Dummy"></param>
        protected MessageConnection(int Dummy)
        {
            this.State = ConnectionState.New;
        }
        /// <summary>
        /// Request a shutdown of the server after handling the current or the next message
        /// </summary>
        public bool ShutdownAfterNextMessage
        {
            get
            {
                System.Diagnostics.Contracts.Contract.Assert(Producer != null);
                return Producer.ShutdownAfterNextMessage;
            }
            set
            {
                System.Diagnostics.Contracts.Contract.Assert(Producer != null);
                Producer.ShutdownAfterNextMessage = value;
            }
        }

        private int myState;
        /// <summary>
        /// Current Connection State
        /// </summary>
        public ConnectionState State
        {
            get
            {
                return (ConnectionState)System.Threading.Interlocked.Exchange(ref myState, myState);
            }
            protected set
            {
                if (System.Threading.Interlocked.Exchange(ref myState, (int)value) != (int)value)
                {
                    if (StageChangedEvent != null)
                    {
                        StageChangedEvent(this, null);
                    }
                }
            }
        }

        event EventHandler StageChangedEvent;

        /// <summary>
        /// Add a State Change Event handler.
        /// </summary>
        public void AddStageChangedEventHandler(EventHandler handler)
        {
            StageChangedEvent += handler;
        }

        /// <summary>
        /// Remove a State Change Event Handler. 
        /// </summary>
        /// <param name="handler"></param>
        public void RemoveStageChangedEventHandler(EventHandler handler)
        {
            StageChangedEvent -= handler;
        }

        /// <summary>
        /// Is This connection in listening mode.
        /// </summary>
        public bool IsListening
        {
            get
            {
                return System.Threading.Interlocked.Exchange(ref myState, myState) == (int)ConnectionState.Listening;
            }
        }

        /// <summary>
        /// Is this connection closed
        /// </summary>
        /// <returns></returns>
        public bool IsClosed
        {
            get
            {
                return System.Threading.Interlocked.Exchange(ref myState, myState) == (int)ConnectionState.Closed;
            }
        }

        /// <summary>
        /// Is this connection disposed
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return System.Threading.Interlocked.Exchange(ref myState, myState) == (int)ConnectionState.Disposed;
            }
        }

        public StreamMessageProducer Producer { get; protected set; }
        public StreamMessageConsumer Consumer { get; protected set; }

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
        /// Throws an exception if the connection was Closed or Disposed.
        /// </summary>
        protected void ThrowIfClosedOrDisposed()
        {
            if (IsClosed) {
                throw new ConnectionException(ConnectionException.Error.Closed); //$NON-NLS-1$
            }
            if (IsDisposed) {
                throw new ConnectionException(ConnectionException.Error.Disposed); //$NON-NLS-1$
            }
        }

        /// <summary>
        /// Throws an error we are already in listening mode.
        /// </summary>
        protected void ThrowIfListening()
        {
		    if (IsListening) {
                throw new ConnectionException(ConnectionException.Error.AlreadyListening); //$NON-NLS-1$
            }
        }

        /// <summary>
        /// Start the connection, Listening of incoming message.
        /// </summary>
        /// <param name="messageConsumer">The message consumer of listened message</param>
        /// <returns>Return the Listening task.</returns>
        public virtual async Task<bool> Start(IMessageConsumer messageConsumer)
        {
            System.Diagnostics.Contracts.Contract.Assert(messageConsumer != null);
            System.Diagnostics.Contracts.Contract.Assert(Producer != null);            
            ThrowIfClosedOrDisposed();
            ThrowIfListening();
            State = ConnectionState.Listening;
            bool bResult = await Producer.Listen(messageConsumer);
            State = ConnectionState.Closed;
            return bResult;
        }

        /// <summary>
        /// Send a String message
        /// </summary>
        /// <param name="message">The message to be sent</param>
        public void SendMessage(string message)
        {
            System.Diagnostics.Contracts.Contract.Assert(Consumer != null);
            Consumer.Consume(message); ;
        }

        public void WriteConnectionLog(string trace)
        {
            LogWriter?.WriteLine(trace);
        }

        /// <summary>
        /// Propagate Connection Log settings to this.
        /// </summary>
        /// <param name="log">The Connection Logs setting.</param>
        public void PropagateConnectionLogs(ConnectionLog log = null)
        {
            log = log ?? ConnectionLog.GetInstance();
            log.AssignTo(this);
            Producer?.PropagateConnectionLogs(log);
            Consumer?.PropagateConnectionLogs(log);
        }
    }
}
