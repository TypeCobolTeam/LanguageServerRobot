﻿/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

using System.Collections.Generic;
using LanguageServer.JsonRPC;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// Request to request completion at a given text document position. The request's
    /// parameter is of type[TextDocumentPosition](#TextDocumentPosition) the response
    /// is of type[CompletionItem[]](#CompletionItem) or a Thenable that resolves to such.
    /// </summary>
    public class CompletionRequest
    {
        public readonly static RequestType Type = new RequestType("textDocument/completion", typeof(TextDocumentPosition), typeof(List<CompletionItem>), null);
    }
}
