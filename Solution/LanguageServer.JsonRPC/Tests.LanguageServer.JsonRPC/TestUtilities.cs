using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Tests.LanguageServer.JsonRPC
{
    /// <summary>
    /// Various static helper methods to help writting tests
    /// </summary>
    public class TestUtilities
    {
        public const string JsonRpcVersion = "2.0";
        public const string ContentLengthHeader = "Content-Length";
        public const string ContentTypeHeader = "Content-Type";
        public const string JsonMimeType = "application/json";
        public const string CrLf = "\r\n";

        // Add Json RPC standard property
        public static void PrepareJsonPRCMessage(JObject jsonMessage)
        {
            jsonMessage["jsonrpc"] = "2.0";
        }

        /// <summary>
        /// Compute a JSon header corresponding to a content length and with charset name
        /// the Encoding name of the writer instance.
        /// </summary>
        /// <param name="contentLength">The content length</param>
        /// <param name="encoding">An optional encoding name</param>
        /// <returns>The Json header</returns>
        public static string JsonHeader(int contentLength, string encoding = null)
        {
            StringBuilder headerBuffer = new StringBuilder();
            headerBuffer.AppendFormat("{0}:{1}{2}", ContentLengthHeader, contentLength, CrLf);
            if (encoding != null)
            {
                if (!encoding.Equals(Encoding.UTF8.BodyName))
                {
                    headerBuffer.AppendFormat("{0}:{1}; charset={2}{3}", ContentTypeHeader,
                        JsonMimeType, encoding, CrLf);
                }
            }
            headerBuffer.Append(CrLf);
            return headerBuffer.ToString();
        }

        /// <summary>
        /// Write the Json message corresponding to the given message, in the given stream using
        /// an encoding.
        /// </summary>
        /// <param name="stream">The Stream in whi</param>
        /// <param name="message"></param>
        /// <param name="encoding"></param>
        public static void WriteJsonMessage(Stream stream, string message, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            int contentLength = encoding.GetByteCount(message);
            string jsonHeader = JsonHeader(contentLength, encoding.BodyName);
            byte[] bytes = Encoding.ASCII.GetBytes(jsonHeader);
            stream.Write(bytes, 0, bytes.Length);
            bytes = encoding.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// Write in the given stream a dummy Json message.
        /// </summary>
        /// <param name="stream"></param>
        public static void WriteJsonDummyMessage(Stream stream)
        {
            WriteJsonMessage(stream, "");
        }
    }
}
