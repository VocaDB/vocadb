using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Model.Utils.Search {

	public class SearchRouteParams {

		public SearchRouteParams(EntryType? searchType = null) {
			this.searchType = searchType;
		}

		public int? artistId { get; set; }

		public ArtistType? artistType { get; set; }

		public DiscType? discType { get; set; }

		public EventCategory? eventCategory { get; set; }

		public string filter { get; set; }

		public bool? onlyRatedSongs { get; set; }

		public bool? onlyWithPVs { get; set; }

		public EntryType? searchType { get; set; }

		public SongType? songType { get; set; }

		public object sort { get; set; }

		public string tag { get; set; }

		public int? tagId { get; set; }

	}

	public class SearchRouteParamsFactory {

		public static SearchRouteParamsFactory Instance => new SearchRouteParamsFactory();

		public SearchRouteParams Albums(int? tagId = null) {
			
			return new SearchRouteParams(EntryType.Album) {
				tagId = tagId,
			};

		}

		public SearchRouteParams Artists(int? tagId = null) {
			
			return new SearchRouteParams(EntryType.Artist) {
				tagId = tagId,
			};

		}

		public SearchRouteParams Entries(int? tagId = null) {

			return new SearchRouteParams(EntryType.Undefined) {
				tagId = tagId,
			};

		}

		public SearchRouteParams Events(
			int? tagId = null, 
			int? artistId = null, 
			EventSortRule? sort = null,
			EventCategory? category = null) {

			return new SearchRouteParams(EntryType.ReleaseEvent) {
				tagId = tagId,
				artistId = artistId,
				sort = sort,
				eventCategory = category
			};

		}

		public SearchRouteParams Songs(
			int? artistId = null, 
			SongType? songType = null, 
			object sort = null, 
			int? tagId = null) {
			
			return new SearchRouteParams(EntryType.Song) {
				artistId = artistId, songType =  songType, 
				sort = sort,
				tagId = tagId
			};

		}

	}

}
