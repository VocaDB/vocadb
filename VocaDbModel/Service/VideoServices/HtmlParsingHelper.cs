using System;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace VocaDb.Model.Service.VideoServices
{

	public static class HtmlParsingHelper
	{

		public static Encoding GetEncoding(string encodingStr, Encoding defaultEncoding)
		{

			if (string.IsNullOrEmpty(encodingStr))
				return defaultEncoding;

			try
			{
				return Encoding.GetEncoding(encodingStr);
			}
			catch (ArgumentException)
			{
				return defaultEncoding;
			}

		}

		public static T ParseHtmlPage<T>(string url, Encoding defaultEncoding, Func<HtmlDocument, string, T> func) where T : class
		{

			var request = WebRequest.Create(url);
			WebResponse response;

			try
			{
				response = request.GetResponse();
			}
			catch (WebException)
			{
				return null;
			}

			var enc = GetEncoding(response.Headers[HttpResponseHeader.ContentEncoding], defaultEncoding);

			try
			{
				using (var stream = response.GetResponseStream())
				{
					var doc = new HtmlDocument();
					doc.Load(stream, enc);
					return func(doc, url);
				}
			}
			finally
			{
				response.Close();
			}

		}

	}

}
