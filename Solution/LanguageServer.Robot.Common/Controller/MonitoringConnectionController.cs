using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.Robot.Common.Connection;

namespace LanguageServer.Robot.Common.Controller
{
    /// <summary>
    /// The Monitoring class for handle monitoring message dispatching from
    /// the consumer point of view.
    /// </summary>
    public class MonitoringConnectionController
    {
        /// <summary>
        /// The consumer
        /// </summary>
        public MonitoringConsumerController Consumer
        {
            get;
            private set;
        }

        /// <summary>
        /// Connection/Consumer constructor
        /// </summary>
        /// <param name="consumer">The Consumer Controller</param>
        public MonitoringConnectionController(MonitoringConsumerController consumer)
        {     
            this.Consumer = consumer;
        }

        /// <summary>
        /// Start the asynchronous task of listening to the DataConnection and dispatching
        /// consumer messages.
        /// </summary>
        /// <returns></returns>
        public async Task<MonitoringConsumerController.ConnectionState> Start()
        {
            return await Consumer.Start();
        }
    }
}
