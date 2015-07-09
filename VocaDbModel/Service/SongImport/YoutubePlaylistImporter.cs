using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using NLog;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.SongImport {

	public class YoutubePlaylistImporter : ISongListImporter {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static readonly Regex regex = new Regex(@"www\.youtube\.com/playlist\?list=(\w+)");

		private string YoutubeApiKey {
			get {
				return AppConfig.YoutubeApiKey;
			}
		}

		private string GetId(string url) {
			
			var match = regex.Match(url);

			if (!match.Success) {
				log.Warn("Youtube URL not regonized: {0}", url);
				return null;				
			}

			var id = match.Groups[1].Value;
			return id;

		}

		private PartialImportedSongs GetSongs(YouTubeService youtubeService, string playlistId, string pageToken, int maxResults, bool parseAll) {
			
			var songs = new List<ImportedSongInListContract>();

			try {

				var request = youtubeService.PlaylistItems.List("snippet");
				request.PlaylistId = playlistId;
				request.MaxResults = maxResults;
				request.PageToken = pageToken;

				var result = request.Execute();
				
				if (!result.Items.Any()) {
					log.Info("Youtube playlist contained no songs");
					return null;
				}

				foreach (var item in result.Items) {
					var song = new ImportedSongInListContract(PVService.Youtube, item.Snippet.ResourceId.VideoId) {
						Name = item.Snippet.Title,
						SortIndex = (int?)item.Snippet.Position ?? 0,
						Url = string.Format("https://www.youtube.com/watch?v={0}", item.Snippet.ResourceId.VideoId)
					};
					songs.Add(song);
				}

				return new PartialImportedSongs(songs.ToArray(), result.PageInfo.TotalResults ?? 0, result.NextPageToken);

			} catch (Exception x) {
				log.Warn(x, "Unable to read Youtube playlist");
				return null;
			}


		}

		public PartialImportedSongs GetSongs(string url, string pageToken, bool parseAll) {
			
			var youtubeService = new YouTubeService(new BaseClientService.Initializer {
				ApiKey = YoutubeApiKey,
				ApplicationName = "VocaDB"
			});			

			return GetSongs(youtubeService, GetId(url), pageToken, 20, parseAll);

		}

		public ImportedSongListContract Parse(string url, bool parseAll) {
			
			var match = regex.Match(url);

			if (!match.Success) {
				log.Warn("Youtube URL not regonized: {0}", url);
				return null;				
			}

			var id = match.Groups[1].Value;

			var youtubeService = new YouTubeService(new BaseClientService.Initializer {
				ApiKey = YoutubeApiKey,
				ApplicationName = "VocaDB"
			});			

			string name, description;
			DateTime created;

			try {

				var request = youtubeService.Playlists.List("snippet");
				request.Id = id;

				var result = request.Execute();
				
				if (!result.Items.Any()) {
					log.Info("Youtube playlist contained no songs");
					return null;
				}

				name = result.Items[0].Snippet.Title;
				description = result.Items[0].Snippet.Description;
				created = result.Items[0].Snippet.PublishedAt ?? DateTime.Now;

			} catch (Exception x) {
				log.Warn(x, "Unable to read Youtube playlist");
				return null;
			}

			var songs = GetSongs(youtubeService, id, null, 10, parseAll);
			
			return new ImportedSongListContract { Name = name, Description = description, CreateDate = created, Songs = songs };

		}

		public bool MatchUrl(string url) {
			return regex.IsMatch(url);			
		}

	}

}
