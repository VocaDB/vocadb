using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.VideoServices.Youtube;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.SongImport {

	public class YoutubePlaylistImporter : ISongListImporter {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static readonly Regex regex = new Regex(@"www\.youtube\.com/playlist\?list=([\w\-_]+)");

		private const string playlistsFormat =
			"https://www.googleapis.com/youtube/v3/playlists?part=snippet&key={0}&id={1}";

		private const string playlistItemsFormat =
			"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&key={0}&playlistId={1}&maxResults={2}&pageToken={3}";

		private string YoutubeApiKey {
			get {
				return AppConfig.YoutubeApiKey;
			}
		}

		private string GetId(string url) {
			
			var match = regex.Match(url);

			if (!match.Success) {
				log.Warn("Youtube URL not regonized: {0}", url);
				throw new UnableToImportException(string.Format("Youtube URL not regonized: {0}", url));
			}

			var id = match.Groups[1].Value;
			return id;

		}

		private PartialImportedSongs GetSongsById(string playlistId, string pageToken, int maxResults, bool parseAll) {
			
			var songs = new List<ImportedSongInListContract>();

			var requestUrl = string.Format(playlistItemsFormat, YoutubeApiKey, playlistId, maxResults, pageToken);

			YoutubePlaylistItemResponse result;

			try {
				result = JsonRequest.ReadObject<YoutubePlaylistItemResponse>(requestUrl);
			} catch (Exception x) {
				log.Warn(x, "Unable to read Youtube playlist");
				throw new UnableToImportException("Unable to read Youtube playlist", x);
			}

			foreach (var item in result.Items) {
				var song = new ImportedSongInListContract(PVService.Youtube, item.Snippet.ResourceId.VideoId) {
					Name = item.Snippet.Title,
					SortIndex = ((int?)item.Snippet.Position ?? 0) + 1,
					Url = string.Format("https://www.youtube.com/watch?v={0}", item.Snippet.ResourceId.VideoId)
				};
				songs.Add(song);
			}

			return new PartialImportedSongs(songs.ToArray(), result.PageInfo.TotalResults ?? 0, result.NextPageToken);

		}

		public PartialImportedSongs GetSongs(string url, string nextPageToken, int maxResults, bool parseAll) {
			return GetSongsById(GetId(url), nextPageToken, maxResults, parseAll);
		}

		public ImportedSongListContract Parse(string url, bool parseAll) {
			
			var id = GetId(url);

			var requestUrl = string.Format(playlistsFormat, YoutubeApiKey, id);
			YoutubePlaylistResponse result;
			
			try {
				result = JsonRequest.ReadObject<YoutubePlaylistResponse>(requestUrl);
			} catch (Exception x) {
				log.Warn(x, "Unable to read Youtube playlist");
				throw new UnableToImportException("Unable to read Youtube playlist");
			}

			if (!result.Items.Any()) {
				log.Info("Youtube playlist not found");
				throw new UnableToImportException(string.Format("Youtube playlist not found: {0}", url));
			}

			var name = result.Items[0].Snippet.Title;
			var description = result.Items[0].Snippet.Description;
			var created = (result.Items[0].Snippet.PublishedAt ?? DateTimeOffset.Now).DateTime;

			var songs = GetSongsById(id, null, 10, parseAll);

			return new ImportedSongListContract { Name = name, Description = description, CreateDate = created, Songs = songs };

		}

		public bool MatchUrl(string url) {
			return regex.IsMatch(url);			
		}

		public class YoutubePlaylistResponse : YoutubeResponse<YoutubePlaylistItem> {}

		public class YoutubePlaylistItem : YoutubeItem<Snippet> {}

		public class YoutubePlaylistItemResponse : YoutubeResponse<YoutubePlaylistItemItem> {}

		public class YoutubePlaylistItemItem : YoutubeItem<YoutubePlaylistItemSnippet> {}

		public class YoutubePlaylistItemSnippet : Snippet {
			
			public uint? Position { get; set; }

			public YoutubeResourceId ResourceId { get; set; }

		}

		public class YoutubeResourceId {
			
			public string VideoId { get; set; }

		}

	}

}
