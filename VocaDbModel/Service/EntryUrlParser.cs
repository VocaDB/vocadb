#nullable disable

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service
{
	/// <summary>
	/// Parses entry type and ID from URLs to common entries, for example https://vocadb.net/S/3939.
	/// This default implementation is based on regular expressions and handles most common cases.
	/// </summary>
	public class EntryUrlParser : IEntryUrlParser
	{
		private readonly Dictionary<string, EntryType> entryTypeNames = new Dictionary<string, EntryType>(StringComparer.InvariantCultureIgnoreCase) {
			{ "Al", EntryType.Album },
			{ "Album/Details", EntryType.Album },
			{ "Ar", EntryType.Artist },
			{ "Artist/Details", EntryType.Artist },
			{ "E", EntryType.ReleaseEvent },
			{ "Es", EntryType.ReleaseEventSeries },
			{ "S", EntryType.Song },
			{ "Song/Details", EntryType.Song },
			{ "L", EntryType.SongList },
			{ "T", EntryType.Tag }
		};

		private readonly Regex entryUrlRegex;
		private readonly Regex entryUrlRegexOptionalPrefix;

		public EntryUrlParser()
			: this(AppConfig.HostAddress) { }

		public EntryUrlParser(string hostAddress)
		{
			hostAddress = hostAddress.Replace("https://", "https?://");
			var hostAddresses = VocaUriBuilder.RemoveTrailingSlash(hostAddress);

			var entryUrlRegexTemplate = @"^(?:{0}){1}/(Al|Ar|E|Es|S|L|T|Album/Details|Artist/Details|Song/Details)/(\d+)";

			entryUrlRegex = new Regex(string.Format(entryUrlRegexTemplate, hostAddresses, string.Empty), RegexOptions.IgnoreCase);

			entryUrlRegexOptionalPrefix = new Regex(string.Format(entryUrlRegexTemplate, hostAddresses, "?"), RegexOptions.IgnoreCase);
		}

		public GlobalEntryId Parse(string url, bool allowRelative = false)
		{
			if (string.IsNullOrEmpty(url))
				return GlobalEntryId.Empty;

			var match = allowRelative ? entryUrlRegexOptionalPrefix.Match(url) : entryUrlRegex.Match(url);

			if (!match.Success)
				return GlobalEntryId.Empty;

			var entryTypeStr = match.Groups[1].Value; // URL portion that identifies the entry type, for example "Al" or "Album/Details".
			var entryId = match.Groups[2].Value; // Entry ID, integer
			EntryType entryType;

			if (entryTypeNames.TryGetValue(entryTypeStr, out entryType))
			{
				return new GlobalEntryId(entryType, int.Parse(entryId));
			}
			else
			{
				return GlobalEntryId.Empty;
			}
		}
	}

	public interface IEntryUrlParser
	{
		/// <summary>
		/// Parse URL.
		/// </summary>
		/// <param name="url">URL to be parsed. Can be absolute or relative (if <paramref name="allowRelative"/> is true). Can be null or empty.</param>
		/// <param name="allowRelative">
		/// Whether relative URLs are allowed.
		/// If this is true, relative URLs without hostname, for example /S/3939 are parsed as well.
		/// If this is false (default), only absolute URLs such as https://vocadb.net/S/3939 are allowed.
		/// </param>
		/// <returns>
		/// Global ID, including type and ID of the entry.
		/// If the URL could not be parsed, this will be <see cref="GlobalEntryId.Empty" />.
		/// </returns>
		GlobalEntryId Parse(string url, bool allowRelative = false);
	}
}
