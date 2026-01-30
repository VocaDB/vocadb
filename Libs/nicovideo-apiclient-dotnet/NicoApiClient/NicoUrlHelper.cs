using System.Linq;

namespace VocaDb.NicoApi {

	/// <summary>
	/// Helper methods for parsing NicoNicoDouga URLs.
	/// </summary>
	public class NicoUrlHelper {

		private static readonly RegexLinkMatcher[] matchers = new[] {
			new RegexLinkMatcher("www.nicovideo.jp/watch/{0}", @"nicovideo.jp/watch/([a-z]{2}\d{4,10})"),
			new RegexLinkMatcher("www.nicovideo.jp/watch/{0}", @"nicovideo.jp/watch/(\d{6,12})"),
			new RegexLinkMatcher("www.nicovideo.jp/watch/{0}", @"nico.ms/([a-z]{2}\d{4,10})"),
			new RegexLinkMatcher("www.nicovideo.jp/watch/{0}", @"nico.ms/(\d{6,12})")
		};

		/// <summary>
		/// Gets NND video ID by URL.
		/// </summary>
		/// <param name="url">NND video URL, for example http://www.nicovideo.jp/watch/sm1234567 </param>
		/// <returns>Full URL to that video, for example sm1234567. Null if URL is not recognized.</returns>
		public static string GetIdByUrl(string url) {

			var matcher = matchers.FirstOrDefault(m => m.IsMatch(url));

			if (matcher == null)
				return null;

			return matcher.GetId(url);

		}

		/// <summary>
		/// Gets NND URL by video ID.
		/// </summary>
		/// <param name="id">NND video ID, for example sm1234567.</param>
		/// <returns>Full URL to that video, for example http://www.nicovideo.jp/watch/sm1234567 </returns>
		public string GetUrlById(string id) {

			var matcher = matchers.First();
			return string.Format("http://{0}", matcher.MakeLinkFromId(id));

		}

		/// <summary>
		/// Whether the URL is a valid NicoNicoDouga video URL.
		/// </summary>
		/// <param name="url">URL to be tested.</param>
		/// <returns>True if the URl is recognized, otherwise false.</returns>
		public bool IsValidFor(string url) {

			return matchers.Any(m => m.IsMatch(url));

		}

	}

}
