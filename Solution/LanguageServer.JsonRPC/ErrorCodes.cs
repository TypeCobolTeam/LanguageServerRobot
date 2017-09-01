﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// Predefined error codes.
    /// </summary>
    enum ErrorCodes : int
    {
        ParseError = -32700,
        InvalidRequest = -32600,
        MethodNotFound = -32601,
        InvalidParams = -32602,
        InternalError = -32603,
        serverErrorStart = -32099,
        serverErrorEnd = -32000
    }
}
