using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PastebinApiWrapper
{
    static class WebAgent
    {
        public static async Task<HttpResponseMessage> Execute(Uri requestedUri, FormUrlEncodedContent content)
        {
            using (var client = new HttpClient())
            {
                return await client.PostAsync(requestedUri, content).ConfigureAwait(false);
            }
        }
    }
}
