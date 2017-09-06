/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

using LanguageServer.JsonRPC;

namespace LanguageServer.Protocol
{
    /// <summary>
    /// A request to resolve a command for a given code lens.
    /// </summary>
    public class CodeLensResolveRequest
    {
        public static readonly RequestType Type = new RequestType("codeLens/resolve", typeof(CodeLens), typeof(CodeLens), null);
    }
}
