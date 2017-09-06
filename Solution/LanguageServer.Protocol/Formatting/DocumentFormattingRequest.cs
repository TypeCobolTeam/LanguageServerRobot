﻿/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

using System.Collections.Generic;
using LanguageServer.JsonRPC;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// A request to to format a whole document.
    /// </summary>
    public class DocumentFormattingRequest
    {
        public static readonly RequestType Type = new RequestType("textDocument/formatting", typeof(DocumentFormattingParams), typeof(List<TextEdit>), null);
    }
}
