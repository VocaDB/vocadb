using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using NLog;
using VocaDb.Model.DataContracts.Ranking;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service.VideoServices;
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

		public RankingContract GetSongs(string url, bool parseAll) {
			
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
			var songs = new List<SongInRankingContract>();

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

			try {

				var request = youtubeService.PlaylistItems.List("snippet");
				request.PlaylistId = id;
				request.MaxResults = 20;

				var result = request.Execute();
				
				if (!result.Items.Any()) {
					log.Info("Youtube playlist contained no songs");
					return null;
				}

				int order = 1;

				foreach (var item in result.Items) {
					var song = new SongInRankingContract(PVService.Youtube, item.Snippet.ResourceId.VideoId) {
						Name = item.Snippet.Title,
						SortIndex = order++,
						Url = "youtube.com/watch?v=" + item.Snippet.ResourceId.VideoId
					};
					songs.Add(song);
				}

				return new RankingContract { Name = name, Description = description, CreateDate = created, Songs = songs.ToArray() };

			} catch (Exception x) {
				log.Warn(x, "Unable to read Youtube playlist");
				return null;
			}

		}

		public bool MatchUrl(string url) {
			return regex.IsMatch(url);			
		}

	}

}
