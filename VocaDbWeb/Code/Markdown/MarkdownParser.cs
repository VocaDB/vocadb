#nullable disable

using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web;
using MarkdownSharp;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Code.Markdown
{
	/// <summary>
	/// Caching Markdown parser
	/// </summary>
	public class MarkdownParser
	{
		// Match "&gt;" at the beginning of each line, to fix markdown blockquotes
		private static readonly Regex s_quoteRegex = new("^&gt;", RegexOptions.Multiline);

		private static string TranformMarkdown(string text)
		{
			if (string.IsNullOrEmpty(text))
				return text;

			var encoded = HttpUtility.HtmlEncode(text);
			encoded = s_quoteRegex.Replace(encoded, ">");

			// StrictBoldItalic is needed because otherwise links with underscores won't work (links are more common on VDB).
			// These settings roughtly correspond to GitHub-flavored Markdown (https://help.github.com/articles/github-flavored-markdown)
			return new MarkdownSharp.Markdown(new MarkdownOptions { AutoHyperlink = true, AutoNewlines = true, StrictBoldItalic = true, EmptyElementSuffix = " />" })
				.Transform(encoded);
		}

		private readonly ObjectCache _cache;

		public MarkdownParser(ObjectCache cache)
		{
			_cache = cache;
		}

		/// <summary>
		/// Transforms a block of text with Markdown. The input will be sanitized. The result will be cached.
		/// </summary>
		/// <param name="markdownText">Markdown text to be transformed. HTML will be encoded.</param>
		/// <returns>
		/// Markdown-transformed text. This will include HTML. 
		/// The block is usually surrounded by "p" tags.
		/// </returns>
		public string GetHtml(string markdownText)
		{
			if (string.IsNullOrEmpty(markdownText))
				return markdownText;

			var key = $"MarkdownParser.Html_{markdownText}";
			return _cache.GetOrInsert(key, CachePolicy.Never(), () => TranformMarkdown(markdownText));
		}

		/// <summary>
		/// Gets plain text from markdown-formatted text (strips markdown).
		/// </summary>
		/// <param name="markdownText">Markdown-formatted text, for example "**Miku**".</param>
		/// <returns>Text without markdown formatting, for example "Miku".</returns>
		public string GetPlainText(string markdownText)
		{
			if (string.IsNullOrEmpty(markdownText))
				return markdownText;

			return HtmlHelperFunctions.StripHtml(ReplaceHtmlEntities(GetHtml(markdownText)));
		}

		private string ReplaceHtmlEntities(string text)
		{
			return text.Replace("&#39;", "'");
		}
	}
}