using TypeCobol.LanguageServer.JsonRPC;

namespace TypeCobol.LanguageServer.Protocol
{
    internal class WorkspaceExecuteCommandRequest
    {
        public static readonly RequestType Type = new RequestType("workspace/executeCommand", typeof(ExecuteCommandParams), typeof(object), null);
    }
}
