// Code from: https://github.com/mono/aspnetwebstack/blob/6248bfd24c31356e75a31c1b1030d4d96f669a6a/src/Microsoft.Web.Helpers/Gravatar.cs

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.Web.Helpers;

namespace VocaDb.Web.Helpers
{
	public static class Gravatar
	{
		// review - extract conversion of anonymous object to html attributes string into separate helper
		public static HtmlString GetHtml(string email, int imageSize = 80, string? defaultImage = null, GravatarRating rating = GravatarRating.Default, string? imageExtension = null, object? attributes = null)
		{
			var altSpecified = false;
			var url = GetUrl(email, imageSize, defaultImage, rating, imageExtension);
			var html = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "<img src=\"{0}\" ", url));
			if (attributes != null)
			{
				foreach (var p in attributes.GetType().GetProperties().OrderBy(p => p.Name))
				{
					if (!p.Name.Equals("src", StringComparison.OrdinalIgnoreCase))
					{
						var value = p.GetValue(attributes, null);
						if (value != null)
						{
							var encodedValue = HttpUtility.HtmlAttributeEncode(value.ToString());
							html.Append(string.Format(CultureInfo.InvariantCulture, "{0}=\"{1}\" ", p.Name, encodedValue));
						}
						if (p.Name.Equals("alt", StringComparison.OrdinalIgnoreCase))
							altSpecified = true;
					}
				}
			}
			if (!altSpecified)
				html.Append("alt=\"gravatar\" ");
			html.Append("/>");
			return new HtmlString(html.ToString());
		}

		public static string GetUrl(string email, int imageSize = 80, string? defaultImage = null, GravatarRating rating = GravatarRating.Default, string? imageExtension = null)
			=> Microsoft.Web.Helpers.Gravatar.GetUrl(email, imageSize, defaultImage, rating, imageExtension).Replace("http://", "https://");
	}
}
