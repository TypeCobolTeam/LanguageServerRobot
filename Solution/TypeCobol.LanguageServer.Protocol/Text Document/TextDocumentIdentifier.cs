/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

namespace TypeCobol.LanguageServer.Protocol
{
    /// <summary>
    /// A literal to identify a text document in the client.
    /// </summary>
    public class TextDocumentIdentifier
    {
        /// <summary>
        /// The text document's uri.
        /// </summary>
        public string uri { get; set; }

        /// <summary>
        /// Creates a new TextDocumentIdentifier literal.
        /// </summary>
        /// <param name="uri">The document's uri.</param>
        public TextDocumentIdentifier(string uri)
        {
            this.uri = uri;
        }
    }
}
