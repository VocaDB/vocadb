using System.Text.RegularExpressions;

namespace VocaDb.Model.Service.Helpers {

	/// <summary>
	/// Parses external URL for an artist from a possible URL fragment.
	/// Used for finding artists based on URLs to their profiles.
	/// Only certain URL patterns such as NicoNico mylist are matched.
	/// </summary>
	public class ArtistExternalUrlParser {

		private readonly Regex nicoMatcher = 
			new Regex(@"^(?:http://www.nicovideo.jp)?/?(user|mylist)/(\d+)", RegexOptions.IgnoreCase);

		private const string nicoUrlTemplate = "http://www.nicovideo.jp/{0}/{1}";

		/// <summary>
		/// Get full external URL from a possible external URL fragment.
		/// For example both /mylist/6667938 and http://www.nicovideo.jp/mylist/6667938 will be identified as http://www.nicovideo.jp/mylist/6667938
		/// </summary>
		/// <param name="possibleUrl">
		/// Possible external URL fragment or full URL. For example, /mylist/6667938 or http://www.nicovideo.jp/mylist/6667938
		/// Can be null or empty, but obviously those don't match anything.
		/// </param>
		/// <returns>
		/// Full external URL, if matched. For example, http://www.nicovideo.jp/mylist/6667938.
		/// Can be null if there was no match.
		/// </returns>
		public string GetExternalUrl(string possibleUrl) {
			
			if (string.IsNullOrEmpty(possibleUrl))
				return null;

			var match = nicoMatcher.Match(possibleUrl);

			if (!match.Success)
				return null;

			var type = match.Groups[1].Value;
			var id = match.Groups[2].Value;

			return string.Format(nicoUrlTemplate, type, id);

		}

	}

}
