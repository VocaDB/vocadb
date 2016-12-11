using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Aggregate;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Code.WebApi;
using WebApi.OutputCache.V2;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for songs.
	/// </summary>
	[System.Web.Http.RoutePrefix("api/songs")]
	public class SongApiController : ApiController {

		private const int absoluteMax = 50;
		private const int defaultMax = 10;
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly SongQueries queries;
		private readonly SongService service;
		private readonly SongAggregateQueries songAggregateQueries;
		private readonly UserService userService;
		private readonly IUserPermissionContext userPermissionContext;

		/// <summary>
		/// Initializes controller.
		/// </summary>
		public SongApiController(SongService service, SongQueries queries, SongAggregateQueries songAggregateQueries, 
			IEntryLinkFactory entryLinkFactory, IUserPermissionContext userPermissionContext, UserService userService) {
			this.service = service;
			this.queries = queries;
			this.userService = userService;
			this.songAggregateQueries = songAggregateQueries;
			this.entryLinkFactory = entryLinkFactory;
			this.userPermissionContext = userPermissionContext;
		}

		/// <summary>
		/// Deletes a comment.
		/// </summary>
		/// <param name="commentId">ID of the comment to be deleted.</param>
		/// <remarks>
		/// Normal users can delete their own comments, moderators can delete all comments.
		/// Requires login.
		/// </remarks>
		[System.Web.Http.Route("comments/{commentId:int}")]
		[System.Web.Http.Authorize]
		public void DeleteComment(int commentId) {
			
			queries.HandleTransaction(ctx => queries.Comments(ctx).Delete(commentId));

		}

		/// <summary>
		/// Deletes a song.
		/// </summary>
		/// <param name="id">ID of the song to be deleted.</param>
		/// <param name="notes">Notes.</param>
		[Route("{id:int}")]
		[Authorize]
		public void Delete(int id, string notes = "") {
			
			service.Delete(id, notes ?? string.Empty);

		}

		/// <summary>
		/// Gets a list of comments for a song.
		/// </summary>
		/// <param name="id">ID of the song whose comments to load.</param>
		/// <returns>List of comments in no particular order.</returns>
		/// <remarks>
		/// Pagination and sorting might be added later.
		/// </remarks>
		[System.Web.Http.Route("{id:int}/comments")]
		public IEnumerable<CommentForApiContract> GetComments(int id) {
			
			return queries.GetComments(id);

		}

		[System.Web.Http.Route("findDuplicate")]
		[ApiExplorerSettings(IgnoreApi = true)]
		[System.Web.Http.HttpGet]
		public NewSongCheckResultContract GetFindDuplicate([FromUri] string[] term = null, [FromUri] string[] pv = null, [FromUri] int[] artistIds = null, bool getPVInfo = false) {

			var result = queries.FindDuplicates(
				(term ?? new string[0]).Where(p => p != null).ToArray(),
				(pv ?? new string[0]).Where(p => p != null).ToArray(),
				artistIds, getPVInfo);

			return result;

		}

		/// <summary>
		/// Gets derived (alternate versions) of a song.
		/// </summary>
		/// <param name="id">Song Id (required).</param>
		/// <param name="fields">
		/// List of optional fields (optional). 
		/// Possible values are Albums, Artists, Names, PVs, Tags, ThumbUrl, WebLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <example>https://vocadb.net/api/songs/121/derived</example>
		/// <returns>List of derived songs.</returns>
		/// <remarks>
		/// Pagination and sorting might be added later.
		/// </remarks>
		[System.Web.Http.Route("{id:int}/derived")]
		public IEnumerable<SongForApiContract> GetDerived(int id, SongOptionalFields fields = SongOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			var songs = queries.HandleQuery(s => s.Load(id).AlternateVersions.Select(child => new SongForApiContract(child, null, lang, fields)).ToArray());
			return songs;

		}

		[System.Web.Http.Route("{id:int}/for-edit")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public SongForEditContract GetForEdit(int id) {
			
			return queries.GetSongForEdit(id);

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
		[System.Web.Http.Route("{id:int}")]
		public SongForApiContract GetById(int id, SongOptionalFields fields = SongOptionalFields.None, ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			var song = queries.GetSongForApi(id, fields, lang);
			return song;

		}

		/// <summary>
		/// Get ratings for a song.
		/// </summary>
		/// <param name="id">Song ID.</param>
		/// <param name="userFields">Optional fields for the users.</param>
		/// <param name="lang">Content language preference.</param>
		/// <returns>List of ratings.</returns>
		/// <remarks>
		/// The result includes ratings and user information.
		/// For users who have requested not to make their ratings public, the user will be empty.
		/// </remarks>
		[Route("{id:int}/ratings")]
		public IEnumerable<RatedSongForUserForApiContract> GetRatings(int id, UserOptionalFields userFields, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			return queries.GetRatings(id, userFields, lang);

		}

		/// <summary>
		/// Add or update rating for a specific song, for the currently logged in user.
		/// </summary>
		/// <param name="id">ID of the song to be rated.</param>
		/// <param name="rating">Rating to be given. Possible values are Nothing, Like, Favorite.</param>
		/// <remarks>
		/// If the user has already rated the song, any previous rating is replaced.
		/// Authorization cookie must be included.
		/// This API supports CORS.
		/// </remarks>
		[Route("{id:int}/ratings")]
		[Authorize]
		[AuthenticatedCorsApi(System.Web.Mvc.HttpVerbs.Post)]
		public void PostRating(int id, SongRatingContract rating) {

			userService.UpdateSongRating(userPermissionContext.LoggedUserId, id, rating.Rating);

		}

		/// <summary>
		/// Gets related songs.
		/// </summary>
		/// <param name="id">Song whose related songs are to be queried.</param>
		/// <param name="fields">Optional song fields.</param>
		/// <param name="lang">Content language preference.</param>
		/// <returns>Related songs.</returns>
		[Route("{id:int}/related")]
		public RelatedSongsContract GetRelated(int id, SongOptionalFields fields = SongOptionalFields.None, ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			return queries.GetRelatedSongs(id, fields, lang);

		}

		/// <summary>
		/// Find songs.
		/// </summary>
		/// <param name="query">Song name query (optional).</param>
		/// <param name="songTypes">
		/// Filtered song types (optional). 
		/// Possible values are Original, Remaster, Remix, Cover, Instrumental, Mashup, MusicPV, DramaPV, Other.
		/// </param>
		/// <param name="tagName">Filter by one or more tag names (optional).</param>
		/// <param name="tagId">Filter by one or more tag Ids (optional).</param>
		/// <param name="childTags">Include child tags, if the tags being filtered by have any.</param>
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
		/// <param name="userCollectionId">Filter by user's rated songs. By default there is no filtering.</param>
		/// <param name="status">Filter by entry status (optional).</param>
		/// <param name="advancedFilters">List of advanced filters (optional).</param>
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
		[System.Web.Http.Route("")]
		public PartialFindResult<SongForApiContract> GetList(
			string query = "", 
			string songTypes = null,
			[FromUri] string[] tagName = null,
			[FromUri] int[] tagId = null,
			bool childTags = false,
			[FromUri] int[] artistId = null,
			ArtistAlbumParticipationStatus artistParticipationStatus = ArtistAlbumParticipationStatus.Everything,
			bool childVoicebanks = false,
			bool onlyWithPvs = false,
			[FromUri] PVServices? pvServices = null,
			int? since = null,
			int? minScore = null,
			int? userCollectionId = null,
			EntryStatus? status = null,
			[FromUri] AdvancedSearchFilter[] advancedFilters = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			SongSortRule sort = SongSortRule.Name,
			bool preferAccurateMatches = false,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			SongOptionalFields fields = SongOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			var textQuery = SearchTextQuery.Create(query, nameMatchMode);
			var types = EnumVal<SongType>.ParseMultiple(songTypes);

			var param = new SongQueryParams(textQuery, types, start, Math.Min(maxResults, absoluteMax), getTotalCount, sort, false, preferAccurateMatches, null) {
				TagIds = tagId,
				Tags = tagName, 
				ChildTags = childTags,
				OnlyWithPVs = onlyWithPvs,
				ArtistIds = artistId,		
				ArtistParticipationStatus = artistParticipationStatus,
				ChildVoicebanks = childVoicebanks,
				TimeFilter = since.HasValue ? TimeSpan.FromHours(since.Value) : TimeSpan.Zero,
				MinScore = minScore ?? 0,
				PVServices = pvServices,
				UserCollectionId = userCollectionId ?? 0,
				AdvancedFilters = advancedFilters
			};
			param.Common.EntryStatus = status;

			var artists = service.Find(s => new SongForApiContract(s, null, lang, fields), param);

			return artists;			

		}

		/// <summary>
		/// Gets lyrics by ID.
		/// </summary>
		/// <param name="lyricsId">Lyrics ID.</param>
		/// <returns>Lyrics information.</returns>
		/// <remarks>
		/// Output is cached. Specify song version as parameter to refresh.
		/// </remarks>
		[Route("lyrics/{lyricsId:int}")]
		[CacheOutput(ClientTimeSpan = 3600, ServerTimeSpan = 3600)]
		public LyricsForSongContract GetLyrics(int lyricsId) {
			return queries.GetLyrics(lyricsId);
		}

		/// <summary>
		/// Gets a list of song names. Ideal for autocomplete boxes.
		/// </summary>
		/// <param name="query">Text query.</param>
		/// <param name="nameMatchMode">Name match mode.</param>
		/// <param name="maxResults">Maximum number of results.</param>
		/// <returns>List of song names.</returns>
		[System.Web.Http.Route("names")]
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
		[System.Web.Http.Route("")]
		[System.Web.Http.Route("byPv")]
		public SongForApiContract GetByPV(
			PVService pvService, 
			string pvId, 
			SongOptionalFields fields = SongOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			var song = service.GetSongWithPV(s => new SongForApiContract(s, null, lang, fields), pvService, pvId);

			return song;

		}

		[System.Web.Http.Route("ids")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public IEnumerable<int> GetIds() {

			var versions = queries
				.HandleQuery(ctx => ctx.Query()
					.Where(a => !a.Deleted)
					.Select(v => v.Id)
					.ToArray());

			return versions;

		}

		[System.Web.Http.Route("{id:int}/pvs")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public string GetPVId(int id, PVService service) {
			
			var pv = queries.PVForSongAndService(id, service);
			return pv.PVId;

		}

		[Route("over-time")]
		[ApiExplorerSettings(IgnoreApi = true)]
		[CacheOutput(ClientTimeSpan = 600, ServerTimeSpan = 600)]
		public CountPerDayContract[] GetSongsOverTime(TimeUnit timeUnit, int artistId = 0, int tagId = 0) {

			return songAggregateQueries.SongsOverTime(timeUnit, true, null, artistId, tagId);

		}

		/// <summary>
		/// Gets top rated songs.
		/// </summary>
		/// <param name="durationHours">Duration in hours from which to get songs.</param>
		/// <param name="startDate">Lower bound of the date. Optional.</param>
		/// <param name="filterBy">Filtering mode.</param>
		/// <param name="vocalist">Vocalist selection.</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional).</param>
		/// <param name="fields">Optional song fields to load.</param>
		/// <param name="languagePreference">Language preference.</param>
		/// <returns>List of sorts, sorted by the rating position.</returns>
		[System.Web.Http.Route("top-rated")]
		[CacheOutput(ClientTimeSpan = 600, ServerTimeSpan = 600)]
		public IEnumerable<SongForApiContract> GetTopSongs(
			int? durationHours = null, 
			DateTime? startDate = null,
			TopSongsDateFilterType? filterBy = null,
			SongVocalistSelection? vocalist = null,
			int maxResults = 25,
			SongOptionalFields fields = SongOptionalFields.None,
			ContentLanguagePreference languagePreference = ContentLanguagePreference.Default) {
			
			return queries.HandleQuery(ctx => {			

				var query = ctx.Query()
					.Where(s => !s.Deleted && s.RatingScore > 0)
					.WhereHasVocalist(vocalist ?? SongVocalistSelection.Nothing);

				if (durationHours.HasValue) {

					var timeSpan = TimeSpan.FromHours(durationHours.Value);
					DateTime? endDate = null;

					if (!startDate.HasValue) {
						startDate = DateTime.Now - timeSpan;
					} else {
						endDate = startDate + timeSpan;
					}

					switch (filterBy) {
						case TopSongsDateFilterType.PublishDate: {
							startDate = startDate?.Date;
							endDate = endDate?.Date;
							query = query.WherePublishDateIsBetween(startDate, endDate);
							break;
						}
						case TopSongsDateFilterType.CreateDate: {
							query = query.Where(s => s.CreateDate >= startDate);
							break;		
						}
						case TopSongsDateFilterType.Popularity: {
							// Sort by number of ratings and hits during that time
							// Older songs get more hits so value them even less
							query = query.OrderByDescending(s => s.UserFavorites
								.Where(f => f.Date >= startDate)
								.Sum(f => (int)f.Rating) + (s.Hits.Count(h => h.Date >= startDate) / 100));
							break;
						}
					}
						
					if (filterBy != TopSongsDateFilterType.Popularity) {
						query = query.OrderByDescending(s => s.RatingScore + (s.Hits.Count / 30));
					}

				} else {
					query = query.OrderByDescending(s => s.RatingScore);			
				}
					
				var songs = query
					.Take(maxResults)
					.ToArray();

				var contracts = songs
					.Select(s => new SongForApiContract(s, null, languagePreference, fields))
					.ToArray();

				return contracts;

			});

		}

		/// <summary>
		/// Updates a comment.
		/// </summary>
		/// <param name="commentId">ID of the comment to be edited.</param>
		/// <param name="contract">New comment data. Only message can be edited.</param>
		/// <remarks>
		/// Normal users can edit their own comments, moderators can edit all comments.
		/// Requires login.
		/// </remarks>
		[System.Web.Http.Route("comments/{commentId:int}")]
		[System.Web.Http.Authorize]
		public void PostEditComment(int commentId, CommentForApiContract contract) {
			
			queries.HandleTransaction(ctx => queries.Comments(ctx).Update(commentId, contract));

		}

		/// <summary>
		/// Posts a new comment.
		/// </summary>
		/// <param name="id">ID of the song for which to create the comment.</param>
		/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
		/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
		[System.Web.Http.Route("{id:int}/comments")]
		[System.Web.Http.Authorize]
		public CommentForApiContract PostNewComment(int id, CommentForApiContract contract) {
			
			return queries.CreateComment(id, contract);

		}

		[Route("")]
		[Authorize]
		[HttpPost]
		[ApiExplorerSettings(IgnoreApi = true)]
		[AuthenticatedCorsApi(System.Web.Mvc.HttpVerbs.Post)]
		public SongContract PostNewSong(CreateSongContract contract) {

			if (contract == null)
				throw new HttpBadRequestException("Message was empty");

			try {
				return queries.Create(contract);
			} catch (VideoParseException x) {
				throw new HttpBadRequestException(x.Message);
			} catch (ArgumentException x) {
				throw new HttpBadRequestException(x.Message);
			}

		}

		[System.Web.Http.Route("{id:int}/pvs")]
		[System.Web.Http.Authorize]
		[ApiExplorerSettings(IgnoreApi = true)]
		public void PostPVs(int id, PVContract[] pvs) {

			queries.HandleTransaction(ctx => {

				var song = ctx.Load(id);

				EntryPermissionManager.VerifyEdit(userPermissionContext, song);

				var diff = new SongDiff();

				var pvDiff = queries.UpdatePVs(ctx, song, diff, pvs);

				if (pvDiff.Changed) {

					var logStr = string.Format("updated PVs for song {0}", entryLinkFactory.CreateEntryLink(song)).Truncate(400);

					queries.Archive(ctx, song, diff, SongArchiveReason.PropertiesUpdated, string.Empty);
					ctx.Update(song);
					ctx.AuditLogger.AuditLog(logStr);

				}

			});

		}
	}

	public enum TopSongsDateFilterType {
		CreateDate,
		PublishDate,
		Popularity
	}

}