/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

using LanguageServer.JsonRPC;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// The initialize method is sent from the client to the server.
    /// It is send once as the first method after starting up the
    /// worker.The requests parameter is of type [InitializeParams](#InitializeParams)
    /// the response if of type [InitializeResult](#InitializeResult) of a Thenable that
    /// resolves to such.
    /// </summary>
    public class InitializeRequest
    {
        public static readonly RequestType Type = new RequestType("initialize", typeof(InitializeParams), typeof(InitializeResult), typeof(InitializeError));
    }
}
