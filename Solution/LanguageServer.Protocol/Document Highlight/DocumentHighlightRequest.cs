/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

using System.Collections.Generic;
using LanguageServer.JsonRPC;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// Request to resolve a [DocumentHighlight](#DocumentHighlight) for a given
    /// text document position.The request's parameter is of type [TextDocumentPosition]
    /// (#TextDocumentPosition) the request reponse is of type [DocumentHighlight[]]
    /// (#DocumentHighlight) or a Thenable that resolves to such.
    /// </summary>
    public class DocumentHighlightRequest
    {
        public static readonly RequestType Type = new RequestType("textDocument/documentHighlight", typeof(TextDocumentPosition), typeof(List<DocumentHighlight>), null);
    }
}
