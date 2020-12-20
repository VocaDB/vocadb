// Code from: https://github.com/saad749/BeginCollectionItemCore/blob/6614c505fde60fa72430c9da18999b8ceb7918f7/BeginCollectionItemCore/HtmlPrefixScopeExtensions.cs

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VocaDb.Web.Helpers
{
	public static class HtmlPrefixScopeExtensions
	{
		private const string IdsToReuseKey = "__htmlPrefixScopeExtensions_IdsToReuse_";

		public static IDisposable BeginCollectionItem(this IHtmlHelper html, string collectionName)
		{
			return BeginCollectionItem(html, collectionName, html.ViewContext.Writer);
		}

		public static IDisposable BeginCollectionItem(this IHtmlHelper html, string collectionName, TextWriter writer)
		{
			/*
             * added Nested collection support for newly added collection items
             * as per this http://stackoverflow.com/questions/33916004/nested-list-of-lists-with-begincollectionitem
             * and this http://www.joe-stevens.com/2011/06/06/editing-and-binding-nested-lists-with-asp-net-mvc-2/
            */
			if (html.ViewData["ContainerPrefix"] != null)
				collectionName = string.Concat(html.ViewData["ContainerPrefix"], ".", collectionName);

			var idsToReuse = GetIdsToReuse(html.ViewContext.HttpContext, collectionName);
			var itemIndex = idsToReuse.Count > 0 ? idsToReuse.Dequeue() : Guid.NewGuid().ToString();

			string htmlFieldPrefix = $"{collectionName}[{itemIndex}]";
			html.ViewData["ContainerPrefix"] = htmlFieldPrefix;

			/* 
             * html.Name(); has been removed
             * because of incorrect naming of collection items
             * e.g.
             * let collectionName = "Collection"
             * the first item's name was Collection[0].Collection[<GUID>]
             * instead of Collection[<GUID>]
             */
			string indexInputName = $"{collectionName}.index";

			// autocomplete="off" is needed to work around a very annoying Chrome behaviour
			// whereby it reuses old values after the user clicks "Back", which causes the
			// xyz.index and xyz[...] values to get out of sync.
			writer.WriteLine($@"<input type=""hidden"" name=""{indexInputName}"" autocomplete=""off"" value=""{html.Encode(itemIndex)}"" />");


			return BeginHtmlFieldPrefixScope(html, htmlFieldPrefix);
		}

		public static IDisposable BeginHtmlFieldPrefixScope(this IHtmlHelper html, string htmlFieldPrefix)
		{
			return new HtmlFieldPrefixScope(html.ViewData.TemplateInfo, htmlFieldPrefix);
		}

		private static Queue<string> GetIdsToReuse(HttpContext httpContext, string collectionName)
		{
			// We need to use the same sequence of IDs following a server-side validation failure,
			// otherwise the framework won't render the validation error messages next to each item.
			var key = IdsToReuseKey + collectionName;
			var queue = (Queue<string>)httpContext.Items[key];
			if (queue == null)
			{
				httpContext.Items[key] = queue = new Queue<string>();

				if (httpContext.Request.Method == "POST" && httpContext.Request.HasFormContentType)
				{
					StringValues previouslyUsedIds = httpContext.Request.Form[collectionName + ".index"];
					if (!string.IsNullOrEmpty(previouslyUsedIds))
						foreach (var previouslyUsedId in previouslyUsedIds)
							queue.Enqueue(previouslyUsedId);
				}
			}
			return queue;
		}

		internal class HtmlFieldPrefixScope : IDisposable
		{
			internal readonly TemplateInfo TemplateInfo;
			internal readonly string PreviousHtmlFieldPrefix;

			public HtmlFieldPrefixScope(TemplateInfo templateInfo, string htmlFieldPrefix)
			{
				TemplateInfo = templateInfo;

				PreviousHtmlFieldPrefix = TemplateInfo.HtmlFieldPrefix;
				TemplateInfo.HtmlFieldPrefix = htmlFieldPrefix;
			}

			public void Dispose()
			{
				TemplateInfo.HtmlFieldPrefix = PreviousHtmlFieldPrefix;
			}
		}
	}
}
