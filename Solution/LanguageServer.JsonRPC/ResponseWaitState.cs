using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// Utility class used to store all the necessary elements to handle the response to a request
    /// </summary>
    public class ResponseWaitState
    {
        public ResponseWaitState(RequestType requestType, string requestId, TaskCompletionSource<ResponseResultOrError> taskCompletionSource)
        {
            this.RequestType = requestType;
            this.RequestId = requestId;
            this.TaskCompletionSource = taskCompletionSource;
        }

        public RequestType RequestType { get; private set; }

        public string RequestId { get; private set; }

        public TaskCompletionSource<ResponseResultOrError> TaskCompletionSource { get; private set; }
    }
}
