﻿/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

namespace TypeCobol.LanguageServer.Protocol
{
    public class DocumentRangeFormattingParams
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        public TextDocumentIdentifier textDocument { get; set; }

        /// <summary>
        /// The range to format
        /// </summary>
        public Range range { get; set; }

        /// <summary>
        /// The format options
        /// </summary>
        public FormattingOptions options { get; set; }
    }
}
