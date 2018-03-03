using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeCobol.LanguageServer.Robot.Common.Pipe;

namespace TypeCobol.LanguageServer.Robot.Common.Connection
{
    /// <summary>
    /// The Data connection factory.
    /// </summary>
    public class DataConnectionfactory
    {
        /// <summary>
        /// The Kind of connection provided by  this factory
        /// </summary>
        public enum ConnectionType
        {
            PIPE,
        }

        /// <summary>
        /// The connection side type.
        /// </summary>
        public enum ConnectionSide
        {
            Consumer,
            Producer,
        }

        /// <summary>
        /// Create a new connection object.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns>The connection objet if any, null otherwise</returns>
        public static IDataConnection Create(ConnectionType type, ConnectionSide side)
        {
            switch (type)
            {
                case ConnectionType.PIPE:
                    switch (side)
                    {
                        case ConnectionSide.Consumer:
                            return new ConsumerPipeConnection();
                        case ConnectionSide.Producer:
                            return new ProducerPipeConnection();
                    }
                    break;
            }
            return null;
        }
    }
}
