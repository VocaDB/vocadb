using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Web.Models.Search {

	public class SearchIndexViewModel {

		public SearchIndexViewModel()
			: this(EntryType.Undefined) {}

		public SearchIndexViewModel(EntryType searchType, string filter = null) {
			AllowRedirect = true;
			SearchType = searchType;
			Filter = filter;
		}

		public bool AllowRedirect { get; set; }

		public int[] ArtistId { get; set; }

		public ArtistType? ArtistType { get; set; }

		public bool? Autoplay { get; set; }

		public bool? ChildVoicebanks { get; set; }

		public DiscType? DiscType { get; set; }

		public string Filter { get; set; }

		public int? MinScore { get; set; }

		public bool? OnlyRatedSongs { get; set; }

		public bool? OnlyWithPVs { get; set; }

		public int? PageSize { get; set; }

		public EntryType SearchType { get; set; }

		public string SearchTypeName {
			get {
				return SearchType != EntryType.Undefined ? SearchType.ToString() : "Anything";
			}
		}

		public bool? Shuffle { get; set; }

		public int? Since { get; set; }

		public string Tag { get; set; }

		public int[] TagId { get; set; }

		public SongType? SongType { get; set; }

		public string Sort { get; set; }

		public string ViewMode { get; set; }


	}

}