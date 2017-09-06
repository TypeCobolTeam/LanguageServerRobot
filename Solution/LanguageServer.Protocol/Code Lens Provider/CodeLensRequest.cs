/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

using System.Collections.Generic;
using LanguageServer.JsonRPC;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// A request to provide code lens for the given text document.
    /// </summary>
    public class CodeLensRequest
    {
        public static readonly RequestType Type = new RequestType("textDocument/codeLens", typeof(TextDocumentIdentifier), typeof(List<CodeLens>), null);
    }
}
