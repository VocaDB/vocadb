using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Service.VideoServices
{
	/// <summary>
	/// Captures values from (partial) URL using regex and then formats a full
	/// URL using those captured values.
	/// </summary>
	public class RegexLinkMatcher
	{
		private readonly string template;
		private readonly Regex regex;

		/// <summary>
		/// Initializes matcher.
		/// </summary>
		/// <param name="template">URL template with ID placeholder, for example "https://twitter.com/{0}".</param>
		/// <param name="regexStr">Regex with groups for the ID, for example "^http(?:s)?://twitter\.com/(\w+)". Case will be ignored.</param>
		/// <remarks>The number of captured groups in <paramref name="regexStr"/> should match placeholders in <paramref name="template"/>.</remarks>
		public RegexLinkMatcher(string template, string regexStr)
		{
			regex = new Regex(regexStr, RegexOptions.IgnoreCase);
			this.template = template;
		}

		public string GetId(string url)
		{
			var match = regex.Match(url);

			if (match.Groups.Count < 2)
				throw new InvalidOperationException("Regex needs a group for the ID");

			var group = match.Groups[1];
			return group.Value;
		}

		public bool IsMatch(string url) => regex.IsMatch(url);

		public string MakeLinkFromUrl(string url) => MakeLinkFromId(GetId(url));

		public string MakeLinkFromId(string id) => string.Format(template, id);

		public (bool success, string formattedUrl) GetLinkFromUrl(string url)
		{
			var success = TryGetLinkFromUrl(url, out var formattedUrl);
			return (success, formattedUrl);
		}

		public bool TryGetLinkFromUrl(string url, out string formattedUrl)
		{
			var match = regex.Match(url);

			if (match.Success)
			{
				var values = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();
				formattedUrl = string.Format(template, values);
				return true;
			}
			else
			{
				formattedUrl = null;
				return false;
			}
		}
	}
}
