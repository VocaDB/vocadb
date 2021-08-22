using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VocaDb.Model.Helpers
{
	public static class JsonRequest
	{
		public static Task<T?> ReadObjectAsync<T>(string url) => ReadObjectAsync<T>(url, TimeSpan.FromSeconds(100));

		/// <summary>
		/// Reads JSON object from URL.
		/// </summary>
		/// <typeparam name="T">Type of object to be read.</typeparam>
		/// <param name="url">URL. Cannot be null or empty.</param>
		/// <param name="timeout">Request timeout.</param>
		/// <param name="userAgent">User agent string.</param>
		/// <returns>The read object.</returns>
		/// <exception cref="WebException">If a web request error occurred.</exception>
		/// <exception cref="JsonSerializationException">If the response wasn't valid JSON.</exception>
		/// <exception cref="HttpRequestException">If the request failed.</exception>
		public static async Task<T?> ReadObjectAsync<T>(string url, TimeSpan timeout, string userAgent = "",
			Action<HttpRequestHeaders>? headers = null)
		{
			return await HtmlRequestHelper.GetStreamAsync(url, stream =>
			{
				using (var streamReader = new StreamReader(stream))
				using (var jsonReader = new JsonTextReader(streamReader))
				{
					var serializer = new JsonSerializer();
					return serializer.Deserialize<T>(jsonReader);
				}
			}, timeout: timeout, userAgent: userAgent, headers: headers);
		}
	}
}
