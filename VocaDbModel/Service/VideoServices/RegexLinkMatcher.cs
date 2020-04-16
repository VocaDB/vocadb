using System;
using System.Linq;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.VideoServices {

    /// <summary>
	/// Captures values from (partial) URL using regex and then formats a full
	/// URL using those captured values.
	/// </summary>
	public class RegexLinkMatcher {

		private readonly string template;
		private readonly Regex regex;

        /// <summary>
		/// Initializes matcher.
		/// </summary>
		/// <param name="template">URL template with ID placeholder, for example "https://twitter.com/{0}".</param>
		/// <param name="regexStr">Regex with groups for the ID, for example "^http(?:s)?://twitter\.com/(\w+)". Case will be ignored.</param>
		/// <remarks>The number of captured groups in <paramref name="regexStr"/> should match placeholders in <paramref name="template"/>.</remarks>
		public RegexLinkMatcher(string template, string regexStr) {

			regex = new Regex(regexStr, RegexOptions.IgnoreCase);
			this.template = template;

		}

		public string GetId(VocaDbUrl url) {

			var match = regex.Match(url.Url);

			if (match.Groups.Count < 2)
				throw new InvalidOperationException("Regex needs a group for the ID");

			var group = match.Groups[1];
			return group.Value;

		}

		public bool IsMatch(VocaDbUrl url) => regex.IsMatch(url.Url);

		public VocaDbUrl MakeLinkFromUrl(VocaDbUrl url) => MakeLinkFromId(GetId(url));

		public VocaDbUrl MakeLinkFromId(string id) => VocaDbUrl.External(string.Format(template, id));

		public (bool success, VocaDbUrl formattedUrl) GetLinkFromUrl(VocaDbUrl url) {
            var success = TryGetLinkFromUrl(url, out var formattedUrl);
            return (success, formattedUrl);
		}

		public bool TryGetLinkFromUrl(VocaDbUrl url, out VocaDbUrl formattedUrl) {

	        var match = regex.Match(url.Url);

            if (match.Success) {
	            var values = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();
	            formattedUrl = VocaDbUrl.External(string.Format(template, values));
	            return true;
			} else {
	            formattedUrl = VocaDbUrl.Empty;
                return false;
            }

        }

	}

}
