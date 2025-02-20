using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Movies.Tests.Util
{
    internal static class RequestExtensions
    {
        internal static ByteArrayContent CreateRequestBody(object data)
        {
            var myContent = JsonConvert.SerializeObject(data);

            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return byteContent;
        }
    }
}
