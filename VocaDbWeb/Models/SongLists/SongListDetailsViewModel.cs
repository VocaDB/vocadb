using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.SongLists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.SongLists {

	public class SongListDetailsViewModel {

		public SongListDetailsViewModel() { }

		public SongListDetailsViewModel(SongListDetailsContract songList) {
			SongList = songList;
		}

		public bool IsFeatured {
			get {
				return SongList.FeaturedCategory != SongListFeaturedCategory.Nothing;
			}
		}

		public string SmallThumbUrl { get; set; }

		public SongListDetailsContract SongList { get; set; }

		public Dictionary<string, string> SortRules {
			get {
				return new[] { new KeyValuePair<string, string>(string.Empty, ViewRes.SongList.DetailsStrings.DefaultSortRule) }
					.Concat(Translate.SongSortRuleNames.ValuesAndNamesStrings).ToDictionary(s => s.Key, s => s.Value);
			}
		}

		public string ThumbUrl { get; set; }

	}

}