#nullable disable

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VocaDb.Model.Helpers
{
	public static class HtmlRequestHelper
	{
		public static HtmlDocument Download(string url, string acceptLanguage = null)
		{
			var request = WebRequest.Create(url);

			if (!string.IsNullOrEmpty(acceptLanguage))
				request.Headers.Add(HttpRequestHeader.AcceptLanguage, acceptLanguage);

			WebResponse response = request.GetResponse();

			try
			{
				var enc = response.Headers[HttpResponseHeader.ContentEncoding];

				using (var stream = response.GetResponseStream())
				{
					var encoding = (!string.IsNullOrEmpty(enc) ? Encoding.GetEncoding(enc) : Encoding.UTF8);

					var doc = new HtmlDocument();
					doc.Load(stream, encoding);
					return doc;
				}
			}
			finally
			{
				response.Close();
			}
		}

		public static Task<T> GetStreamAsync<T>(string url, Func<Stream, T> func) => GetStreamAsync(url, func, TimeSpan.FromSeconds(30));

		/// <exception cref="HttpRequestException">If the request failed</exception>
		public static async Task<T> GetStreamAsync<T>(string url, Func<Stream, T> func, TimeSpan timeout, string userAgent = "",
			Action<HttpRequestHeaders> headers = null)
		{
			var uri = new Uri(url);

			using (var client = new HttpClient())
			{
				if (string.IsNullOrEmpty(userAgent))
				{
					client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("VocaDB", "1.0"));
				}
				else
				{
					client.DefaultRequestHeaders.Add("User-Agent", userAgent);
				}
				client.Timeout = timeout;

				headers?.Invoke(client.DefaultRequestHeaders);

				using (var response = await client.GetAsync(uri))
				{
					response.EnsureSuccessStatusCode();
					var stream = await response.Content.ReadAsStreamAsync();
					return func(stream);
				}
			}
		}
	}
}
