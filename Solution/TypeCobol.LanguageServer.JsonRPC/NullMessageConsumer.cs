using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeCobol.LanguageServer.JsonRPC
{
    /// <summary>
    /// A Message Consumer that does nothing.
    /// </summary>
    public class NullMessageConsumer : IMessageConsumer
    {
        public void Consume(string message)
        {            
        }
    }
}
