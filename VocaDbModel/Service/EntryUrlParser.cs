using System.Text.RegularExpressions;
using VocaDb.Model.Domain;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service {

	/// <summary>
	/// Parses entry type and ID from URLs to common entries, for example http://vocadb.net/S/3939.
	/// This default implementation is based on regular expressions and handles most common cases.
	/// </summary>
	public class EntryUrlParser : IEntryUrlParser {

		private readonly Regex entryUrlRegex;
		private readonly Regex entryUrlRegexOptionalPrefix;

		public EntryUrlParser()
			: this(AppConfig.HostAddress, AppConfig.HostAddressSecure) {}

		public EntryUrlParser(string hostAddress, string hostAddressSecure) {
			
			// Host addresses http and https
			var hostAddresses = VocaUriBuilder.RemoveTrailingSlash(hostAddress) + "|" + VocaUriBuilder.RemoveTrailingSlash(hostAddressSecure);

			var entryUrlRegexBase = @"^(?:{0})/(Al|Ar|S|Album/Details|Artist/Details|Song/Details)/(\d+)";

			entryUrlRegex = new Regex(string.Format(entryUrlRegexBase, hostAddresses), 
				RegexOptions.IgnoreCase);

			var entryUrlRegexBaseOptionalPrefix = @"^(?:{0})?/(Al|Ar|S|Album/Details|Artist/Details|Song/Details)/(\d+)";

			entryUrlRegexOptionalPrefix = new Regex(string.Format(entryUrlRegexBaseOptionalPrefix, hostAddresses), 
				RegexOptions.IgnoreCase);

		}

		public GlobalEntryId Parse(string url, bool allowRelative = false) {
			
			if (string.IsNullOrEmpty(url))
				return GlobalEntryId.Empty;

			var match = (allowRelative ? entryUrlRegexOptionalPrefix.Match(url) : entryUrlRegex.Match(url));

			if (!match.Success)
				return GlobalEntryId.Empty;

			var entryTypeStr = match.Groups[1].Value;
			var entryId = match.Groups[2].Value;
			EntryType entryType;

			switch (entryTypeStr.ToLowerInvariant()) {
				case "al":
				case "album/details":
					entryType = EntryType.Album;
					break;
				case "ar":
				case "artist/details":
					entryType = EntryType.Artist;
					break;
				case "s":
				case "song/details":
					entryType = EntryType.Song;
					break;
				default:
					return GlobalEntryId.Empty;
			}

			return new GlobalEntryId(entryType, int.Parse(entryId));

		}

	}

	public interface IEntryUrlParser {

		/// <summary>
		/// Parse URL.
		/// </summary>
		/// <param name="url">URL to be parsed. Can be null or empty.</param>
		/// <param name="allowRelative">
		/// Whether relative URLs are allowed.
		/// If this is true, relative URLs without hostname, for example /S/3939 are parsed as well.
		/// If this is false (default), only absolute URLs such as http://vocadb.net/S/3939 are allowed.
		/// </param>
		/// <returns>
		/// Global ID, including type and ID of the entry.
		/// If the URL could not be parsed, this will be <see cref="GlobalEntryId.Empty" />.
		/// </returns>
		GlobalEntryId Parse(string url, bool allowRelative = false);

	}

}
