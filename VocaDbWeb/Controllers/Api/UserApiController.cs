#nullable disable

using AspNetCore.CacheOutput;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.SongLists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.Search.User;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models;
using VocaDb.Web.Models.Shared;
using VocaDb.Web.Models.User;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api;

using NLog;

/// <summary>
/// API queries for users.
/// </summary>
[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
[Route("api/users")]
[ApiController]
public class UserApiController : ApiController
{
	private const int AbsoluteMax = 100;
	private const int DefaultMax = 10;
	private readonly UserMessageQueries _messageQueries;
	private readonly IUserPermissionContext _permissionContext;
	private readonly UserQueries _queries;
	private readonly UserService _service;
	private readonly IAggregatedEntryImageUrlFactory _thumbPersister;
	private readonly IUserIconFactory _userIconFactory;
	private readonly LoginManager _loginManager;
	private readonly IPRuleManager _ipRuleManager;
	private readonly VdbConfigManager _config;
	private readonly OtherService _otherService;

	public UserApiController(
		UserQueries queries,
		UserMessageQueries messageQueries,
		UserService service,
		IUserPermissionContext permissionContext,
		IAggregatedEntryImageUrlFactory thumbPersister,
		IUserIconFactory userIconFactory,
		LoginManager loginManager,
		IPRuleManager ipRuleManager,
		VdbConfigManager config,
		OtherService otherService
	)
	{
		_queries = queries;
		_messageQueries = messageQueries;
		_service = service;
		_permissionContext = permissionContext;
		_thumbPersister = thumbPersister;
		_userIconFactory = userIconFactory;
		_loginManager = loginManager;
		_ipRuleManager = ipRuleManager;
		_config = config;
		_otherService = otherService;
	}

	[Authorize]
	[HttpDelete("current/events/{eventId:int}")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public void DeleteEventAttendance(int eventId) =>
		_queries.UpdateEventForUser(_permissionContext.LoggedUserId, eventId, null);

	[HttpDelete("current/followedTags/{tagId:int}")]
	[Authorize]
	public void DeleteFollowedTag(int tagId) =>
		_queries.RemoveFollowedTag(_permissionContext.LoggedUserId, tagId);

	/// <summary>
	/// Deletes a comment.
	/// </summary>
	/// <param name="commentId">ID of the comment to be deleted.</param>
	/// <remarks>
	/// Normal users can delete their own comments, moderators can delete all comments.
	/// Requires login.
	/// </remarks>
	[HttpDelete("profileComments/{commentId:int}")]
	[Authorize]
	public void DeleteProfileComment(int commentId) =>
		_service.DeleteComment(commentId);

#nullable enable
	/// <summary>
	/// Gets a list of albums in a user's collection.
	/// </summary>
	/// <param name="id">ID of the user whose albums are to be browsed.</param>
	/// <param name="query">Album name query (optional).</param>
	/// <param name="tagId">Filter by tag Id (optional).</param>
	/// <param name="tag">Filter by tag (optional).</param>
	/// <param name="artistId">Filter by album artist (optional).</param>
	/// <param name="purchaseStatuses">
	/// Filter by a comma-separated list of purchase statuses (optional). Possible values are Nothing, Wishlisted, Ordered, Owned, and all combinations of these.
	/// </param>
	/// <param name="releaseEventId">Filter by release event. Optional.</param>
	/// <param name="albumTypes">Filter by album type (optional).</param>
	/// <param name="advancedFilters">List of advanced filters (optional).</param>
	/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
	/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
	/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
	/// <param name="sort">Sort rule (optional, defaults to Name). Possible values are None, Name, ReleaseDate, AdditionDate, RatingAverage, RatingTotal, CollectionCount.</param>
	/// <param name="nameMatchMode">Match mode for album name (optional, defaults to Auto).</param>
	/// <param name="fields">
	/// List of optional fields (optional). Possible values are Artists, MainPicture, Names, PVs, Tags, WebLinks.
	/// </param>
	/// <param name="lang">Content language preference (optional).</param>
	/// <returns>Page of albums with collection properties.</returns>
	/// <remarks>
	/// This includes albums that have been rated by the user as well as albums that the user has bought or wishlisted.
	/// Note that the user might have set his album ownership status and media type as private, in which case those properties are not included.
	/// </remarks>
	[HttpGet("{id:int}/albums")]
	public PartialFindResult<AlbumForUserForApiContract> GetAlbumCollection(
		int id,
		string query = "",
		int? tagId = null,
		string? tag = null,
		int? artistId = null,
		PurchaseStatuses? purchaseStatuses = null,
		int releaseEventId = 0,
		DiscType albumTypes = DiscType.Unknown,
		[FromQuery(Name = "advancedFilters")] AdvancedSearchFilterParams[]? advancedFilters = null,
		int start = 0,
		int maxResults = DefaultMax,
		bool getTotalCount = false,
		AlbumSortRule? sort = null,
		NameMatchMode nameMatchMode = NameMatchMode.Exact,
		AlbumOptionalFields fields = AlbumOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default,
		MediaType? mediaType = null
	)
	{
		maxResults = Math.Min(maxResults, AbsoluteMax);
		var textQuery = SearchTextQuery.Create(query, nameMatchMode);

		var queryParams = new AlbumCollectionQueryParams(id, new PagingProperties(start, maxResults, getTotalCount))
		{
			AlbumType = albumTypes,
			ArtistId = artistId ?? 0,
			FilterByStatus = purchaseStatuses != null ? purchaseStatuses.Value.ToIndividualSelections().ToArray() : null,
			TextQuery = textQuery,
			ReleaseEventId = releaseEventId,
			Sort = sort ?? AlbumSortRule.Name,
			TagId = tagId ?? 0,
			Tag = tag,
			AdvancedFilters = advancedFilters?.Select(advancedFilter => advancedFilter.ToAdvancedSearchFilter()).ToArray(),
			MediaType = mediaType,
		};

		var albums = _queries.GetAlbumCollection(queryParams, (afu, shouldShowCollectionStatus) =>
			new AlbumForUserForApiContract(afu, lang, _permissionContext, _thumbPersister, fields, shouldShowCollectionStatus));

		return albums;
	}
#nullable disable

	/// <summary>
	/// Gets information about the currently logged in user.
	/// </summary>
	/// <param name="fields">Optional fields.</param>
	/// <returns>User details.</returns>
	/// <remarks>
	/// Requires login information.
	/// This API supports CORS, and is restricted to specific origins.
	/// </remarks>
	[HttpGet("current")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[RequireSsl]
	public UserForApiContract GetCurrent(UserOptionalFields fields = UserOptionalFields.None) =>
		_queries.GetUser(_permissionContext.LoggedUserId, fields);

	/// <summary>
	/// Gets a list of events a user has subscribed to.
	/// </summary>
	/// <param name="id">User ID.</param>
	/// <param name="relationshipType">Type of event subscription.</param>
	/// <returns>List of events.</returns>
	[HttpGet("{id:int}/events")]
	public ReleaseEventForApiContract[] GetEvents(int id, UserEventRelationshipType relationshipType) =>
		_queries.GetEvents(id, relationshipType, ReleaseEventOptionalFields.AdditionalNames | ReleaseEventOptionalFields.MainPicture | ReleaseEventOptionalFields.Series);

#nullable enable
	/// <summary>
	/// Gets a list of artists followed by a user.
	/// </summary>
	/// <param name="id">ID of the user whose followed artists are to be browsed.</param>
	/// <param name="query">Artist name query (optional).</param>
	/// <param name="tagId">Filter by tag Id (optional). This filter can be specified multiple times.</param>
	/// <param name="artistType">Filter by artist type.</param>
	/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
	/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
	/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
	/// <param name="sort">Sort rule (optional, defaults to Name). Possible values are None, Name, AdditionDate, AdditionDateAsc.</param>
	/// <param name="nameMatchMode">Match mode for artist name (optional, defaults to Auto).</param>
	/// <param name="fields">List of optional fields (optional). Possible values are Description, Groups, Members, Names, Tags, WebLinks.</param>
	/// <param name="lang">Content language preference (optional).</param>
	/// <returns>Page of artists.</returns>
	[HttpGet("{id:int}/followedArtists")]
	public PartialFindResult<ArtistForUserForApiContract> GetFollowedArtists(
		int id,
		string query = "",
		[FromQuery(Name = "tagId[]")] int[]? tagId = null,
		ArtistType artistType = ArtistType.Unknown,
		int start = 0,
		int maxResults = DefaultMax,
		bool getTotalCount = false,
		ArtistSortRule sort = ArtistSortRule.Name,
		NameMatchMode nameMatchMode = NameMatchMode.Auto,
		ArtistOptionalFields fields = ArtistOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	)
	{
		maxResults = Math.Min(maxResults, AbsoluteMax);
		var textQuery = ArtistSearchTextQuery.Create(query, nameMatchMode);

		var queryParams = new FollowedArtistQueryParams
		{
			UserId = id,
			ArtistType = artistType,
			Paging = new PagingProperties(start, maxResults, getTotalCount),
			SortRule = sort,
			TagIds = tagId,
			TextQuery = textQuery
		};

		var artists = _queries.GetArtists(
			queryParams,
			afu => new ArtistForUserForApiContract(afu, lang, _permissionContext, _thumbPersister, fields)
		);

		return artists;
	}

	/// <summary>
	/// Gets a list of users.
	/// </summary>
	/// <param name="query">User name query (optional).</param>
	/// <param name="groups">Filter by user group. Only one value supported for now. Optional.</param>
	/// <param name="joinDateAfter">Filter by users who joined after this date (inclusive).</param>
	/// <param name="joinDateBefore">Filter by users who joined before this date (exclusive).</param>
	/// <param name="nameMatchMode">Name match mode.</param>
	/// <param name="start">Index of the first entry to be loaded.</param>
	/// <param name="maxResults">Maximum number of results to be loaded.</param>
	/// <param name="getTotalCount">Whether to get total number of results.</param>
	/// <param name="sort">Sort rule.</param>
	/// <param name="includeDisabled">Whether to include disabled user accounts.</param>
	/// <param name="onlyVerified">Whether to only include verified artists.</param>
	/// <param name="knowsLanguage">Filter by known language (optional). This is the ISO 639-1 language code, for example "en" or "zh".</param>
	/// <param name="fields">Optional fields. Possible values are None and MainPicture. Optional.</param>
	/// <returns>Partial result of users.</returns>
	/// <example>http://vocadb.net/api/users?query=Shiro&amp;groups=Trusted</example>
	[HttpGet("")]
	public PartialFindResult<UserForApiContract> GetList(
		string query = "",
		UserGroupId groups = UserGroupId.Nothing,
		DateTime? joinDateAfter = null,
		DateTime? joinDateBefore = null,
		NameMatchMode nameMatchMode = NameMatchMode.Auto,
		int start = 0,
		int maxResults = 10,
		bool getTotalCount = false,
		UserSortRule? sort = null,
		bool includeDisabled = false,
		bool onlyVerified = false,
		string? knowsLanguage = null,
		UserOptionalFields fields = UserOptionalFields.None
	)
	{
		var queryParams = new UserQueryParams
		{
			Common = new CommonSearchParams(SearchTextQuery.Create(query, nameMatchMode), false, false),
			Group = groups,
			IncludeDisabled = includeDisabled,
			OnlyVerifiedArtists = onlyVerified,
			KnowsLanguage = knowsLanguage,
			JoinDateAfter = joinDateAfter,
			JoinDateBefore = joinDateBefore,
			Sort = sort ?? UserSortRule.Name,
			Paging = new PagingProperties(start, maxResults, getTotalCount)
		};

		return _queries.GetUsers(queryParams, user => new UserForApiContract(user, _userIconFactory, fields));
	}
#nullable disable

	/// <summary>
	/// Gets user by ID.
	/// </summary>
	/// <param name="id">User ID.</param>
	/// <param name="fields">Optional fields.</param>
	/// <returns>User properties.</returns>
	[HttpGet("{id:int}")]
	public UserForApiContract GetOne(int id, UserOptionalFields fields = UserOptionalFields.None) =>
		_queries.GetOne(id, fields);

	/// <summary>
	/// Gets a user message.
	/// </summary>
	/// <param name="messageId">ID of the message.</param>
	/// <returns>Message details.</returns>
	/// <remarks>
	/// The message will be marked as read.
	/// User can only load messages from their own inbox.
	/// </remarks>
	[HttpGet("messages/{messageId:int}")]
	[Authorize]
	[CacheOutput(ClientTimeSpan = Model.Domain.Constants.SecondsInADay)]
	public UserMessageContract GetMessage(int messageId) =>
		_messageQueries.Get(messageId, _userIconFactory);

	/// <summary>
	/// Gets a list of messages.
	/// </summary>
	/// <param name="id">User ID. Must be the currently logged in user (loading messages for another user is not allowed).</param>
	/// <param name="inbox">Type of inbox. Possible values are Nothing (load all, default), Received, Sent, Notifications.</param>
	/// <param name="unread">Whether to only load unread messages. Loading unread messages is only possible for received messages and notifications (not sent messages).</param>
	/// <param name="anotherUserId">Filter by id of the other user (either sender or receiver).</param>
	/// <param name="start">Index of the first entry to be loaded.</param>
	/// <param name="maxResults">Maximum number of results to be loaded.</param>
	/// <param name="getTotalCount">Whether to get total number of results.</param>
	/// <returns>List of messages.</returns>
	[HttpGet("{id:int}/messages")]
	[Authorize]
	public ActionResult<PartialFindResult<UserMessageContract>> GetMessages(
		int id,
		UserInboxType? inbox = null,
		bool unread = false,
		int? anotherUserId = null,
		int start = 0,
		int maxResults = 10,
		bool getTotalCount = false
	)
	{
		if (id != _permissionContext.LoggedUserId)
			return Forbid();

		return _messageQueries.GetList(_permissionContext.LoggedUserId, new PagingProperties(start, maxResults, getTotalCount),
			inbox ?? UserInboxType.Nothing, unread, anotherUserId, _userIconFactory);
	}

	/// <summary>
	/// Deletes a list of user messages.
	/// </summary>
	/// <param name="id">ID of the user whose messages to delete.</param>
	/// <param name="messageId">IDs of messages.</param>
	[HttpDelete("{id:int}/messages")]
	[Authorize]
	public IActionResult DeleteMessages(int id, [FromQuery] int[] messageId)
	{
		if (id != _permissionContext.LoggedUserId)
			return Forbid();

		_messageQueries.Delete(messageId);
		return NoContent();
	}

	/// <summary>
	/// Gets a list of user names. Ideal for autocomplete boxes.
	/// </summary>
	/// <param name="query">Text query.</param>
	/// <param name="nameMatchMode">Name match mode. Words is treated the same as Partial.</param>
	/// <param name="maxResults">Maximum number of results.</param>
	/// <param name="includeDisabled">Whether to include disabled user accounts. If false, disabled accounts are excluded.</param>
	/// <returns>List of user names.</returns>
	[HttpGet("names")]
	public IEnumerable<string> GetNames(string query = "", NameMatchMode nameMatchMode = NameMatchMode.Auto, int maxResults = 10, bool includeDisabled = false) =>
		_queries.FindNames(SearchTextQuery.Create(query, nameMatchMode), maxResults, includeDisabled);

	/// <summary>
	/// Gets a list of comments posted on user's profile.
	/// </summary>
	/// <param name="id">ID of the user whose comments are to be retrieved.</param>
	/// <param name="start">Index of the first comment to be loaded.</param>
	/// <param name="maxResults">Maximum number of comments to load.</param>
	/// <param name="getTotalCount">Whether to load the total number of comments.</param>
	/// <returns>Page of comments.</returns>
	[HttpGet("{id:int}/profileComments")]
	public PartialFindResult<CommentForApiContract> GetProfileComments(
		int id,
		int start = 0, int maxResults = DefaultMax, bool getTotalCount = false
	)
	{
		var paging = new PagingProperties(start, maxResults, getTotalCount);
		var result = _queries.GetProfileComments(id, paging);

		return result;
	}

#nullable enable
	/// <summary>
	/// Gets a list of songs rated by a user.
	/// </summary>
	/// <param name="id">ID of the user whose songs are to be browsed.</param>
	/// <param name="query">Song name query (optional).</param>
	/// <param name="tagId">Filter by tag Id (optional). This filter can be specified multiple times.</param>
	/// <param name="tagName">Filter by tag name (optional).</param>
	/// <param name="artistId">Filter by song artist (optional).</param>
	/// <param name="childVoicebanks">Include child voicebanks, if the artist being filtered by has any.</param>
	/// <param name="artistGrouping">Logical grouping for artists.</param>
	/// <param name="rating">Filter songs by given rating (optional).</param>
	/// <param name="songListId">Filter songs by song list (optional).</param>
	/// <param name="groupByRating">Group results by rating so that highest rated are first.</param>
	/// <param name="pvServices">Filter by one or more PV services (separated by commas). The song will pass the filter if it has a PV for any of the matched services.</param>
	/// <param name="advancedFilters">List of advanced filters (optional).</param>
	/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
	/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
	/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
	/// <param name="sort">Sort rule (optional, defaults to Name). Possible values are None, Name, AdditionDate, FavoritedTimes, RatingScore.</param>
	/// <param name="nameMatchMode">Match mode for song name (optional, defaults to Auto).</param>
	/// <param name="fields">
	/// List of optional fields (optional). Possible values are Albums, Artists, Names, PVs, Tags, ThumbUrl, WebLinks.
	/// </param>
	/// <param name="lang">Content language preference (optional).</param>
	/// <returns>Page of songs with ratings.</returns>
	[HttpGet("{id:int}/ratedSongs")]
	public PartialFindResult<RatedSongForUserForApiContract> GetRatedSongs(
		int id,
		string query = "",
		string? tagName = null,
		[FromQuery(Name = "tagId[]")] int[]? tagId = null,
		[FromQuery(Name = "artistId[]")] int[]? artistId = null,
		bool childVoicebanks = false,
		LogicalGrouping artistGrouping = LogicalGrouping.And,
		SongVoteRating? rating = null,
		int? songListId = null,
		bool groupByRating = true,
		PVServices? pvServices = null,
		[FromQuery(Name = "advancedFilters")] AdvancedSearchFilterParams[]? advancedFilters = null,
		int start = 0, int maxResults = DefaultMax, bool getTotalCount = false,
		RatedSongForUserSortRule? sort = null,
		NameMatchMode nameMatchMode = NameMatchMode.Auto,
		SongOptionalFields fields = SongOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	)
	{
		maxResults = Math.Min(maxResults, AbsoluteMax);
		var textQuery = SearchTextQuery.Create(query, nameMatchMode);

		var queryParams = new RatedSongQueryParams(id, new PagingProperties(start, maxResults, getTotalCount))
		{
			TextQuery = textQuery,
			SortRule = sort ?? RatedSongForUserSortRule.Name,
			ArtistIds = artistId,
			ArtistGrouping = artistGrouping,
			ChildVoicebanks = childVoicebanks,
			FilterByRating = rating ?? SongVoteRating.Nothing,
			GroupByRating = groupByRating,
			PVServices = pvServices,
			SonglistId = songListId ?? 0,
			TagIds = tagId,
			TagName = tagName,
			AdvancedFilters = advancedFilters?.Select(advancedFilter => advancedFilter.ToAdvancedSearchFilter()).ToArray(),
		};

		var songs = _queries.GetRatedSongs(queryParams, ratedSong => new RatedSongForUserForApiContract(ratedSong, lang, _permissionContext, fields));
		return songs;
	}

	/// <summary>
	/// Gets a list of song lists for a user.
	/// </summary>
	/// <param name="id">User whose song lists are to be loaded.</param>
	/// <param name="query">Song list name query (optional).</param>
	/// <param name="tagId">Filter by one or more tag Ids (optional).</param>
	/// <param name="childTags">Include child tags, if the tags being filtered by have any.</p
	/// <param name="nameMatchMode">Match mode for song name (optional, defaults to Auto).</param>
	/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
	/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
	/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
	/// <param name="sort">Sort option for the song lists. Possible values are None, Name, Date, CreateDate. Default is Name.</param>
	/// <param name="fields">List of optional fields.</param>
	/// <returns>List of song lists.</returns>
	[HttpGet("{id:int}/songLists")]
	public PartialFindResult<SongListForApiContract> GetSongLists(
		int id,
		string query = "",
		[FromQuery(Name = "tagId[]")] int[]? tagId = null,
		bool childTags = false,
		NameMatchMode nameMatchMode = NameMatchMode.Auto,
		int start = 0, int maxResults = DefaultMax, bool getTotalCount = false,
		SongListSortRule sort = SongListSortRule.Name,
		SongListOptionalFields? fields = null
	)
	{
		var textQuery = SearchTextQuery.Create(query, nameMatchMode);
		var queryParams = new SongListQueryParams
		{
			TextQuery = textQuery,
			SortRule = sort,
			Paging = new PagingProperties(start, maxResults, getTotalCount),
			TagIds = tagId,
			ChildTags = childTags
		};

		return _queries.GetCustomSongLists(id, queryParams, fields ?? SongListOptionalFields.None);
	}
#nullable disable

	/// <summary>
	/// Gets a specific user's rating for a song.
	/// </summary>
	/// <param name="id">User whose rating is to be checked.</param>
	/// <param name="songId">ID of the song whose rating is to be checked.</param>
	/// <returns>Specified user's rating for the specified song. If the song is not rated by the user, this will be Nothing.</returns>
	[HttpGet("{id:int}/ratedSongs/{songId:int}")]
	public SongVoteRating GetSongRating(int id, int songId) =>
		_queries.GetSongRating(id, songId);

	/// <summary>
	/// Gets currently logged in user's rating for a song.
	/// </summary>
	/// <param name="songId">ID of the song whose rating is to be checked.</param>
	/// <returns>Currently logged in user's rating for the specified song. If the song is not rated by the current user, this will be Nothing.</returns>
	/// <remarks>
	/// Requires authentication.
	/// </remarks>
	[HttpGet("current/ratedSongs/{songId:int}")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	public SongVoteRating GetSongRatingForCurrent(int songId) =>
		GetSongRating(_permissionContext.LoggedUserId, songId);

	/// <summary>
	/// Gets tags for a specific album and information whether the logged in user has voted on those tags.
	/// </summary>
	/// <param name="albumId">Album Id.</param>
	/// <returns>List of tags with selections by the current user.</returns>
	[HttpGet("current/albumTags/{albumId:int}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public TagSelectionContract[] GetAlbumTags(int albumId) =>
		_queries.GetAlbumTagSelections(albumId, _permissionContext.LoggedUserId);

	/// <summary>
	/// Gets tags for a specific artist and information whether the logged in user has voted on those tags.
	/// </summary>
	/// <param name="artistId">Artist Id.</param>
	/// <returns>List of tags with selections by the current user.</returns>
	[HttpGet("current/artistTags/{artistId:int}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public TagSelectionContract[] GetArtistTags(int artistId) =>
		_queries.GetArtistTagSelections(artistId, _permissionContext.LoggedUserId);

	[HttpGet("current/eventTags/{eventId:int}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public TagSelectionContract[] GetEventTags(int eventId) =>
		_queries.GetEventTagSelections(eventId, _permissionContext.LoggedUserId);

	[HttpGet("current/eventSeriesTags/{seriesId:int}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public TagSelectionContract[] GetEventSeriesTags(int seriesId) =>
		_queries.GetEventSeriesTagSelections(seriesId, _permissionContext.LoggedUserId);

	[HttpGet("current/songListTags/{songListId:int}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public TagSelectionContract[] GetSongListTags(int songListId) =>
		_queries.GetSongListTagSelections(songListId, _permissionContext.LoggedUserId);

	[HttpGet("{id:int}/songs-per-genre")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[CacheOutput(ClientTimeSpan = Model.Domain.Constants.SecondsInADay)]
	public Tuple<string, int>[] GetSongsPerGenre(int id) =>
		_queries.GetRatingsByGenre(id);

	/// <summary>
	/// Gets tags for a specific song and information whether the logged in user has voted on those tags.
	/// </summary>
	/// <param name="songId">Song Id.</param>
	/// <returns>List of tags with selections by the current user.</returns>
	[HttpGet("current/songTags/{songId:int}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public TagSelectionContract[] GetSongTags(int songId) =>
		_queries.GetSongTagSelections(songId, _permissionContext.LoggedUserId);

	[HttpDelete("current/songTags/{tagUsageId:long}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult RemoveSongTagUsage(long tagUsageId) {
		_queries.DeleteSongTag(tagUsageId);
		return NoContent();
	}

	[HttpDelete("current/artistTags/{tagUsageId:long}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult RemoveArtistTagUsage(long tagUsageId) {
		_queries.DeleteArtistTag(tagUsageId);
		return NoContent();
	}
	
	[HttpDelete("current/albumTags/{tagUsageId:long}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult RemoveAlbumTagUsage(long tagUsageId) {
		_queries.DeleteAlbumTag(tagUsageId);
		return NoContent();
	}
	
	[HttpDelete("current/eventTags/{tagUsageId:long}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult RemoveEventTagUsage(long tagUsageId) {
		_queries.DeleteEventTag(tagUsageId);
		return NoContent();
	}
	/// <summary>
	/// Add or update collection status, media type and rating for a specific album, for the currently logged in user.
	/// </summary>
	/// <param name="albumId">ID of the album to be rated.</param>
	/// <param name="collectionStatus">Collection status. Possible values are Nothing, Wishlisted, Ordered and Owned.</param>
	/// <param name="mediaType">Album media type. Possible values are PhysicalDisc, DigitalDownload and Other.</param>
	/// <param name="rating">Rating to be given. Possible values are between 0 and 5.</param>
	/// <returns>The string "OK" if successful.</returns>
	/// <remarks>
	/// If the user has already rated the album, any previous rating is replaced.
	/// Authorization cookie must be included.
	/// </remarks>
	[HttpPost("current/albums/{albumId:int}")]
	[Authorize]
	public string PostAlbumStatus(int albumId, PurchaseStatus collectionStatus, MediaType mediaType, int rating)
	{
		_queries.UpdateAlbumForUser(_permissionContext.LoggedUserId, albumId, collectionStatus, mediaType, rating);
		return "OK";
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
	[HttpPost("profileComments/{commentId:int}")]
	[Authorize]
	public void PostEditComment(int commentId, CommentForApiContract contract) =>
		_queries.PostEditComment(commentId, contract);

	public class UserEventAssociation
	{
		public UserEventRelationshipType AssociationType { get; set; }
	}

	[Authorize]
	[HttpPost("current/events/{eventId:int}")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public void PostEventAttendance(int eventId, UserEventAssociation association) =>
		_queries.UpdateEventForUser(_permissionContext.LoggedUserId, eventId, association.AssociationType);

	/// <summary>
	/// Creates a new message.
	/// </summary>
	/// <param name="id">User ID. Must be logged in user.</param>
	/// <param name="contract">Message data.</param>
	/// <returns>Message data.</returns>
	[HttpPost("{id:int}/messages")]
	[Authorize]
	public async Task<UserMessageContract> PostNewMessage(int id, UserMessageContract contract)
	{
		var mySettingsUrl = VocaUriBuilder.CreateAbsolute("User/MySettings").ToString();
		var messagesUrl = VocaUriBuilder.CreateAbsolute("User/Messages").ToString();

		return await _queries.SendMessage(contract, mySettingsUrl, messagesUrl);
	}

	/// <summary>
	/// Refresh entry edit status, indicating that the current user is still editing that entry.
	/// </summary>
	/// <param name="entryType">Type of entry.</param>
	/// <param name="entryId">Entry ID.</param>
	[HttpPost("current/refreshEntryEdit")]
	[Authorize]
	public EntryEditDataContract PostRefreshEntryEdit(EntryType entryType, int entryId) =>
		ConcurrentEntryEditManager.CheckConcurrentEdits(new EntryRef(entryType, entryId), _permissionContext.LoggedUser);

	public class CreateReportModel
	{
		public UserReportType ReportType { get; set; }
		public string Reason { get; set; }
	}

	[Authorize]
	[HttpPost("{id:int}/reports")]
	[ValidateAntiForgeryToken]
	public bool PostReport(int id, [FromBody] CreateReportModel model) =>
		_queries.CreateReport(id, model.ReportType, WebHelper.GetRealHost(Request), model.Reason).created;

	[HttpPost("current/followedTags/{tagId:int}")]
	[Authorize]
	public void PostFollowedTag(int tagId) =>
		_queries.AddFollowedTag(_permissionContext.LoggedUserId, tagId);

	// This is the standard way of providing value in body. Alternatively, a custom model binder.
	public class PostSongRatingParams
	{
		public SongVoteRating Rating { get; set; }
	}

	/// <summary>
	/// Add or update rating for a specific song, for the currently logged in user.
	/// </summary>
	/// <param name="songId">ID of the song to be rated.</param>
	/// <param name="rating">Rating to be given. Possible values are Nothing, Like, Favorite.</param>
	/// <remarks>
	/// If the user has already rated the song, any previous rating is replaced.
	/// Authorization cookie must be included.
	/// This API supports CORS.
	/// </remarks>
	[HttpPost("current/ratedSongs/{songId:int}")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ApiExplorerSettings(IgnoreApi = true)]
	[Obsolete]
	public string PostSongRating(int songId, SongVoteRating rating)
	{
		_service.UpdateSongRating(_permissionContext.LoggedUserId, songId, rating);
		return "OK";
	}

#nullable enable
	/// <summary>
	/// Updates tag selections for an album by the logged in user.
	/// </summary>
	/// <param name="albumId">Album Id.</param>
	/// <param name="tags">List of names of tags that the user has selected.</param>
	/// <returns>List of tag usages with information on how many times a particular tag has been added.</returns>
	[HttpPut("current/albumTags/{albumId:int}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult<TagUsageForApiContract[]>> PutAlbumTags(int albumId, [FromBody] TagBaseContract[]? tags)
	{
		if (tags == null)
			return BadRequest();

		return await _queries.SaveAlbumTags(albumId, tags, onlyAdd: false);
	}

	[HttpPut("current/artistTags/{artistId:int}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult<TagUsageForApiContract[]>> PutArtistTags(int artistId, [FromBody] TagBaseContract[]? tags)
	{
		if (tags == null)
			return BadRequest();

		return await _queries.SaveArtistTags(artistId, tags, onlyAdd: false);
	}

	[HttpPut("current/eventTags/{eventId:int}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult<TagUsageForApiContract[]>> PutEventTags(int eventId, [FromBody] TagBaseContract[]? tags)
	{
		if (tags == null)
			return BadRequest();

		return await _queries.SaveEventTags(eventId, tags, onlyAdd: false);
	}

	[HttpPut("current/eventSeriesTags/{seriesId:int}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult<TagUsageForApiContract[]>> PutEventSeriesTags(int seriesId, [FromBody] TagBaseContract[]? tags)
	{
		if (tags == null)
			return BadRequest();

		return await _queries.SaveEventSeriesTags(seriesId, tags, onlyAdd: false);
	}

	[HttpPut("current/songListTags/{songListId:int}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult<TagUsageForApiContract[]>> PutSongListTags(int songListId, [FromBody] TagBaseContract[]? tags)
	{
		if (tags == null)
			return BadRequest();

		return await _queries.SaveSongListTags(songListId, tags, onlyAdd: false);
	}

	/// <summary>
	/// Appends tags for a song, by the currently logged in user.
	/// </summary>
	/// <param name="songId">ID of the song to be tagged.</param>
	/// <param name="tags">List of tags to be appended.</param>
	/// <remarks>
	/// This can only be used to add tags - existing tags will not be removed. 
	/// Nothing will be done for tags that are already applied by the current user for the song.
	/// Authorization cookie is required.
	/// </remarks>
	[HttpPost("current/songTags/{songId:int}")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	public async Task<IActionResult> PostSongTags(int songId, TagBaseContract[]? tags)
	{
		if (tags == null)
			return BadRequest();

		await _queries.SaveSongTags(songId, tags, onlyAdd: true);
		return NoContent();
	}

	[HttpPut("current/songTags/{songId:int}")]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult<TagUsageForApiContract[]>> PutSongTags(int songId, [FromBody] TagBaseContract[]? tags)
	{
		if (tags == null)
			return BadRequest();

		return await _queries.SaveSongTags(songId, tags, onlyAdd: false);
	}
#nullable disable

	/// <summary>
	/// Posts a new comment.
	/// </summary>
	/// <param name="id">ID of the user for whom to create the comment.</param>
	/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
	/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
	[HttpPost("{id:int}/profileComments")]
	[Authorize]
	public CommentForApiContract PostNewComment(int id, CommentForApiContract contract) =>
		_queries.CreateComment(id, contract.Message);

	/// <summary>
	/// Updates user setting.
	/// </summary>
	/// <param name="id">ID of the user to be updated. This must match the current user OR be unspecified (or 0) if the user is not logged in.</param>
	/// <param name="settingName">Name of the setting to be updated, for example 'showChatBox'.</param>
	/// <param name="settingValue">Setting value, for example 'false'.</param>
	[HttpPost("{id:int}/settings/{settingName}")]
	public IActionResult PostSetting(int id, string settingName, [FromBody] string settingValue)
	{
		if (id != 0 && id != _permissionContext.LoggedUserId)
			return Forbid();

		IUserSetting setting = null;

		switch (settingName.ToLowerInvariant())
		{
			case "languagepreference":
				setting = _permissionContext.LanguagePreferenceSetting;
				break;
		}

		if (setting == null)
			return BadRequest();

		setting.ParseFromValue(settingValue);

		if (_permissionContext.IsLoggedIn)
			_queries.UpdateUserSetting(setting);

		return NoContent();
	}

	public class PostStatusLimitedModel
	{
		public bool CreateReport { get; set; }
		public string Reason { get; set; }
	}

	[Authorize]
	[HttpPost("{id:int}/status-limited")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[ValidateAntiForgeryToken]
	public void PostStatusLimited(int id, [FromBody] PostStatusLimitedModel model) =>
		_queries.SetUserToLimited(id, model.Reason, WebHelper.GetRealHost(Request), model.CreateReport);

	[HttpGet("{id:int}/followedArtists/{artistId:int}")]
	public ArtistForUserForApiContract GetArtistForUser(int id, int artistId) =>
		_queries.GetArtistForUser(id, artistId);

	[HttpGet("current/followedArtists/{artistId:int}")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	public ArtistForUserForApiContract GetArtistForUser(int artistId) =>
		GetArtistForUser(_permissionContext.LoggedUserId, artistId);

	[HttpGet("{id:int}/album-collection-statuses/{albumId:int}")]
	public AlbumForUserForApiContract GetAlbumForUser(int id, int albumId) =>
		_queries.GetAlbumForUser(id, albumId);

	[HttpGet("current/album-collection-statuses/{albumId:int}")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	public AlbumForUserForApiContract GetAlbumForUser(int albumId) =>
		GetAlbumForUser(_permissionContext.LoggedUserId, albumId);

#nullable enable
	[HttpGet("~/api/profiles/{name}")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult<UserDetailsForApiContract> GetDetails(string name/* TODO: , int? artistId = null, bool? childVoicebanks = null */)
	{
		var user = _queries.GetUserDetailsForApi(name);
		return user is not null ? user : NotFound();
	}

	[HttpGet("current/for-my-settings")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ServerOnlyUserForMySettingsForApiContract GetUserForMySettings()
	{
		return _queries.GetUserForMySettings();
	}

	[HttpPost("current/my-settings")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ValidateAntiForgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult<string>> UpdateMySettings(
		[ModelBinder(BinderType = typeof(JsonModelBinder))] ServerOnlyUpdateUserSettingsForApiContract contract,
		IFormFile? pictureUpload = null
	)
	{
		var user = _permissionContext.LoggedUser;

		if (user is null || user.Id != contract.Id)
			return Forbid();

		if (!ModelState.IsValid)
			return ValidationProblem(ModelState);

		try
		{
			var pictureData = ControllerBase.ParsePicture(this, pictureUpload, "pictureUpload", ImagePurpose.Main);

			var newUser = await _queries.UpdateUserSettings(contract, pictureData);
			_loginManager.SetLoggedUser(newUser);
			_permissionContext.LanguagePreferenceSetting.Value = contract.DefaultLanguageSelection;

			// Updating username currently requires signing in again
			if (newUser.Name != user.Name)
			{
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			}

			return newUser.Name;
		}
		catch (InvalidPasswordException x)
		{
			ModelState.AddModelError("OldPass", x.Message);
			return ValidationProblem(ModelState);
		}
		catch (UserEmailAlreadyExistsException)
		{
			ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.EmailTaken);
			return ValidationProblem(ModelState);
		}
		catch (InvalidEmailFormatException)
		{
			ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.InvalidEmail);
			return ValidationProblem(ModelState);
		}
		catch (InvalidUserNameException)
		{
			ModelState.AddModelError("Username", "Username is invalid. Username may contain alphanumeric characters and underscores.");
			return ValidationProblem(ModelState);
		}
		catch (UserNameAlreadyExistsException)
		{
			ModelState.AddModelError("Username", "Username is already in use.");
			return ValidationProblem(ModelState);
		}
		catch (UserNameTooSoonException)
		{
			ModelState.AddModelError("Username", "Username may only be changed once per year. If necessary, contact a staff member.");
			return ValidationProblem(ModelState);
		}
		catch (ValidationException x)
		{
			foreach (var error in x.Errors)
				ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
			return ValidationProblem(ModelState);
		}
	}

	[HttpGet("{id:int}/for-edit")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ServerOnlyUserWithPermissionsForApiContract GetForEdit(int id) => _queries.GetUserWithPermissions(id);

	// TODO: Move.
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

	[HttpPost("register")]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ValidateAntiForgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult> Create(RegisterModel model)
	{
		const string restrictedErr = "Sorry, access from your host is restricted. It is possible this restriction is no longer valid. If you think this is the case, please contact support.";

		var hostname = WebHelper.GetRealHost(Request);

		if (ModelState[nameof(model.Extra)]?.Errors.Any() == true)
		{
			s_log.Warn("An attempt was made to fill the bot decoy field from {0} with the value '{1}'.", hostname, ModelState[nameof(model.Extra)]);
			_ipRuleManager.AddTempBannedIP(hostname, "Attempt to fill the bot decoy field");
			return ValidationProblem(ModelState);
		}

		if (_config.SiteSettings.SignupsDisabled)
		{
			ModelState.AddModelError(string.Empty, "Signups are disabled");
		}

		if (!string.IsNullOrEmpty(AppConfig.ReCAPTCHAKey))
		{
			var recaptchaResult = await ReCaptcha2.ValidateAsync(
				userResponse: model.ReCAPTCHAResponse,
				userIp: new AspNetCoreHttpRequest(Request).UserHostAddress,
				privateKey: AppConfig.ReCAPTCHAKey
			);
			if (!recaptchaResult.Success)
			{
				ErrorLogger.LogMessage(Request, $"Invalid CAPTCHA (error {recaptchaResult.Error})", LogLevel.Warn);
				_otherService.AuditLog("failed CAPTCHA", hostname, AuditLogCategory.UserCreateFailCaptcha);
				ModelState.AddModelError("CAPTCHA", ViewRes.User.CreateStrings.CaptchaInvalid);
			}
		}

		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		if (!_ipRuleManager.IsAllowed(hostname))
		{
			s_log.Warn("Restricting blocked IP {0}.", hostname);
			ModelState.AddModelError("Restricted", restrictedErr);
			return ValidationProblem(ModelState);
		}

		var time = TimeSpan.FromTicks(DateTime.Now.Ticks - model.EntryTime);

		// Attempt to register the user
		try
		{
			var verifyEmailUrl = VocaUriBuilder.CreateAbsolute(Url.Action("VerifyEmail", "User")).ToString();
			var user = await _queries.Create(
				model.UserName,
				model.Password,
				model.Email ?? string.Empty,
				hostname,
				Request.Headers[HeaderNames.UserAgent],
				WebHelper.GetInterfaceCultureName(Request),
				time,
				_ipRuleManager,
				verifyEmailUrl
			);
			await UserController.SetAuthCookieAsync(HttpContext, user.Name, createPersistentCookie: false);
			return NoContent();
		}
		catch (UserNameAlreadyExistsException)
		{
			ModelState.AddModelError("UserName", ViewRes.User.CreateStrings.UsernameTaken);
			return ValidationProblem(ModelState);
		}
		catch (UserEmailAlreadyExistsException)
		{
			ModelState.AddModelError("Email", ViewRes.User.CreateStrings.EmailTaken);
			return ValidationProblem(ModelState);
		}
		catch (InvalidEmailFormatException)
		{
			ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.InvalidEmail);
			return ValidationProblem(ModelState);
		}
		catch (TooFastRegistrationException)
		{
			ModelState.AddModelError("Restricted", restrictedErr);
			return ValidationProblem(ModelState);
		}
		catch (RestrictedIPException)
		{
			ModelState.AddModelError("Restricted", restrictedErr);
			return ValidationProblem(ModelState);
		}
	}

	[HttpPost("{id:int}")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ValidateAntiForgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult<int> Edit(UserForEditForApiContract contract)
	{
		try
		{
			_queries.UpdateUser(contract);
		}
		catch (InvalidEmailFormatException)
		{
			ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.InvalidEmail);
			return ValidationProblem(ModelState);
		}
		catch (UserNameAlreadyExistsException)
		{
			ModelState.AddModelError("Username", "Username is already in use.");
			return ValidationProblem(ModelState);
		}

		return contract.Id;
	}

	[HttpPost("login")]
	[RestrictBannedIP]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ValidateAntiForgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult> Login(LoginModel model)
	{
		if (ModelState.IsValid)
		{
			var host = WebHelper.GetRealHost(Request);
			var culture = WebHelper.GetInterfaceCultureName(Request);
			var result = _queries.CheckAuthentication(model.UserName, model.Password, host, culture, delayFailedLogin: true);

			if (!result.IsOk)
			{
				ModelState.AddModelError("", ViewRes.User.LoginStrings.WrongPassword);

				if (result.Error == LoginError.AccountPoisoned)
					_ipRuleManager.AddTempBannedIP(host, "Account poisoned");
			}
			else
			{
				await UserController.SetAuthCookieAsync(HttpContext, result.User.Name, model.KeepLoggedIn);
				return NoContent();
			}
		}

		return ValidationProblem(ModelState);
	}

	[HttpPost("logout")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ValidateAntiForgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult> Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return NoContent();
	}

	[HttpPost("forgot-password")]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ValidateAntiForgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult> ForgotPassword(ForgotPassword model)
	{
		if (!string.IsNullOrEmpty(AppConfig.ReCAPTCHAKey))
		{
			var captchaResult = await ReCaptcha2.ValidateAsync(
				userResponse: model.ReCAPTCHAResponse,
				userIp: new AspNetCoreHttpRequest(Request).UserHostAddress,
				privateKey: AppConfig.ReCAPTCHAKey
			);
			if (!captchaResult.Success)
				ModelState.AddModelError("CAPTCHA", ViewRes.User.ForgotPasswordStrings.CaptchaIsInvalid);
		}

		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		try
		{
			await _queries.RequestPasswordReset(model.Username, model.Email, AppConfig.HostAddress + Url.Action("ResetPassword", "User"));
			return NoContent();
		}
		catch (UserNotFoundException)
		{
			s_log.Info($"User not found: {model.Username.Replace(Environment.NewLine, "")}");
			return BadRequest();
		}
	}
#nullable disable
}
