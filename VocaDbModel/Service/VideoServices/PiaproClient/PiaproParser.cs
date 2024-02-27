using AngleSharp.Text;

namespace VocaDb.Model.Service.VideoServices.PiaproClient;

using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;

/// <summary>
/// Parses Piapro HTML document.
/// </summary>
public class PiaproParser
{
	private static int? ParseLength(string lengthStr)
	{
		if (string.IsNullOrEmpty(lengthStr))
			return null;

		var parts = lengthStr.Split(':');

		if (parts.Length != 2)
			return null;

		if (!int.TryParse(parts[0], out var min) || !int.TryParse(parts[1], out var sec))
			return null;

		var totalSec = min * 60 + sec;

		return totalSec;
	}

	private (DateTime, string)? GetDate(HtmlNode? dataElem)
	{
		if (dataElem == null)
			return null;

		var match = Regex.Match(dataElem.InnerHtml,
			@"投稿日.+(\d\d\d\d/\d\d/\d\d \d\d:\d\d:\d\d)"); // "2010/08/21 19:09:15"

		if (!match.Success)
			return null;

		DateTime result;

		var rawDate = match.Groups[1].Value;
		var timeStamp = new string(rawDate.Where(c => c.IsDigit()).ToArray());

		if (DateTime.TryParse(rawDate, out result))
			return (result, timeStamp);
		else
			return null;
	}

	private int? GetLength(HtmlDocument doc)
	{
		var lengthEle = doc.DocumentNode.SelectSingleNode("//p[@class = 'contents_info_item' and contains(text(), '長さ')]");
		var lengthMatch = Regex.Match(lengthEle.InnerText, @"長さ.+(\d\d:\d\d)");

		if (!lengthMatch.Success)
			return null;

		return ParseLength(lengthMatch.Groups[1].Value);
	}

	private string GetContentId(HtmlDocument doc)
	{
		// Find both piapro.jp and www.piapro.jp
		// Note: HtmlAgilityPack does not support regex (XPath 2.0) :(
		var relatedMovieSpan = doc.DocumentNode.SelectSingleNode(
			"//a[starts-with(@href, \"/content/relate_movie/\")]" +
			"|//a[starts-with(@href, \"/content_list_recommend/\")]"
		);

		var relatedMovieMatch = relatedMovieSpan != null
			? Regex.Match(relatedMovieSpan.Attributes["href"].Value,
				@"/content(?:/relate_movie|_list_recommend)/\?id=([\d\w]+)")
			: null;
		var contentId = relatedMovieMatch != null && relatedMovieMatch.Success
			? relatedMovieMatch.Groups[1].Value
			: null;

		if (!string.IsNullOrEmpty(contentId))
			return contentId;

		// No anchor element, attempt to find contentId from script element.
		var scriptElem = doc.DocumentNode.SelectSingleNode("//script[@type = 'application/javascript']");
		var contentIdMatch = scriptElem != null
			? Regex.Match(scriptElem.InnerText, @"contentId\s*:\s*['\""]([a-z0-9]+)['\""]")
			: null;
		return contentIdMatch != null && contentIdMatch.Success ? contentIdMatch.Groups[1].Value : null;
	}

	private string GetUploadTimestamp(HtmlDocument doc)
	{
		var scriptElem = doc.DocumentNode.SelectSingleNode("//script[@type = 'application/javascript']");
		var uploadTimestampMatch = scriptElem != null
			? Regex.Match(scriptElem.InnerText, "createDate\\s*:\\s*['\"]([0-9]{14})['\"]")
			: null;
		return uploadTimestampMatch != null && uploadTimestampMatch.Success
			? uploadTimestampMatch.Groups[1].Value
			: null;
	}

	private string GetArtworkUrl(HtmlDocument doc)
	{
		var artworkElem = doc.DocumentNode.SelectSingleNode("/html/head/meta[@name = 'twitter:image']");
		var artworkUrl = artworkElem?.Attributes["content"]?.Value ?? string.Empty;
		return artworkUrl.StartsWith("https://res.piapro.jp/images/card_chara/") ? string.Empty : artworkUrl;
	}

	/// <summary>
	/// Parses a Piapro HTML document.
	/// </summary>
	/// <param name="doc">HTML document. Cannot be null.</param>
	/// <param name="url">URL of the post. Cannot be null or empty.</param>
	/// <returns>Query result. Cannot be null.</returns>
	/// <remarks>
	/// At least ID and title will be parsed.
	/// Author and length are optional.
	/// </remarks>
	/// <exception cref="PiaproException">If the query failed.</exception>
	public PostQueryResult ParseDocument(HtmlDocument doc, string url)
	{
		if (doc == null)
			throw new ArgumentNullException(nameof(doc));

		if (string.IsNullOrEmpty(url))
			throw new ArgumentException("URL cannot be null or empty", nameof(url));

		var dataElem = doc.DocumentNode.SelectSingleNode("//p[@class = 'contents_info_item']/a");
		var dateElem =
			doc.DocumentNode.SelectSingleNode("//p[@class = 'contents_info_item' and contains(text(), '投稿日')]");
		var postType = PostType.Other;
		int? length = null;

		if (dataElem != null && dataElem.Attributes["href"].Value.Contains("/music/"))
		{
			postType = PostType.Audio;
			length = GetLength(doc);
		}
		else if (dataElem != null && dataElem.Attributes["href"].Value.Contains("/illust/"))
		{
			postType = PostType.Illustration;
		}

		var timestamp = GetDate(dateElem);
		
		if (timestamp == null)
		{
			throw new PiaproException("Could not find timestamp");
		}
		
		var contentId = GetContentId(doc);

		if (contentId == null)
		{
			throw new PiaproException("Could not find id element on page.");
		}

		var titleElem = doc.DocumentNode.SelectSingleNode("//h1[@class = 'contents_title']");

		if (titleElem == null)
		{
			throw new PiaproException("Could not find title element on page.");
		}

		var title = HtmlEntity.DeEntitize(titleElem.InnerText).Trim();

		var authorID = doc.DocumentNode.SelectSingleNode("//div[@class = 'contents_creator']/a").Attributes["href"]
			?.Value.Substring(1) ?? string.Empty;
		var authorName = doc.DocumentNode.SelectSingleNode("//p[@class = 'contents_creator_txt']").InnerText;

		var artworkUrl = GetArtworkUrl(doc);

		return new PostQueryResult
		{
			Author = authorName,
			AuthorId = authorID,
			Id = contentId,
			LengthSeconds = length,
			PostType = postType,
			Title = title,
			Url = url,
			Date = timestamp.Value.Item1,
			ArtworkUrl = artworkUrl,
			UploadTimestamp = timestamp.Value.Item2
		};
	}
}