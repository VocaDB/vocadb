#nullable disable

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Helpers
{
	public static class StringHelper
	{
		/// <summary>
		/// Removes unsupported control characters from XML.
		/// See http://en.wikipedia.org/w/index.php?title=C0_and_C1_control_codes for more info.
		/// </summary>
		public static string CleanInvalidXmlChars(string text)
		{
			var re = @"(&#x1;)|(&#x2;)|(&#x8;)|(&#x10)";
			return Regex.Replace(text, re, "");
		}

		/// <summary>
		/// Removes invisible control characters from the string.
		/// This includes characters from <see cref="char.IsControl(char)"/>, except
		/// common whitespace characters like newlines and tabs.
		/// </summary>
		/// <param name="text">String to be processed. Can be null or empty.</param>
		/// <returns>String without control characters.</returns>
		public static string RemoveControlChars(string text)
		{
			if (string.IsNullOrEmpty(text))
				return text;

			var whitelist = new HashSet<char>(new[] { '\n', '\t' });
			return new string(text.Where(c => whitelist.Contains(c) || !char.IsControl(c)).ToArray());
		}

		/// <summary>
		/// Trims string if it's not completely whitespace.
		/// </summary>
		/// <remarks>
		/// There are songs and albums where the name is completely whitespace, so this must be supported.
		/// However, whitespace in the beginning or end is assumed to be an error.
		/// </remarks>
		public static string TrimIfNotWhitespace(string text)
		{
			return string.IsNullOrWhiteSpace(text) ? text : text.Trim();
		}
	}
}
