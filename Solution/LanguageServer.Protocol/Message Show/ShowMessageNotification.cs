﻿/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

using LanguageServer.JsonRPC;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// The show message notification is sent from a server to a client to ask
    /// the client to display a particular message in the user interface.
    /// </summary>
    public class ShowMessageNotification
    {
        public static readonly NotificationType Type = new NotificationType("window/showMessage", typeof(ShowMessageParams));
    }
}
