/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

using LanguageServer.JsonRPC;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// The exit event is sent from the client to the server to
    /// ask the server to exit its process.
    /// </summary>
    public class ExitNotification
    {
        public static readonly NotificationType Type = new NotificationType("exit", null);
    }
}
