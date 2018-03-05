using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeCobol.LanguageServer.JsonRPC
{
    /// <summary>
    /// The Connection State enumeration.
    /// </summary>
    public enum ConnectionState
    {
        New, Listening, Closed, Disposed
    }
}
