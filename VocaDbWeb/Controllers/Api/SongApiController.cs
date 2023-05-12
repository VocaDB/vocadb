#nullable disable

using AspNetCore.CacheOutput;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Aggregate;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Resources;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Shared;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api;

/// <summary>
/// API queries for songs.
/// </summary>
[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
[Route("api/songs")]
[ApiController]
public class SongApiController : ApiController
{
	private const int HourInSeconds = 3600;
	private const int AbsoluteMax = 100;
	private const int DefaultMax = 10;
	private readonly IEntryLinkFactory _entryLinkFactory;
	private readonly OtherService _otherService;
	private readonly SongQueries _queries;
	private readonly SongService _service;
	private readonly SongAggregateQueries _songAggregateQueries;
	private readonly UserService _userService;
	private readonly IUserPermissionContext _userPermissionContext;
	private readonly PVHelper _pvHelper;

	/// <summary>
	/// Initializes controller.
	/// </summary>
	public SongApiController(
		SongService service,
		SongQueries queries,
		SongAggregateQueries songAggregateQueries,
		IEntryLinkFactory entryLinkFactory,
		IUserPermissionContext userPermissionContext,
		UserService userService,
		OtherService otherService,
		PVHelper pvHelper
	)
	{
		_service = service;
		_queries = queries;
		_userService = userService;
		_songAggregateQueries = songAggregateQueries;
		_entryLinkFactory = entryLinkFactory;
		_userPermissionContext = userPermissionContext;
		_otherService = otherService;
		_pvHelper = pvHelper;
	}

	/// <summary>
	/// Deletes a comment.
	/// </summary>
	/// <param name="commentId">ID of the comment to be deleted.</param>
	/// <remarks>
	/// Normal users can delete their own comments, moderators can delete all comments.
	/// Requires login.
	/// </remarks>
	[HttpDelete("comments/{commentId:int}")]
	[Authorize]
	public void DeleteComment(int commentId) =>
		_queries.DeleteComment(commentId);

	/// <summary>
	/// Deletes a song.
	/// </summary>
	/// <param name="id">ID of the song to be deleted.</param>
	/// <param name="notes">Notes.</param>
	[HttpDelete("{id:int}")]
	[Authorize]
	[ValidateAntiForgeryToken]
	public void Delete(int id, string notes = "") =>
		_service.Delete(id, notes ?? string.Empty);

	/// <summary>
	/// Gets a list of comments for a song.
	/// </summary>
	/// <param name="id">ID of the song whose comments to load.</param>
	/// <returns>List of comments in no particular order.</returns>
	/// <remarks>
	/// Pagination and sorting might be added later.
	/// </remarks>
	[HttpGet("{id:int}/comments")]
	public IEnumerable<CommentForApiContract> GetComments(int id) =>
		_queries.GetComments(id);

	[HttpGet("findDuplicate")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<NewSongCheckResultContract> GetFindDuplicate(
		[FromQuery(Name = "term[]")] string[] term = null,
		[FromQuery(Name = "pv[]")] string[] pv = null,
		[FromQuery(Name = "artistIds[]")] int[] artistIds = null,
		bool getPVInfo = false
	)
	{
		return await _queries.FindDuplicates(
			(term ?? Array.Empty<string>()).Where(p => p != null).ToArray(),
			(pv ?? Array.Empty<string>()).Where(p => p != null).ToArray(),
			artistIds,
			getPVInfo
		);
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
	[HttpGet("{id:int}/derived")]
	public IEnumerable<SongForApiContract> GetDerived(
		int id,
		SongOptionalFields fields = SongOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	) =>
		_queries.GetDerived(id, fields, lang);

#nullable enable
	[HttpGet("{id:int}/for-edit")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public SongForEditForApiContract GetForEdit(int id) =>
		_queries.GetSongForEdit(id);
#nullable disable

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
	[HttpGet("{id:int}")]
	public SongForApiContract GetById(
		int id,
		SongOptionalFields fields = SongOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	) =>
		_queries.GetSongForApi(id, fields, lang);

	/// <summary>
	/// Gets list of highlighted songs, same as front page.
	/// </summary>
	/// <remarks>
	/// Output is cached for 1 hour.
	/// </remarks>
	[HttpGet("highlighted")]
	[CacheOutput(ClientTimeSpan = HourInSeconds, ServerTimeSpan = HourInSeconds)]
	public async Task<IEnumerable<SongForApiContract>> GetHighlightedSongs(
		ContentLanguagePreference languagePreference = ContentLanguagePreference.Default,
		SongOptionalFields fields = SongOptionalFields.None
	) =>
		await _otherService.GetHighlightedSongs(languagePreference, fields);

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
	[HttpGet("{id:int}/ratings")]
	public IEnumerable<RatedSongForUserForApiContract> GetRatings(
		int id,
		UserOptionalFields userFields,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	) =>
		_queries.GetRatings(id, userFields, lang);

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
	[HttpPost("{id:int}/ratings")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	public void PostRating(int id, SongRatingContract rating) =>
		_userService.UpdateSongRating(_userPermissionContext.LoggedUserId, id, rating.Rating);

#nullable enable
	[HttpGet("by-names")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public SongForApiContract[] GetByNames(
		[FromQuery(Name = "names[]")] string[] names,
		[FromQuery(Name = "ignoreIds[]")] int[] ignoreIds,
		ContentLanguagePreference lang,
		[FromQuery(Name = "songTypes[]")] SongType[]? songTypes = null,
		int maxResults = 3
	)
	{
		return _queries.GetByNames(names, songTypes ?? new SongType[0], ignoreIds, lang, maxResults);
	}
#nullable disable

	/// <summary>
	/// Gets related songs.
	/// </summary>
	/// <param name="id">Song whose related songs are to be queried.</param>
	/// <param name="fields">Optional song fields.</param>
	/// <param name="lang">Content language preference.</param>
	/// <returns>Related songs.</returns>
	[HttpGet("{id:int}/related")]
	public RelatedSongsContract GetRelated(
		int id,
		SongOptionalFields fields = SongOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	) =>
		_queries.GetRelatedSongs(id, fields, lang);

#nullable enable
	/// <summary>
	/// Find songs.
	/// </summary>
	/// <param name="query">Song name query (optional).</param>
	/// <param name="songTypes">
	/// Filtered song types (optional). 
	/// Possible values are Original, Remaster, Remix, Cover, Instrumental, Mashup, MusicPV, DramaPV, Other.
	/// </param>
	/// <param name="afterDate">Filter by songs published after this date (inclusive).</param>
	/// <param name="beforeDate">Filter by songs published before this date (exclusive).</param>
	/// <param name="tagName">Filter by one or more tag names (optional).</param>
	/// <param name="tagId">Filter by one or more tag Ids (optional).</param>
	/// <param name="childTags">Include child tags, if the tags being filtered by have any.</param>
	/// <param name="unifyTypesAndTags">When searching by song type, search by associated tag as well, and vice versa.</param>
	/// <param name="artistId">Filter by artist Id.</param>
	/// <param name="artistParticipationStatus">
	/// Filter by artist participation status. Only valid if artistId is specified.
	/// Everything (default): Show all songs by that artist (no filter).
	/// OnlyMainAlbums: Show only main songs by that artist.
	/// OnlyCollaborations: Show only collaborations by that artist.
	/// </param>
	/// <param name="childVoicebanks">Include child voicebanks, if the artist being filtered by has any.</param>
	/// <param name="includeMembers">Include members of groups. This applies if <paramref name="artistId"/> is a group.</param>
	/// <param name="onlyWithPvs">Whether to only include songs with at least one PV.</param>
	/// <param name="pvServices">Filter by one or more PV services (separated by commas). The song will pass the filter if it has a PV for any of the matched services.</param>
	/// <param name="since">Allow only entries that have been created at most this many hours ago. By default there is no filtering.</param>
	/// <param name="minScore">Minimum rating score. Optional.</param>
	/// <param name="userCollectionId">Filter by user's rated songs. By default there is no filtering.</param>
	/// <param name="releaseEventId">Filter by release event. By default there is no filtering.</param>
	/// <param name="parentSongId">Filter by parent song. By default there is no filtering.</param>
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
	[HttpGet("")]
	public PartialFindResult<SongForApiContract> GetList(
		string query = "",
		string? songTypes = null,
		DateTime? afterDate = null,
		DateTime? beforeDate = null,
		[FromQuery(Name = "tagName[]")] string[]? tagName = null,
		[FromQuery(Name = "tagId[]")] int[]? tagId = null,
		bool childTags = false,
		bool unifyTypesAndTags = false,
		[FromQuery(Name = "artistId[]")] int[]? artistId = null,
		ArtistAlbumParticipationStatus artistParticipationStatus = ArtistAlbumParticipationStatus.Everything,
		bool childVoicebanks = false,
		bool includeMembers = false,
		bool onlyWithPvs = false,
		PVServices? pvServices = null,
		int? since = null,
		int? minScore = null,
		int? userCollectionId = null,
		int? releaseEventId = null,
		int? parentSongId = null,
		EntryStatus? status = null,
		[FromQuery(Name = "advancedFilters")] AdvancedSearchFilterParams[]? advancedFilters = null,
		int start = 0, int maxResults = DefaultMax, bool getTotalCount = false,
		SongSortRule sort = SongSortRule.Name,
		bool preferAccurateMatches = false,
		NameMatchMode nameMatchMode = NameMatchMode.Exact,
		SongOptionalFields fields = SongOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default,
		int? minMilliBpm = null,
		int? maxMilliBpm = null,
		int? minLength = null,
		int? maxLength = null
	)
	{
		var textQuery = SearchTextQuery.Create(query, nameMatchMode);
		var types = EnumVal<SongType>.ParseMultiple(songTypes);

		var param = new SongQueryParams(
			textQuery,
			types,
			start,
			Math.Min(maxResults, AbsoluteMax),
			getTotalCount,
			sort,
			onlyByName: false,
			moveExactToTop: preferAccurateMatches,
			ignoredIds: null
		)
		{
			ArtistParticipation =
			{
				ArtistIds = artistId,
				Participation = artistParticipationStatus,
				ChildVoicebanks = childVoicebanks,
				IncludeMembers = includeMembers
			},
			AfterDate = afterDate,
			BeforeDate = beforeDate,
			TagIds = tagId,
			Tags = tagName,
			ChildTags = childTags,
			UnifyEntryTypesAndTags = unifyTypesAndTags,
			OnlyWithPVs = onlyWithPvs,
			TimeFilter = since.HasValue ? TimeSpan.FromHours(since.Value) : TimeSpan.Zero,
			MinScore = minScore ?? 0,
			PVServices = pvServices,
			UserCollectionId = userCollectionId ?? 0,
			ReleaseEventId = releaseEventId ?? 0,
			ParentSongId = parentSongId ?? 0,
			AdvancedFilters = advancedFilters?.Select(advancedFilter => advancedFilter.ToAdvancedSearchFilter()).ToArray(),
			LanguagePreference = lang,
			MinMilliBpm = minMilliBpm,
			MaxMilliBpm = maxMilliBpm,
			MinLength = minLength,
			MaxLength = maxLength,
		};
		param = param with { Common = param.Common with { EntryStatus = status } };

		var songs = _service.Find(s => new SongForApiContract(s, null, lang, _userPermissionContext, fields), param);

		return songs;
	}
#nullable disable

	/// <summary>
	/// Gets lyrics by ID.
	/// </summary>
	/// <param name="lyricsId">Lyrics ID.</param>
	/// <returns>Lyrics information.</returns>
	/// <remarks>
	/// Output is cached. Specify song version as parameter to refresh.
	/// </remarks>
	[HttpGet("lyrics/{lyricsId:int}")]
	[CacheOutput(ClientTimeSpan = 3600, ServerTimeSpan = 3600)]
	public ActionResult<LyricsForSongContract> GetLyrics(int lyricsId)
	{
		return _userPermissionContext.HasPermission(PermissionToken.ViewLyrics)
			? _queries.GetLyrics(lyricsId)
			: NotFound();
	}

	/// <summary>
	/// Gets a list of song names. Ideal for autocomplete boxes.
	/// </summary>
	/// <param name="query">Text query.</param>
	/// <param name="nameMatchMode">Name match mode.</param>
	/// <param name="maxResults">Maximum number of results.</param>
	/// <returns>List of song names.</returns>
	[HttpGet("names")]
	public string[] GetNames(
		string query = "",
		NameMatchMode nameMatchMode = NameMatchMode.Auto,
		int maxResults = 15
	) =>
		_service.FindNames(SearchTextQuery.Create(query, nameMatchMode), maxResults);

#nullable enable
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
	[HttpGet("byPv")]
	public SongForApiContract? GetByPV(
		PVService pvService,
		string pvId,
		SongOptionalFields fields = SongOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	)
	{
		return _service.GetSongWithPV(s => new SongForApiContract(s, null, lang, _userPermissionContext, fields), pvService, pvId);
	}
#nullable disable

	[HttpGet("ids")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public IEnumerable<int> GetIds() =>
		_queries.GetIds();

	[HttpGet("{id:int}/pvs")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public string GetPVId(int id, PVService service) =>
		_queries.PVForSongAndService(id, service).PVId;

	[HttpGet("over-time")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[CacheOutput(ClientTimeSpan = 600, ServerTimeSpan = 600)]
	public CountPerDayContract[] GetSongsOverTime(
		TimeUnit timeUnit,
		int artistId = 0,
		int tagId = 0
	) =>
		_songAggregateQueries.SongsOverTime(timeUnit, true, null, artistId, tagId);

	[HttpGet("{id:int}/tagSuggestions")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<IEnumerable<TagUsageForApiContract>> GetTagSuggestions(int id) =>
		await _queries.GetTagSuggestionsAsync(id);
	
	[HttpGet("{id:int}/tagUsages")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public EntryWithTagUsagesForApiContract GetTagUsages(int id)  => _service.GetEntryWithTagUsages(id);

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
	[HttpGet("top-rated")]
	[CacheOutput(ClientTimeSpan = 600, ServerTimeSpan = 600)]
	public async Task<IEnumerable<SongForApiContract>> GetTopSongs(
		int? durationHours = null,
		DateTime? startDate = null,
		TopSongsDateFilterType? filterBy = null,
		SongVocalistSelection? vocalist = null,
		int maxResults = 25,
		SongOptionalFields fields = SongOptionalFields.None,
		ContentLanguagePreference languagePreference = ContentLanguagePreference.Default
	) =>
		await _queries.GetTopRated(durationHours, startDate, filterBy, vocalist, maxResults, fields, languagePreference);

	/// <summary>
	/// Updates a comment.
	/// </summary>
	/// <param name="commentId">ID of the comment to be edited.</param>
	/// <param name="contract">New comment data. Only message can be edited.</param>
	/// <remarks>
	/// Normal users can edit their own comments, moderators can edit all comments.
	/// Requires login.
	/// </remarks>
	[HttpPost("comments/{commentId:int}")]
	[Authorize]
	public void PostEditComment(int commentId, CommentForApiContract contract) =>
		_queries.PostEditComment(commentId, contract);

	/// <summary>
	/// Posts a new comment.
	/// </summary>
	/// <param name="id">ID of the song for which to create the comment.</param>
	/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
	/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
	[HttpPost("{id:int}/comments")]
	[Authorize]
	public CommentForApiContract PostNewComment(int id, CommentForApiContract contract) =>
		_queries.CreateComment(id, contract);

	[HttpPost("{id:int}/pvs")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task PostPVs(int id, PVContract[] pvs) =>
		await _queries.PostPVs(id, pvs);

	[HttpPost("{id:int}/personal-description")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	public void PostPersonalDescription(int id, SongDetailsContract data) =>
		_queries.UpdatePersonalDescription(id, data);

#nullable enable
	[HttpGet("{id:int}/details")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public SongDetailsForApiContract GetDetails(int id, int albumId = 0)
	{
		WebHelper.VerifyUserAgent(Request);

		return _queries.GetSongDetailsForApi(
			songId: id,
			albumId: albumId,
			hostname: WebHelper.GetHostnameForValidHit(Request),
			languagePreference: null,
			userLanguages: WebHelper.GetUserLanguageCodes(Request)
		);
	}

	[HttpGet("{id:int}/versions")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public EntryWithArchivedVersionsForApiContract<SongForApiContract> GetSongWithArchivedVersions(int id) =>
		_queries.GetSongWithArchivedVersionsForApi(id);

	[HttpGet("versions/{id:int}")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ArchivedSongVersionDetailsForApiContract GetVersionDetails(int id, int comparedVersionId = 0) =>
		_queries.GetVersionDetailsForApi(id, comparedVersionId);

	[HttpPost("")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ValidateAntiForgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult<int>> Create(
		[ModelBinder(BinderType = typeof(JsonModelBinder))] CreateSongForApiContract contract
	)
	{
		if (contract.Names.All(name => string.IsNullOrWhiteSpace(name.Value)))
			ModelState.AddModelError("Names", ViewRes.EntryCreateStrings.NeedName);

		if (contract.Artists is null || !contract.Artists.Any())
			ModelState.AddModelError("Artists", ViewRes.Song.CreateStrings.NeedArtist);

		if (!ModelState.IsValid)
			return ValidationProblem(ModelState);

		try
		{
			var song = await _queries.Create(contract);

			return song.Id;
		}
		catch (VideoParseException x)
		{
			ModelState.AddModelError("PVUrl", x.Message);
			return ValidationProblem(ModelState);
		}
	}

	[HttpPost("{id:int}")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ValidateAntiForgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult<int>> Edit(
		[ModelBinder(BinderType = typeof(JsonModelBinder))] SongForEditForApiContract contract
	)
	{
		// Unable to continue if viewmodel is null because we need the ID at least
		if (contract is null)
		{
			return BadRequest("Viewmodel was null - probably JavaScript is disabled");
		}

		try
		{
			static void CheckModel(SongForEditForApiContract contract)
			{
				if (contract is null)
					throw new InvalidFormException("Model was null");

				if (contract.Artists is null)
					throw new InvalidFormException("ArtistLinks list was null"); // Shouldn't be null

				if (contract.Lyrics is null)
					throw new InvalidFormException("Lyrics list was null"); // Shouldn't be null

				if (contract.Names is null)
					throw new InvalidFormException("Names list was null"); // Shouldn't be null

				if (contract.PVs is null)
					throw new InvalidFormException("PVs list was null"); // Shouldn't be null

				if (contract.WebLinks is null)
					throw new InvalidFormException("WebLinks list was null"); // Shouldn't be null
			}

			CheckModel(contract);
		}
		catch (InvalidFormException x)
		{
			ControllerBase.AddFormSubmissionError(this, x.Message);
		}

		// Note: name is allowed to be whitespace, but not empty.
		if (contract.Names is null || contract.Names.All(n => n is null || string.IsNullOrEmpty(n.Value)))
		{
			ModelState.AddModelError("Names", SongValidationErrors.UnspecifiedNames);
		}

		if (contract.Lyrics is not null && contract.Lyrics.Any(n => string.IsNullOrEmpty(n.Value)))
		{
			ModelState.AddModelError("Lyrics", "Lyrics cannot be empty");
		}

		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		await _queries.UpdateBasicProperties(contract);

		return contract.Id;
	}

	[HttpPost("{id:int}/merge")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ValidateAntiForgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult Merge(int id, int? targetSongId)
	{
		if (targetSongId == null)
		{
			ModelState.AddModelError("targetSongId", "Song must be selected");
			return ValidationProblem(ModelState);
		}

		_queries.Merge(id, targetSongId.Value);

		return NoContent();
	}

	/// <summary>
	/// Returns a PV player with song rating by song Id. Primary PV will be chosen.
	/// </summary>
	[HttpGet("{id:int}/with-rating")]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ApiExplorerSettings(IgnoreApi = true)]
	public SongWithPVPlayerAndVoteContract? PVPlayerWithRating(int id)
	{
		var song = _queries.GetSongWithPVAndVote(id, true, WebHelper.GetHostnameForValidHit(Request));
		var pv = _pvHelper.PrimaryPV(song.PVs!);

		if (pv is null)
		{
			return null;
		}

		return new SongWithPVPlayerAndVoteContract
		{
			Song = song,
			PVService = pv.Service,
		};
	}

	[HttpPost("versions/{archivedVersionId:int}/update-visibility")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ValidateAntiForgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult UpdateVersionVisibility(int archivedVersionId, bool hidden)
	{
		_queries.UpdateVersionVisibility<ArchivedSongVersion>(archivedVersionId, hidden);

		return NoContent();
	}

	[HttpPost("versions/{archivedVersionId:int}/revert")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ValidateAntiForgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult<int> RevertToVersion(int archivedVersionId)
	{
		var result = _queries.RevertToVersion(archivedVersionId);

		return result.Id;
	}
#nullable disable
}
