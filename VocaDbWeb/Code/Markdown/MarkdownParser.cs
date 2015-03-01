using System.Runtime.Caching;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Helpers;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code.Markdown {

	/// <summary>
	/// Caching Markdown parser
	/// </summary>
	public class MarkdownParser {

		private readonly ObjectCache cache;

		public MarkdownParser(ObjectCache cache) {
			this.cache = cache;
		}

		public string GetHtml(string markdownText) {
			
			if (string.IsNullOrEmpty(markdownText))
				return markdownText;

			var key = string.Format("MarkdownParser.Html_{0}", markdownText);
			return cache.GetOrInsert(key, CachePolicy.Never(), () => MarkdownHelper.TranformMarkdown(markdownText));

		}

		public string GetPlainText(string markdownText) {
			
			if (string.IsNullOrEmpty(markdownText))
				return markdownText;

			return HtmlHelperFunctions.StripHtml(GetHtml(markdownText));

		}

	}

}