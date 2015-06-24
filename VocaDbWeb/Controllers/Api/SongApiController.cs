using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Web.Code;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for songs.
	/// </summary>
	[RoutePrefix("api/songs")]
	public class SongApiController : ApiController {

		private const int absoluteMax = 50;
		private const int defaultMax = 10;
		private readonly SongQueries queries;
		private readonly SongService service;

		/// <summary>
		/// Initializes controller.
		/// </summary>
		/// <param name="service">Song service. Cannot be null.</param>
		/// <param name="queries">Song queries. Cannot be null.</param>
		public SongApiController(SongService service, SongQueries queries) {
			this.service = service;
			this.queries = queries;
		}

		/// <summary>
		/// Deletes a comment.
		/// Normal users can delete their own comments, moderators can delete all comments.
		/// Requires login.
		/// </summary>
		/// <param name="commentId">ID of the comment to be deleted.</param>
		[Route("comments/{commentId:int}")]
		[Authorize]
		public void DeleteComment(int commentId) {
			
			queries.HandleTransaction(ctx => queries.Comments(ctx).Delete(commentId));

		}

		/// <summary>
		/// Deletes a song.
		/// </summary>
		/// <param name="songId">ID of the song to be deleted.</param>
		/// <param name="notes">Notes.</param>
		[Route("{songId:int}")]
		[Authorize]
		public void Delete(int songId, string notes = "") {
			
			service.Delete(songId, notes ?? string.Empty);

		}

		/// <summary>
		/// Gets a list of comments for a song.
		/// Note: pagination and sorting might be added later.
		/// </summary>
		/// <param name="songId">ID of the song whose comments to load.</param>
		/// <returns>List of comments in no particular order.</returns>
		[Route("{songId:int}/comments")]
		public IEnumerable<CommentForApiContract> GetComments(int songId) {
			
			return queries.GetComments(songId);

		}

		[Route("{id:int}/for-edit")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public SongForEditContract GetForEdit(int id) {
			
			return service.GetSongForEdit(id);

		}

		/// <summary>
		/// Gets a song by Id.
		/// </summary>
		/// <param name="id">Song Id (required).</param>
		/// <param name="fields">
		/// List of optional fields (optional). 
		/// Possible values are Albums, Artists, Names, PVs, Tags, ThumbUrl, WebLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <example>http://vocadb.net/api/songs/121</example>
		/// <returns>Song data.</returns>
		[Route("{id:int}")]
		public SongForApiContract GetById(int id, SongOptionalFields fields = SongOptionalFields.None, ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			var song = service.GetSongWithMergeRecord(id, (s, m) => new SongForApiContract(s, m, lang, fields));

			return song;

		}

		/// <summary>
		/// Find songs.
		/// </summary>
		/// <param name="query">Song name query (optional).</param>
		/// <param name="songTypes">
		/// Filtered song types (optional). 
		/// Possible values are Original, Remaster, Remix, Cover, Instrumental, Mashup, MusicPV, DramaPV, Other.
		/// </param>
		/// <param name="tag">Filter by tag (optional).</param>
		/// <param name="artistId">Filter by artist Id.</param>
		/// <param name="artistParticipationStatus">
		/// Filter by artist participation status. Only valid if artistId is specified.
		/// Everything (default): Show all songs by that artist (no filter).
		/// OnlyMainAlbums: Show only main songs by that artist.
		/// OnlyCollaborations: Show only collaborations by that artist.
		/// </param>
		/// <param name="childVoicebanks">Include child voicebanks, if the artist being filtered by has any.</param>
		/// <param name="onlyWithPvs">Whether to only include songs with at least one PV.</param>
		/// <param name="pvServices">Filter by one or more PV services (separated by commas). The song will pass the filter if it has a PV for any of the matched services.</param>
		/// <param name="since">Allow only entries that have been created at most this many hours ago. By default there is no filtering.</param>
		/// <param name="minScore">Minimum rating score. Optional.</param>
		/// <param name="lyrics">Filter by lyrics in the specified language. By default there is no filtering.</param>
		/// <param name="userCollectionId">Filter by user's rated songs. By default there is no filtering.</param>
		/// <param name="status">Filter by entry status (optional).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">Sort rule (optional, defaults to Name). Possible values are None, Name, AdditionDate, FavoritedTimes, RatingScore.</param>
		/// <param name="preferAccurateMatches">
		/// Whether the search should prefer accurate matches. 
		/// If this is true, entries that match by prefix will be moved first, instead of being sorted alphabetically.
		/// Requires a text query. Does not support pagination.
		/// This is mostly useful for autocomplete boxes.
		/// </param>
		/// <param name="nameMatchMode">Match mode for song name (optional, defaults to Exact).</param>
		/// <param name="fields">
		/// List of optional fields (optional). Possible values are Albums, Artists, Names, PVs, Tags, ThumbUrl, WebLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of songs.</returns>
		/// <example>http://vocadb.net/api/songs?query=Nebula&amp;songTypes=Original</example>
		[Route("")]
		public PartialFindResult<SongForApiContract> GetList(
			string query = "", 
			string songTypes = null,
			[FromUri] string[] tag = null,
			int? artistId = null,
			ArtistAlbumParticipationStatus artistParticipationStatus = ArtistAlbumParticipationStatus.Everything,
			bool childVoicebanks = false,
			bool onlyWithPvs = false,
			[FromUri] PVServices? pvServices = null,
			int? since = null,
			int? minScore = null,
			[EnumArrayBinder] ContentLanguageSelection[] lyrics = null,
			int? userCollectionId = null,
			EntryStatus? status = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			SongSortRule sort = SongSortRule.Name,
			bool preferAccurateMatches = false,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			SongOptionalFields fields = SongOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			var textQuery = SearchTextQuery.Create(query, nameMatchMode);
			var types = EnumVal<SongType>.ParseMultiple(songTypes);

			var param = new SongQueryParams(textQuery, types, start, Math.Min(maxResults, absoluteMax), false, getTotalCount, sort, false, preferAccurateMatches, null) {
				Tags = tag, 
				OnlyWithPVs = onlyWithPvs,
				ArtistId = artistId ?? 0,		
				ArtistParticipationStatus = artistParticipationStatus,
				ChildVoicebanks = childVoicebanks,
				TimeFilter = since.HasValue ? TimeSpan.FromHours(since.Value) : TimeSpan.Zero,
				LyricsLanguages = lyrics,
				MinScore = minScore ?? 0,
				PVServices = pvServices,
				UserCollectionId = userCollectionId ?? 0
			};
			param.Common.EntryStatus = status;

			var artists = service.Find(s => new SongForApiContract(s, null, lang, fields), param);

			return artists;			

		}

		/// <summary>
		/// Gets a list of song names. Ideal for autocomplete boxes.
		/// </summary>
		/// <param name="query">Text query.</param>
		/// <param name="nameMatchMode">Name match mode.</param>
		/// <param name="maxResults">Maximum number of results.</param>
		/// <returns>List of song names.</returns>
		[Route("names")]
		public string[] GetNames(string query = "", NameMatchMode nameMatchMode = NameMatchMode.Auto, int maxResults = 15) {
			
			return service.FindNames(SearchTextQuery.Create(query, nameMatchMode), maxResults);

		}

		/// <summary>
		/// Gets a song by PV.
		/// </summary>
		/// <param name="pvService">
		/// PV service (required). Possible values are NicoNicoDouga, Youtube, SoundCloud, Vimeo, Piapro, Bilibili.
		/// </param>
		/// <param name="pvId">PV Id (required). For example sm123456.</param>
		/// <param name="fields">
		/// List of optional fields (optional). Possible values are Albums, Artists, Names, PVs, Tags, ThumbUrl, WebLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Song data.</returns>
		/// <example>http://vocadb.net/api/songs?pvId=sm19923781&amp;pvService=NicoNicoDouga</example>
		[Route("")]
		public SongForApiContract GetByPV(
			PVService pvService, 
			string pvId, 
			SongOptionalFields fields = SongOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			var song = service.GetSongWithPV(s => new SongForApiContract(s, null, lang, fields), pvService, pvId);

			return song;

		}

		[Route("ids")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public IEnumerable<int> GetIds() {

			var versions = queries
				.HandleQuery(ctx => ctx.Query()
					.Where(a => !a.Deleted)
					.Select(v => v.Id)
					.ToArray());

			return versions;

		}

		[Route("{songId:int}/pvs")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public string GetPVId(int songId, PVService service) {
			
			var pv = queries.PVForSongAndService(songId, service);
			return pv.PVId;

		}

		/// <summary>
		/// Updates a comment.
		/// Normal users can edit their own comments, moderators can edit all comments.
		/// Requires login.
		/// </summary>
		/// <param name="commentId">ID of the comment to be edited.</param>
		/// <param name="contract">New comment data. Only message can be edited.</param>
		[Route("comments/{commentId:int}")]
		[Authorize]
		public void PostEditComment(int commentId, CommentForApiContract contract) {
			
			queries.HandleTransaction(ctx => queries.Comments(ctx).Update(commentId, contract));

		}

		/// <summary>
		/// Posts a new comment.
		/// </summary>
		/// <param name="songId">ID of the song for which to create the comment.</param>
		/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
		/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
		[Route("{songId:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int songId, CommentForApiContract contract) {
			
			return queries.CreateComment(songId, contract);

		}

	}

}