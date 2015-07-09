using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using NHibernate;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.SongImport;

namespace VocaDb.Model.Service {

	public class RankingService : ServiceBase {

		public RankingService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(sessionFactory, permissionContext, entryLinkFactory) {
		}

		public int CreateSongListFromWVR(string url, bool parseAll) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			var parsed = new SongListImporters().GetSongs(url, parseAll);

			var isRanking = parsed.WVRNumber > 0;
			var listName = isRanking ? string.Format("Weekly Vocaloid ranking #{0}", parsed.WVRNumber) : parsed.Name ?? "New list";
			var category = isRanking && PermissionContext.HasPermission(PermissionToken.EditFeaturedLists) ? 
				SongListFeaturedCategory.VocaloidRanking : SongListFeaturedCategory.Nothing;
			var description = isRanking ? parsed.Name : parsed.Description ?? string.Empty;

			return HandleTransaction(session => {

				var user = GetLoggedUser(session);

				var list = new SongList(listName, user) { 
					Description = description, 
					FeaturedCategory = category 
				};
				session.Save(list);

				foreach (var entry in parsed.Songs.Items) {

					var song = session.Query<PVForSong>()
						.Where(p => p.Service == entry.PVService && p.PVId == entry.PVId)
						.First().Song;

					session.Save(list.AddSong(song));

				}

				AuditLog(string.Format("created {0}", EntryLinkFactory.CreateEntryLink(list)), session, user);
				return list.Id;

			});

		}

	}

	public class WVRListResult {

		public WVRListResult(string name, string description, int wvrNumber, IEnumerable<WVRListEntryResult> entries) {

			Name = name;
			Description = description;
			WVRNumber = wvrNumber;
			Entries = entries.ToArray();

		}

		public string Description { get; set; }

		public WVRListEntryResult[] Entries { get; set; }

		public string Name { get; set; }

		public int WVRNumber { get; set; }

		public bool IsIncomplete {
			get {
				return Entries.Any(e => e.Song == null);
			}
		}

	}

	public class WVRListEntryResult {

		public WVRListEntryResult(string nicoId, int order, string name, string url, SongContract song) {

			NicoId = nicoId;
			Order = order;
			Name = name;
			Url = url;
			Song = song;

		}

		public string Name { get; set; }

		public string NicoId { get; set; }

		public int Order { get; set; }

		public SongContract Song { get; set; }

		public string Url { get; set; }

	}

}
