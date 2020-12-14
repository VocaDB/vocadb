#nullable disable

using System.Linq;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Service.Search
{
	public static class SearchParser
	{
		// prop-name:value
		// prop-name is optional. value can be quoted. If quoted, it may contain whitespace
		private static readonly Regex regex = new Regex(@"(?:([\w-]+):)?(?:([^\s""]+)|""([^""]+)"")");

		public static SearchWordCollection ParseQuery(string query)
		{
			var matches = regex.Matches(query);
			var words = matches.Cast<Match>().Select(m => new SearchWord(m.Groups[1].Value, m.Groups[3].Success ? m.Groups[3].Value : m.Groups[2].Value));

			return new SearchWordCollection(words);
		}
	}
}
