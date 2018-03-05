using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeCobol.LanguageServer.JsonRPC
{
    /// <summary>
    /// Json message predefined constants
    /// </summary>
    public static class JsonMessageConstants
    {
        public const string JsonRpcVersion = "2.0";
        public const string ContentLengthHeader = "Content-Length";
        public const string ContentTypeHeader = "Content-Type";
        public const string JsonMimeType = "application/json";
        public const string CrLf = "\r\n";
    }
}
