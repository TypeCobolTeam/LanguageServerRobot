/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

using LanguageServer.JsonRPC;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// Diagnostics notification are sent from the server to the client to signal
    /// results of validation runs.
    /// </summary>
    public class PublishDiagnosticsNotification
    {
        public static readonly NotificationType Type = new NotificationType("textDocument/publishDiagnostics", typeof(PublishDiagnosticsParams));
    }
}
