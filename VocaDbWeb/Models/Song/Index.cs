using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;

namespace VocaDb.Web.Models.Song {

	/// <summary>
	/// Parameter collection given to index action.
	/// </summary>
	public class IndexRouteParams {

		public IndexRouteParams() {}

		public IndexRouteParams(IndexRouteParams index, int? page)
			: this() {

			ParamIs.NotNull(() => index);

			artistId = index.artistId;
			draftsOnly = index.draftsOnly;
			filter = index.filter;
			followedByUserId = index.followedByUserId;
			hasLyrics = index.hasLyrics;
			matchMode = index.matchMode;
			minScore = index.minScore;
			onlyWithPVs = index.onlyWithPVs;
			pageSize = index.pageSize;
			since = index.since;
			songType = index.songType;
			sort = index.sort;
			userCollectionId = index.userCollectionId;
			view = index.view;
			this.page = page;

		}

		public int? artistId { get; set; }

		public bool? draftsOnly { get; set; }

		public string filter { get; set; }

		public int? followedByUserId { get; set; }

		public ContentLanguageSelection? hasLyrics { get; set; }

		public NameMatchMode? matchMode { get; set; }

		public int? minScore { get; set; }

		public bool? onlyWithPVs { get; set; }

		public int? page { get; set; }

		public int? pageSize { get; set; }

		public string since { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public SongType? songType { get; set; }

		public SongSortRule? sort { get; set; }

		public int? userCollectionId { get; set; }

		public SongViewMode? view { get; set; }

	}

	public enum SongViewMode {

		Details,

		Preview

	}

}