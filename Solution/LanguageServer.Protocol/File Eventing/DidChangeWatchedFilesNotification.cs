﻿/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

using LanguageServer.JsonRPC;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// The watched files notification is sent from the client to the server when
    /// the client detects changes to file watched by the lanaguage client.
    /// </summary>
    public class DidChangeWatchedFilesNotification
    {
        public static readonly NotificationType Type = new NotificationType("workspace/didChangeWatchedFiles", typeof(DidChangeWatchedFilesParams));
    }
}
