namespace VocaDb.Model.Service.VideoServices.PiaproClient;

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

/// <summary>
/// Client for accessing Piapro.
/// </summary>
public class PiaproClient
{
	/*
	 * Note: oddly enough, HttpClient is meant to be shared:
	 * https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
	 */

	public PiaproClient()
	{
	}

	public PiaproClient(HttpClient httpClient)
	{
		this.httpClient = httpClient;
	}

	/// <summary>
	/// Request timeout. Default value is 1 second.
	/// </summary>
	public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMilliseconds(1000);

	private readonly HttpClient httpClient;

	private HttpClient HttpClient => httpClient ?? new HttpClient();

	private static Encoding GetEncoding(string encodingStr)
	{
		// New piapro uses UTF-8 by default.
		if (string.IsNullOrEmpty(encodingStr))
			return Encoding.UTF8;

		try
		{
			return Encoding.GetEncoding(encodingStr);
		}
		catch (ArgumentException)
		{
			return Encoding.UTF8;
		}
	}

	private static bool IsFullLink(string str)
	{
		return (str.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
		        || str.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase)
		        || str.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase));
	}

	/// <summary>
	/// Makes a proper URL from a possible URL without a http:// prefix.
	/// </summary>
	/// <param name="partialLink">Partial URL. Can be null.</param>
	/// <returns>Full URL including http://. Can be null if source was null.</returns>
	private static string MakeLink(string partialLink)
	{
		if (string.IsNullOrEmpty(partialLink))
			return partialLink;

		if (IsFullLink(partialLink))
			return partialLink;

		return string.Format("https://{0}", partialLink);
	}

	private PostQueryResult ParseByHtmlStream(Stream htmlStream, Encoding encoding, string url)
	{
		var doc = new HtmlDocument();
		doc.Load(htmlStream, encoding);
		return new PiaproParser().ParseDocument(doc, url);
	}

	/// <summary>
	/// Parses a Piapro post specified by an URL.
	/// </summary>
	/// <param name="url">URL to Piapro post. Cannot be null or empty..</param>
	/// <returns>Result of the query. Cannot be null.</returns>
	/// <remarks>
	/// At least ID and title will be parsed.
	/// Author and length are optional.
	/// </remarks>
	/// <exception cref="PiaproException">If the query failed.</exception>
	public async Task<PostQueryResult> ParseByUrlAsync(string url)
	{
		if (string.IsNullOrEmpty(url))
			throw new ArgumentException("URL cannot be null or empty", nameof(url));

		Uri uri;
		try
		{
			uri = new Uri(MakeLink(url));
		}
		catch (UriFormatException x)
		{
			throw new PiaproException("Invalid Piapro URL", x);
		}

		// From https://stackoverflow.com/a/12023307
		var request = new HttpRequestMessage(HttpMethod.Get, uri);
		request.Headers.UserAgent.Add(new ProductInfoHeaderValue("PiaproClient", "2.0"));

		HttpResponseMessage response;

		// From https://stackoverflow.com/a/46877380
		using (var cts = new CancellationTokenSource())
		{
			cts.CancelAfter(RequestTimeout);

			try
			{
				response = await HttpClient.SendAsync(request, cts.Token);
				response.EnsureSuccessStatusCode();
			}
			catch (HttpRequestException x)
			{
				throw new PiaproException("Unable to get a response from the server, try again later", x);
			}
		}

		response.Content.Headers.TryGetValues("Content-Encoding", out var encodingHeaders);
		var enc = GetEncoding(encodingHeaders?.FirstOrDefault());

		try
		{
			using (var stream = await response.Content.ReadAsStreamAsync())
			{
				return ParseByHtmlStream(stream, enc, url);
			}
		}
		finally
		{
			response.Dispose();
		}
	}

	/// <summary>
	/// Parses a Piapro post specified by an URL.
	/// </summary>
	/// <param name="url">URL to Piapro post. Cannot be null or empty..</param>
	/// <returns>Result of the query. Cannot be null.</returns>
	/// <remarks>
	/// At least ID and title will be parsed.
	/// Author and length are optional.
	/// </remarks>
	/// <exception cref="PiaproException">If the query failed.</exception>
	public PostQueryResult ParseByUrl(string url)
	{
		if (string.IsNullOrEmpty(url))
			throw new ArgumentException("URL cannot be null or empty", "url");

		WebRequest request;
		try
		{
			request = WebRequest.Create(MakeLink(url));
		}
		catch (UriFormatException x)
		{
			throw new PiaproException("Invalid Piapro URL", x);
		}

		request.Timeout = 10000;
		WebResponse response;

		try
		{
			response = request.GetResponse();
		}
		catch (WebException x)
		{
			throw new PiaproException("Unable to get a response from the server, try again later", x);
		}

		var enc = GetEncoding(response.Headers[HttpResponseHeader.ContentEncoding]);

		try
		{
			using (var stream = response.GetResponseStream())
			{
				return ParseByHtmlStream(stream, enc, url);
			}
		}
		finally
		{
			response.Close();
		}
	}
}