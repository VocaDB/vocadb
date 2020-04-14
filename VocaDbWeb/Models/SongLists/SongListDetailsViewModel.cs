using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.SongLists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.SongLists {

	public class SongListDetailsViewModel {

		public SongListDetailsViewModel() { }

		public SongListDetailsViewModel(SongListForApiContract songList, IUserPermissionContext permissionContext) {
			CanEdit = EntryPermissionManager.CanEdit(permissionContext, songList);
			SongList = songList;
		}

		public bool CanEdit { get; set; }

		public bool IsFeatured => SongList.FeaturedCategory != SongListFeaturedCategory.Nothing;

		public string SmallThumbUrl { get; set; }

		public SongListForApiContract SongList { get; set; }

		public Dictionary<string, string> SortRules {
			get {
				return new[] { new KeyValuePair<string, string>(string.Empty, ViewRes.SongList.DetailsStrings.DefaultSortRule) }
					.Concat(Translate.SongSortRuleNames.ValuesAndNamesStrings).ToDictionary(s => s.Key, s => s.Value);
			}
		}

		public string ThumbUrl { get; set; }

	}

}