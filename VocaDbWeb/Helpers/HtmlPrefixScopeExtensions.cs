#nullable disable

using System;
using System.Web.Mvc;
using System.Web;
using System.Collections.Generic;

namespace VocaDb.Web.Helpers
{
	public static class HtmlPrefixScopeExtensions
	{
		private const string IdsToReuseKey = "__htmlPrefixScopeExtensions_IdsToReuse_";

		public static IDisposable BeginCollectionItem(this HtmlHelper html, string collectionName)
		{
			var idsToReuse = GetIdsToReuse(html.ViewContext.HttpContext, collectionName);
			string itemIndex = idsToReuse.Count > 0 ? idsToReuse.Dequeue() : Guid.NewGuid().ToString();

			// autocomplete="off" is needed to work around a very annoying Chrome behaviour whereby it reuses old values after the user clicks "Back", which causes the xyz.index and xyz[...] values to get out of sync.
			html.ViewContext.Writer.WriteLine($"<input type=\"hidden\" name=\"{collectionName}.index\" autocomplete=\"off\" value=\"{html.Encode(itemIndex)}\" />");

			return BeginHtmlFieldPrefixScope(html, $"{collectionName}[{itemIndex}]");
		}

		public static IDisposable BeginHtmlFieldPrefixScope(this HtmlHelper html, string htmlFieldPrefix)
		{
			return new HtmlFieldPrefixScope(html.ViewData.TemplateInfo, htmlFieldPrefix);
		}

		private static Queue<string> GetIdsToReuse(HttpContextBase httpContext, string collectionName)
		{
			// We need to use the same sequence of IDs following a server-side validation failure,  
			// otherwise the framework won't render the validation error messages next to each item.
			string key = IdsToReuseKey + collectionName;
			var queue = (Queue<string>)httpContext.Items[key];
			if (queue == null)
			{
				httpContext.Items[key] = queue = new Queue<string>();
				var previouslyUsedIds = httpContext.Request[collectionName + ".index"];
				if (!string.IsNullOrEmpty(previouslyUsedIds))
					foreach (string previouslyUsedId in previouslyUsedIds.Split(','))
						queue.Enqueue(previouslyUsedId);
			}
			return queue;
		}

		private class HtmlFieldPrefixScope : IDisposable
		{
			private readonly TemplateInfo _templateInfo;
			private readonly string _previousHtmlFieldPrefix;

			public HtmlFieldPrefixScope(TemplateInfo templateInfo, string htmlFieldPrefix)
			{
				_templateInfo = templateInfo;

				_previousHtmlFieldPrefix = templateInfo.HtmlFieldPrefix;
				templateInfo.HtmlFieldPrefix = htmlFieldPrefix;
			}

			public void Dispose()
			{
				_templateInfo.HtmlFieldPrefix = _previousHtmlFieldPrefix;
			}
		}
	}
}