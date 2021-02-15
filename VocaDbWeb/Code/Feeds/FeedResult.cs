#nullable disable

using System;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace VocaDb.Web.Code.Feeds
{
	public class FeedResult : ActionResult
	{
		private readonly SyndicationFeedFormatter _feed;

		public Encoding ContentEncoding { get; set; }
		public string ContentType { get; set; }

		public SyndicationFeedFormatter Feed => _feed;

		public FeedResult(SyndicationFeedFormatter feed)
		{
			_feed = feed;
		}

		public override async Task ExecuteResultAsync(ActionContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			if (_feed != null)
			{
				using var stringWriter = new StringWriter();
				using var xmlWriter = new XmlTextWriter(stringWriter);
				_feed.WriteTo(xmlWriter);
				await new ContentResult
				{
					Content = stringWriter.ToString(),
					ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/rss+xml",
					StatusCode = (int)HttpStatusCode.OK,
				}.ExecuteResultAsync(context);
			}
		}
	}
}