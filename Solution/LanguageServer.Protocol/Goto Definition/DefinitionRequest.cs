/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

using LanguageServer.JsonRPC;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// A request to resolve the defintion location of a symbol at a given text
    /// document position.The request's parameter is of type [TextDocumentPosition]
    /// (#TextDocumentPosition) the response is of type [Definition](#Definition) or a
    /// Thenable that resolves to such.
    /// </summary>
    public class DefinitionRequest
    {
        public static readonly RequestType Type = new RequestType("textDocument/definition", typeof(TextDocumentPosition), typeof(Definition), null);
    }
}
