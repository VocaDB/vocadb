using System;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.Search.SongSearch {

	/// <summary>
	/// Query parameters for songs
	/// </summary>
	public class SongQueryParams {

		private int[] ignoredIds;
		private SongType[] songTypes;

		public SongQueryParams() {

			ArtistParticipationStatus = ArtistAlbumParticipationStatus.Everything;
			Common = new CommonSearchParams();
			IgnoredIds = new int[] {};
			Paging = new PagingProperties(0, 30, true);
			SongTypes = new SongType[] {};

		}

		/// <param name="query">Query search string. Can be null or empty, in which case no filtering by name is done.</param>
		/// <param name="songTypes">Allowed song types. Can be null or empy, in which case no filtering by song type is done.</param>
		/// <param name="start">0-based order number of the first item to be returned.</param>
		/// <param name="maxResults">Maximum number of results to be returned.</param>
		/// <param name="draftsOnly">Whether to return only entries with a draft status.</param>
		/// <param name="getTotalCount">Whether to return the total number of entries matching the criteria.</param>
		/// <param name="nameMatchMode">Mode for name maching. Ignored when query string is null or empty.</param>
		/// <param name="sortRule">Sort rule for results.</param>
		/// <param name="onlyByName">Whether to search items only by name, and not for example NicoId. Ignored when query string is null or empty.</param>
		/// <param name="moveExactToTop">Whether to move exact match to the top of search results.</param>
		/// <param name="ignoredIds">List of entries to be ignored. Can be null in which case no filtering is done.</param>
		public SongQueryParams(SearchTextQuery textQuery, SongType[] songTypes, int start, int maxResults,
			bool draftsOnly, bool getTotalCount, SongSortRule sortRule, 
			bool onlyByName, bool moveExactToTop, int[] ignoredIds) {

			Common = new CommonSearchParams(textQuery, draftsOnly, onlyByName, moveExactToTop);
			Paging = new PagingProperties(start, maxResults, getTotalCount);

			ArtistParticipationStatus = ArtistAlbumParticipationStatus.Everything;
			SongTypes = songTypes;
			SortRule = sortRule;
			IgnoredIds = ignoredIds;
			TimeFilter = TimeSpan.Zero;
			OnlyWithPVs = false;

		}

		public int ArtistId { get; set; }

		public ArtistAlbumParticipationStatus ArtistParticipationStatus { get; set; }

		public bool ChildVoicebanks { get; set; }

		public CommonSearchParams Common { get; set; }

		/// <summary>
		/// List of songs that should be ignored from search. Cannot be null. If set to empty, will be replaced with empty list.
		/// If empty, no filtering is done by song IDs.
		/// TODO: this isn't really in use anymore. Filtering by ID should be done after search.
		/// </summary>
		public int[] IgnoredIds {
			get { return ignoredIds; }
			set {
				ignoredIds = value ?? new int[] { }; 
			}
		}

		public ContentLanguageSelection[] LyricsLanguages { get; set; }

		public int MinScore { get; set; }

		public bool OnlyWithPVs { get; set; }

		public PagingProperties Paging { get; set; }

		/// <summary>
		/// List of song types that should be searched for. Cannot be null.
		/// If empty, all song types are included.
		/// </summary>
		public SongType[] SongTypes {
			get { return songTypes; }
			set {
				songTypes = value ?? new SongType[] { }; 
			}
		}

		public SongSortRule SortRule { get; set; }

		public string Tag { get; set; }

		public TimeSpan TimeFilter { get; set; }

		public int UserCollectionId { get; set; }

	}

}