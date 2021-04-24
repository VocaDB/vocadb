using System;
using System.Text;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Service.BBCode
{
	/// <summary>
	/// Converts BBCode into HTML
	/// </summary>
	public class BBCodeConverter
	{
		/// <summary>
		/// Replaces instances of a regex in a string.
		/// The match may occur multiple times, and all instances will be replaced.
		/// </summary>
		/// <param name="bbCode">Source string to be processed. Cannot be null.</param>
		/// <param name="regex">Regex to be matched. Cannot be null.</param>
		/// <param name="replacementFunc">Replacement operation to be performed for the matches. Cannot be null.</param>
		public static void RegexReplace(StringBuilder bbCode, Regex regex, Func<Match, string> replacementFunc)
		{
			var matches = regex.Matches(bbCode.ToString());

			var indexOffset = 0;

			foreach (Match match in matches)
			{
				var result = replacementFunc(match);

				if (result != match.Value)
				{
					bbCode.Replace(match.Value, result, match.Index + indexOffset, match.Length);
					indexOffset += (result.Length - match.Value.Length);
				}
			}
		}
	}
}
