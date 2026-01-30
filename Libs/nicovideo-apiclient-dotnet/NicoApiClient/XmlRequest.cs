using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VocaDb.NicoApi {

    public static class XmlRequest {

        public static T GetXmlResponse<T>(Stream stream) {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }

        private static async Task<T> GetStreamAsync<T>(string url, HttpClient httpClient, Func<Stream, T> func, int timeoutSec = 10000, string userAgent = "") {

            var uri = new Uri(url);

            // From https://stackoverflow.com/a/12023307
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            if (string.IsNullOrEmpty(userAgent)) {
                request.Headers.UserAgent.Add(new ProductInfoHeaderValue("NicoApiClient", "1.0"));
            } else {
                request.Headers.Add("User-Agent", userAgent);
            }

            // From https://stackoverflow.com/a/46877380
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSec));

            using (var response = await httpClient.SendAsync(request, cts.Token)) {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                return func(stream);
            }

        }

        /// <summary>
        /// Performers a HTTP request that returns XML and deserializes that XML result into object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="url">URL to be requested.</param>
        /// <param name="httpClient">HTTP client. Cannot be null.</param>
        /// <returns>Deserialized object. Cannot be null.</returns>
        /// <exception cref="HttpRequestException">If the request failed.</exception>
        public static Task<T> GetXmlObjectAsync<T>(string url, HttpClient httpClient) => GetStreamAsync(url, httpClient, stream => GetXmlResponse<T>(stream));

    }

}
