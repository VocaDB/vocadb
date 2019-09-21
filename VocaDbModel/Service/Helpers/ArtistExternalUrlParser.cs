using System.Linq;
using System.Text.RegularExpressions;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service.Helpers {

	/// <summary>
	/// Parses external URL for an artist from a possible URL fragment.
	/// Used for finding artists based on URLs to their profiles.
	/// Only certain URL patterns such as NicoNico mylist are matched.
	/// </summary>
	public class ArtistExternalUrlParser {

		private class Matcher {

			public Matcher(string template, string regex) {
				Template = template;
				Regex = new Regex(regex);
			}

			public string Template { get; }
			public Regex Regex { get; }

		}

		private static readonly Matcher[] linkMatchers = {
			new Matcher("https://www.nicovideo.jp/{0}/{1}", @"^(?:http(?:s)?://www.nicovideo.jp)?/?(user|mylist)/(\d+)"),
			new Matcher("https://twitter.com/{0}", @"^https://twitter\.com/(\w+)")
		};

		private static readonly string[] whitelistedUrls = {
			"http://piapro.jp/", "https://piapro.jp/", "http://www.piapro.jp/", "https://www.piapro.jp/"
		};

		/// <summary>
		/// Get full external URL from a possible external URL fragment.
		/// For example both /mylist/6667938 and http://www.nicovideo.jp/mylist/6667938 will be identified as http://www.nicovideo.jp/mylist/6667938
		/// </summary>
		/// <param name="possibleUrl">
		/// Possible external URL fragment or full URL. For example, /mylist/6667938 or http://www.nicovideo.jp/mylist/6667938
		/// Can be null or empty, but obviously those don't match anything.
		/// </param>
		/// <returns>
		/// Full external URL, if matched. For example, https://www.nicovideo.jp/mylist/6667938.
		/// Can be null if there was no match.
		/// </returns>
		public string GetExternalUrl(string possibleUrl) {
			
			if (string.IsNullOrEmpty(possibleUrl))
				return null;

			var lowercase = possibleUrl.ToLowerInvariant();
			if (whitelistedUrls.Any(u => lowercase.StartsWith(u))) {
				return possibleUrl;
			}

			var match = linkMatchers
				.Select(matcher => new {
					matcher.Template,
					Result = matcher.Regex.Match(possibleUrl)
				})
				.FirstOrDefault(m => m.Result.Success);

			if (match == null)
				return null;

			var values = match.Result.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();
			return string.Format(match.Template, values);

		}

	}

}
