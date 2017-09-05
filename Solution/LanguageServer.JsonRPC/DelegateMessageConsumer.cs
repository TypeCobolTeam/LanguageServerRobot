using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// A Class that encapsulate a delegate to a message consumer
    /// </summary>
    public class DelegateMessageConsumer : IMessageConsumer
    {
        public delegate void Consumer(string message);
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="consumer">The delegated consumer</param>
        public DelegateMessageConsumer(Consumer consumer)
        {
            System.Diagnostics.Contracts.Contract.Assert(consumer != null);
            Delegate = consumer;
        }

        /// <summary>
        /// Delegated consumer
        /// </summary>
        public Consumer Delegate
        {
            get;
            set;
        }
        void IMessageConsumer.Consume(string message)
        {
            System.Diagnostics.Contracts.Contract.Assert(Delegate != null);
            Delegate?.Invoke(message);
        }
    }
}
