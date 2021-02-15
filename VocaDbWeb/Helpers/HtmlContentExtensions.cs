using System.IO;
using Microsoft.AspNetCore.Html;

namespace VocaDb.Web.Helpers
{
	// Code from: https://stackoverflow.com/questions/50877875/asp-net-core-tohtmlstring/50877913#50877913
	public static class HtmlContentExtensions
	{
		public static string? ToHtmlString(this IHtmlContent htmlContent)
		{
			if (htmlContent is HtmlString htmlString)
				return htmlString.Value;

			using var writer = new StringWriter();
			htmlContent.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
			return writer.ToString();
		}
	}
}
