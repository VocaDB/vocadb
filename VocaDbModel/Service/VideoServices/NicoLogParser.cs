using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NLog;
using VocaDb.Model.Service.Security;

namespace VocaDb.Model.Service.VideoServices;

public class NicoLogParser : IVideoServiceParser
{
	public Task<VideoTitleParseResult> GetTitleAsync(string id) => NicoLogHelper.GetVideoTitleParseResultAsync(id);
}

public static class NicoLogHelper
{
	private static readonly Logger log = LogManager.GetCurrentClassLogger();

	private static async Task<HtmlDocument?> GetVideoHtmlPage(string videoId)
	{
		var url = $"https://nicolog.jp/watch/{videoId}";

		// NicoLog only support TLS 1.3
		var handler = new HttpClientHandler
		{
			SslProtocols = System.Security.Authentication.SslProtocols.Tls13
		};

		using var client = new HttpClient(handler);
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
		HttpResponseMessage response;

		try
		{
			response = await client.GetAsync(url);
			response.EnsureSuccessStatusCode();
		}
		catch (HttpRequestException x)
		{
			log.Warn(x, "Unable to get response for nicolog page");
			return null;
		}

		var doc = new HtmlDocument();
		try
		{
			var contentStream = await response.Content.ReadAsStreamAsync();
			doc.Load(contentStream, Encoding.UTF8);
		}
		catch (IOException x)
		{
			log.Warn(x, "Unable to load document for nicolog page");
			return null;
		}

		return doc;
	}

	public static Task<VideoTitleParseResult> GetVideoTitleParseResultAsync(string id)
	{
		var doc = GetVideoHtmlPage(id).Result;
		VideoTitleParseResult videoTitleParseResult = ParsePage(doc);

		return Task.FromResult(videoTitleParseResult);
	}

	public static VideoTitleParseResult ParsePage(HtmlDocument? doc)
	{
		var tagsNode = doc?.DocumentNode.SelectSingleNode("//ul[@class='list-inline']");
		if (tagsNode == null)
		{
			return VideoTitleParseResult.CreateError("NicoLog (error): no info about video");
		}

		var metaTable = doc?.DocumentNode.SelectSingleNode("//dl[@class='dl-horizontal']");
		if (metaTable == null)
		{
			return VideoTitleParseResult.CreateError("NicoLog (error): could not parse page");
		}
		var title = metaTable.SelectSingleNode("//dd[2]").InnerText;
		var replace = metaTable.SelectSingleNode("//dd[3]").InnerText.Replace("年", ".").Replace("月", ".").Replace("日 ", " ").Replace("時", ":").Replace("分", ":").Replace("秒", "");
		var uploadDate = DateTime.ParseExact(replace, "yyyy.M.d H:mm:ss", CultureInfo.InvariantCulture);
		var lengthSeconds = (int)TimeSpan.ParseExact(metaTable.SelectSingleNode("//dd[4]").InnerText, "h\\:mm\\:ss", CultureInfo.InvariantCulture).TotalSeconds;
		var author = Regex.Replace(metaTable.SelectSingleNode("//dd[5]").InnerText, @"(\s\(ID:\d+\))", "");
		var authorId = metaTable.SelectSingleNode("//dd[5]/a").GetAttributeValue("href", "").Split('/')[1];
		var thumbUrl = doc?.DocumentNode.SelectSingleNode("//img[@class='center-block img-thumbnail']").GetAttributeValue("src", "");

		var result = VideoTitleParseResult.CreateSuccess(title, author, authorId, thumbUrl,
			lengthSeconds, uploadDate: uploadDate);
		var tags = tagsNode.ChildNodes.Select(node => node.InnerText).ToArray();
		result.Tags = tags;

		return result;
	}
}