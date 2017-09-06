﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// An item to transfer a text document from the client to the server.
    /// </summary>
    public class TextDocumentItem
    {
        /// <summary>
        /// The text document's uri.
        /// </summary>
        public String uri;  
        /// <summary>
        /// The text document's language identifier
        /// </summary>
        public String languageId;
        /// <summary>
        /// The version number of this document (it will strictly increase after each change, including undo/redo).
        /// </summary>
        public int version;
        /// <summary>
        /// The content of the opened  text document.
        /// </summary>
        public String text;

        public TextDocumentItem()
        {

        }
    }
}
