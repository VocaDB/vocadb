using System.Text.RegularExpressions;

namespace VocaDb.Model.Helpers
{

	public static class HtmlHelperFunctions
	{

		/// <summary>
		/// Strips HTML tags from a string.
		/// Text inside tags is kept.
		/// </summary>
		/// <param name="html">Input HTML. Can be null or empty.</param>
		/// <returns>String with all HTML tags stripped. Can be null or empty.</returns>
		public static string StripHtml(string html)
		{

			if (string.IsNullOrEmpty(html))
				return html;

			return Regex.Replace(html, "<.*?>", string.Empty);

		}
	}

}
