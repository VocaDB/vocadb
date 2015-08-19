using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.SongLists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.User;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for users.
	/// </summary>
	[RoutePrefix("api/users")]
	public class UserApiController : ApiController {

		private const int absoluteMax = 50;
		private const int defaultMax = 10;
		private readonly UserMessageQueries messageQueries;
		private readonly IUserPermissionContext permissionContext;
		private readonly UserQueries queries;
		private readonly UserService service;
		private readonly IEntryThumbPersister thumbPersister;
		private readonly IUserIconFactory userIconFactory;

		public UserApiController(UserQueries queries, UserMessageQueries messageQueries, UserService service, IUserPermissionContext permissionContext, IEntryThumbPersister thumbPersister,
			IUserIconFactory userIconFactory) {
			this.queries = queries;
			this.messageQueries = messageQueries;
			this.service = service;
			this.permissionContext = permissionContext;
			this.thumbPersister = thumbPersister;
			this.userIconFactory = userIconFactory;
		}

		/// <summary>
		/// Deletes a comment.
		/// Normal users can delete their own comments, moderators can delete all comments.
		/// Requires login.
		/// </summary>
		/// <param name="commentId">ID of the comment to be deleted.</param>
		[Route("profileComments/{commentId:int}")]
		[Authorize]
		public void DeleteProfileComment(int commentId) {
			
			service.DeleteComment(commentId);

		}

		/// <summary>
		/// Gets a list of albums in a user's collection.
		/// This includes albums that have been rated by the user as well as albums that the user has bought or wishlisted.
		/// Note that the user might have set his album ownership status and media type as private, in which case those properties are not included.
		/// </summary>
		/// <param name="userId">ID of the user whose albums are to be browsed.</param>
		/// <param name="query">Album name query (optional).</param>
		/// <param name="tag">Filter by tag (optional).</param>
		/// <param name="artistId">Filter by album artist (optional).</param>
		/// <param name="purchaseStatuses">
		/// Filter by a comma-separated list of purchase statuses (optional). Possible values are Nothing, Wishlisted, Ordered, Owned, and all combinations of these.
		/// </param>
		/// <param name="releaseEventName">Filter by release event name, for example "The Voc@loid M@ster 24". Must be an exact match. Optional.</param>
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
		[Route("{userId:int}/albums")]
		public PartialFindResult<AlbumForUserForApiContract> GetAlbumCollection(
			int userId,
			string query = "", 
			string tag = null,
			int? artistId = null,
			[FromUri] PurchaseStatuses? purchaseStatuses = null,
			string releaseEventName = null,
			int start = 0, 
			int maxResults = defaultMax,
			bool getTotalCount = false, 
			AlbumSortRule? sort = null,
			NameMatchMode nameMatchMode = NameMatchMode.Exact, 
			AlbumOptionalFields fields = AlbumOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
		
			maxResults = Math.Min(maxResults, absoluteMax);
			var textQuery = SearchTextQuery.Create(query, nameMatchMode);
			var ssl = WebHelper.IsSSL(Request);

			var queryParams = new AlbumCollectionQueryParams(userId, new PagingProperties(start, maxResults, getTotalCount)) {
				ArtistId = artistId ?? 0,
				FilterByStatus = purchaseStatuses != null ? purchaseStatuses.Value.ToIndividualSelections().ToArray() : null,
				TextQuery = textQuery,
				ReleaseEventName = releaseEventName,
				Sort = sort ?? AlbumSortRule.Name,
				Tag = tag
			};

			var albums = queries.GetAlbumCollection(queryParams, (afu, shouldShowCollectionStatus) => 
				new AlbumForUserForApiContract(afu, lang, thumbPersister, ssl, fields, shouldShowCollectionStatus));

			return albums;

		}

		/// <summary>
		/// Gets a list of artists followed by a user.
		/// </summary>
		/// <param name="query">Artist name query (optional).</param>
		/// <param name="userId">ID of the user whose followed artists are to be browsed.</param>
		/// <param name="artistType">Filter by artist type.</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">Sort rule (optional, defaults to Name). Possible values are None, Name, AdditionDate, AdditionDateAsc.</param>
		/// <param name="nameMatchMode">Match mode for artist name (optional, defaults to Auto).</param>
		/// <param name="fields">List of optional fields (optional). Possible values are Description, Groups, Members, Names, Tags, WebLinks.</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of artists.</returns>
		[Route("{userId:int}/followedArtists")]
		public PartialFindResult<ArtistForUserForApiContract> GetFollowedArtists(
			int userId,
			string query = "",
			ArtistType artistType = ArtistType.Unknown,
			int start = 0, 
			int maxResults = defaultMax,
			bool getTotalCount = false,
			ArtistSortRule sort = ArtistSortRule.Name,
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			ArtistOptionalFields fields = ArtistOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			maxResults = Math.Min(maxResults, absoluteMax);
			var ssl = WebHelper.IsSSL(Request);
			var textQuery = ArtistSearchTextQuery.Create(query, nameMatchMode);

			var queryParams = new FollowedArtistQueryParams {
				UserId = userId,
				ArtistType = artistType,
				Paging = new PagingProperties(start, maxResults, getTotalCount),
				SortRule = sort,
				TextQuery = textQuery
			};

			var artists = queries.GetArtists(queryParams, afu => 
				new ArtistForUserForApiContract(afu, lang, thumbPersister, ssl, fields));

			return artists;

		}

		/// <summary>
		/// Gets a list of users.
		/// </summary>
		/// <param name="query">User name query (optional).</param>
		/// <param name="groups">Filter by user group. Only one value supported for now. Optional.</param>
		/// <param name="nameMatchMode">Name match mode.</param>
		/// <param name="start">Index of the first entry to be loaded.</param>
		/// <param name="maxResults">Maximum number of results to be loaded.</param>
		/// <param name="getTotalCount">Whether to get total number of results.</param>
		/// <param name="sort">Sort rule.</param>
		/// <param name="includeDisabled">Whether to include disabled user accounts.</param>
		/// <param name="onlyVerified">Whether to only include verified artists.</param>
		/// <param name="fields">Optional fields. Possible values are None and MainPicture. Optional.</param>
		/// <returns>Partial result of users.</returns>
		/// <example>http://vocadb.net/api/users?query=Shiro&amp;groups=Trusted</example>
		[Route("")]
		public PartialFindResult<UserForApiContract> GetList(
			string query = "", 
			UserGroupId groups = UserGroupId.Nothing,
			NameMatchMode nameMatchMode = NameMatchMode.Auto, 
			int start = 0, 
			int maxResults = 10,
			bool getTotalCount = false,
			UserSortRule? sort = null,
			bool includeDisabled = false,
			bool onlyVerified = false,
			UserOptionalFields fields = UserOptionalFields.None) {

			return queries.GetUsers(SearchTextQuery.Create(query, nameMatchMode), groups, includeDisabled, onlyVerified, sort ?? UserSortRule.Name, 
				new PagingProperties(start, maxResults, getTotalCount), user => new UserForApiContract(user, userIconFactory, fields));
			
		}

		/// <summary>
		/// Gets a list of messages.
		/// </summary>
		/// <param name="userId">User ID. Must be the currently logged in user (loading messages for another user is not allowed).</param>
		/// <param name="inbox">Type of inbox. Possible values are Nothing (load all, default), Received, Sent, Notifications.</param>
		/// <param name="unread">Whether to only load unread messages. Loading unread messages is only possible for received messages and notifications (not sent messages).</param>
		/// <param name="start">Index of the first entry to be loaded.</param>
		/// <param name="maxResults">Maximum number of results to be loaded.</param>
		/// <param name="getTotalCount">Whether to get total number of results.</param>
		/// <returns>List of messages.</returns>
		[Route("{userId:int}/messages")]
		[Authorize]
		public PartialFindResult<UserMessageContract> GetMessages(
			int userId, 
			UserInboxType? inbox = null, 
			bool unread = false,
			int start = 0,
			int maxResults = 10,
			bool getTotalCount = false) {

			if (userId != permissionContext.LoggedUserId) {
				throw new HttpResponseException(HttpStatusCode.Forbidden);
			}

			return messageQueries.GetList(permissionContext.LoggedUserId, new PagingProperties(start, maxResults, getTotalCount), 
				inbox ?? UserInboxType.Nothing, unread, userIconFactory);

		}

		/// <summary>
		/// Deletes a list of user messages.
		/// </summary>
		/// <param name="userId">ID of the user whose messages to delete.</param>
		/// <param name="messageId">IDs of messages.</param>
		[Route("{userId:int}/messages")]
		[Authorize]
		public void DeleteMessages(int userId, [FromUri] int[] messageId) {

			if (userId != permissionContext.LoggedUserId) {
				throw new HttpResponseException(HttpStatusCode.Forbidden);
			}

			foreach (var id in messageId)
				messageQueries.Delete(id);

		}

		/// <summary>
		/// Gets a list of user names. Ideal for autocomplete boxes.
		/// </summary>
		/// <param name="query">Text query.</param>
		/// <param name="nameMatchMode">Name match mode. Words is treated the same as Partial.</param>
		/// <param name="maxResults">Maximum number of results.</param>
		/// <returns>List of user names.</returns>
		[Route("names")]
		public IEnumerable<string> GetNames(string query = "", NameMatchMode nameMatchMode = NameMatchMode.Auto, int maxResults = 10) {
			
			return queries.FindNames(SearchTextQuery.Create(query, nameMatchMode), maxResults);

		}

		/// <summary>
		/// Gets a list of comments posted on user's profile.
		/// </summary>
		/// <param name="userId">ID of the user whose comments are to be retrieved.</param>
		/// <param name="start">Index of the first comment to be loaded.</param>
		/// <param name="maxResults">Maximum number of comments to load.</param>
		/// <param name="getTotalCount">Whether to load the total number of comments.</param>
		/// <returns>Page of comments.</returns>
		[Route("{userId:int}/profileComments")]
		public PartialFindResult<CommentForApiContract> GetProfileComments(
			int userId,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false
			) {
			
			var paging = new PagingProperties(start, maxResults, getTotalCount);
			var result = queries.GetProfileComments(userId, paging);

			return result;

		}

		/// <summary>
		/// Gets a list of songs rated by a user.
		/// </summary>
		/// <param name="userId">ID of the user whose songs are to be browsed.</param>
		/// <param name="query">Song name query (optional).</param>
		/// <param name="tag">Filter by tag (optional).</param>
		/// <param name="artistId">Filter by song artist (optional).</param>
		/// <param name="childVoicebanks">Include child voicebanks, if the artist being filtered by has any.</param>
		/// <param name="rating">Filter songs by given rating (optional).</param>
		/// <param name="songListId">Filter songs by song list (optional).</param>
		/// <param name="groupByRating">Group results by rating so that highest rated are first.</param>
		/// <param name="pvServices">Filter by one or more PV services (separated by commas). The song will pass the filter if it has a PV for any of the matched services.</param>
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
		[Route("{userId:int}/ratedSongs")]
		public PartialFindResult<RatedSongForUserForApiContract> GetRatedSongs(
			int userId,
			string query = "", 
			string tag = null,
			int? artistId = null,
			bool childVoicebanks = false,
			SongVoteRating? rating = null,
			int? songListId = null,
			bool groupByRating = true,
			[FromUri] PVServices? pvServices = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			RatedSongForUserSortRule? sort = null,
			NameMatchMode nameMatchMode = NameMatchMode.Auto, 
			SongOptionalFields fields = SongOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			maxResults = Math.Min(maxResults, absoluteMax);
			var textQuery = SearchTextQuery.Create(query, nameMatchMode);

			var queryParams = new RatedSongQueryParams(userId, new PagingProperties(start, maxResults, getTotalCount)) {
				TextQuery = textQuery,
				SortRule = sort ?? RatedSongForUserSortRule.Name,
				ArtistId = artistId ?? 0,
				ChildVoicebanks = childVoicebanks,
				FilterByRating = rating ?? SongVoteRating.Nothing,
				GroupByRating = groupByRating,
				PVServices = pvServices,
				SonglistId = songListId ?? 0,
				Tag = tag
			};

			var songs = queries.GetRatedSongs(queryParams, ratedSong => new RatedSongForUserForApiContract(ratedSong, lang, fields));
			return songs;

		}

		/// <summary>
		/// Gets a list of song lists for a user.
		/// </summary>
		/// <param name="userId">User whose song lists are to be loaded.</param>
		/// <param name="query">Song list name query (optional).</param>
		/// <param name="nameMatchMode">Match mode for song name (optional, defaults to Auto).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">Sort option for the song lists. Possible values are None, Name, Date, CreateDate. Default is Name.</param>
		/// <param name="fields">List of optional fields.</param>
		/// <returns>List of song lists.</returns>
		[Route("{userId:int}/songLists")]
		public PartialFindResult<SongListForApiContract> GetSongLists(
			int userId,
			string query = "",
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			SongListSortRule sort = SongListSortRule.Name,
			SongListOptionalFields? fields = null) {

			var textQuery = SearchTextQuery.Create(query, nameMatchMode);

			return queries.GetCustomSongLists(userId, textQuery, WebHelper.IsSSL(Request), sort, 
				new PagingProperties(start, maxResults, getTotalCount), fields ?? SongListOptionalFields.None);

		}

		/// <summary>
		/// Gets a specific user's rating for a song.
		/// </summary>
		/// <param name="userId">User whose rating is to be checked.</param>
		/// <param name="songId">ID of the song whose rating is to be checked.</param>
		/// <returns>Specified user's rating for the specified song. If the song is not rated by the user, this will be Nothing.</returns>
		[Route("{userId:int}/ratedSongs/{songId:int}")]
		public SongVoteRating GetSongRating(int userId, int songId) {
			
			return queries.GetSongRating(userId, songId);

		}

		/// <summary>
		/// Gets currently logged in user's rating for a song.
		/// Requires authentication.
		/// </summary>
		/// <param name="songId">ID of the song whose rating is to be checked.</param>
		/// <returns>Currently logged in user's rating for the specified song. If the song is not rated by the current user, this will be Nothing.</returns>
		[Route("current/ratedSongs/{songId:int}")]
		[Authorize]
		[EnableCors(origins: "*", headers: "*", methods: "get", SupportsCredentials = true)]
		public SongVoteRating GetSongRating(int songId) {
			
			return GetSongRating(permissionContext.LoggedUserId, songId);

		}

		/// <summary>
		/// Gets tags for a specific album and information whether the logged in user has voted on those tags.
		/// </summary>
		/// <param name="albumId">Album Id.</param>
		/// <returns>List of tags with selections by the current user.</returns>
		[Route("current/albumTags/{albumId:int}")]
		[Authorize]
		[ApiExplorerSettings(IgnoreApi = true)]
		public TagSelectionContract[] GetAlbumTags(int albumId) {
			
			return queries.GetAlbumTagSelections(albumId, permissionContext.LoggedUserId);

		}

		/// <summary>
		/// Gets tags for a specific artist and information whether the logged in user has voted on those tags.
		/// </summary>
		/// <param name="artistId">Artist Id.</param>
		/// <returns>List of tags with selections by the current user.</returns>
		[Route("current/artistTags/{artistId:int}")]
		[Authorize]
		[ApiExplorerSettings(IgnoreApi = true)]
		public TagSelectionContract[] GetArtistTags(int artistId) {
			
			return queries.GetArtistTagSelections(artistId, permissionContext.LoggedUserId);

		}

		/// <summary>
		/// Gets tags for a specific song and information whether the logged in user has voted on those tags.
		/// </summary>
		/// <param name="songId">Song Id.</param>
		/// <returns>List of tags with selections by the current user.</returns>
		[Route("current/songTags/{songId:int}")]
		[Authorize]
		[ApiExplorerSettings(IgnoreApi = true)]
		public TagSelectionContract[] GetSongTags(int songId) {
			
			return queries.GetSongTagSelections(songId, permissionContext.LoggedUserId);

		}

		/// <summary>
		/// Add or update collection status, media type and rating for a specific album, for the currently logged in user.
		/// If the user has already rated the album, any previous rating is replaced.
		/// Authorization cookie must be included.
		/// </summary>
		/// <param name="albumId">ID of the album to be rated.</param>
		/// <param name="collectionStatus">Collection status. Possible values are Nothing, Wishlisted, Ordered and Owned.</param>
		/// <param name="mediaType">Album media type. Possible values are PhysicalDisc, DigitalDownload and Other.</param>
		/// <param name="rating">Rating to be given. Possible values are between 0 and 5.</param>
		/// <returns>The string "OK" if successful.</returns>
		[Route("current/albums/{albumId:int}")]
		[Authorize]
		public string PostAlbumStatus(int albumId, PurchaseStatus collectionStatus, MediaType mediaType, int rating) {
			
			queries.UpdateAlbumForUser(permissionContext.LoggedUserId, albumId, collectionStatus, mediaType, rating);
			return "OK";

		}

		/// <summary>
		/// Updates a comment.
		/// Normal users can edit their own comments, moderators can edit all comments.
		/// Requires login.
		/// </summary>
		/// <param name="commentId">ID of the comment to be edited.</param>
		/// <param name="contract">New comment data. Only message can be edited.</param>
		[Route("profileComments/{commentId:int}")]
		[Authorize]
		public void PostEditComment(int commentId, CommentForApiContract contract) {
			
			queries.HandleTransaction(ctx => queries.Comments(ctx).Update(commentId, contract));

		}

		[Route("current/refreshEntryEdit")]
		[Authorize]
		public void PostRefreshEntryEdit(EntryType entryType, int entryId) {

			ConcurrentEntryEditManager.CheckConcurrentEdits(new EntryRef(entryType, entryId), permissionContext.LoggedUser);

		}

		/// <summary>
		/// Add or update rating for a specific song, for the currently logged in user.
		/// If the user has already rated the song, any previous rating is replaced.
		/// Authorization cookie must be included.
		/// This API supports CORS.
		/// </summary>
		/// <param name="songId">ID of the song to be rated.</param>
		/// <param name="rating">Rating to be given. Possible values are Nothing, Like, Favorite.</param>
		/// <returns>The string "OK" if successful.</returns>
		[Route("current/ratedSongs/{songId:int}")]
		[Authorize]
		[EnableCors(origins: "*", headers: "*", methods: "post", SupportsCredentials = true)]
		public string PostSongRating(int songId, SongVoteRating rating) {
			
			service.UpdateSongRating(permissionContext.LoggedUserId, songId, rating);
			return "OK";

		}

		/// <summary>
		/// Updates tag selections for an album by the logged in user.
		/// </summary>
		/// <param name="albumId">Album Id.</param>
		/// <param name="tags">List of names of tags that the user has selected.</param>
		/// <returns>List of tag usages with information on how many times a particular tag has been added.</returns>
		[Route("current/albumTags/{albumId:int}")]
		[Authorize]
		[ApiExplorerSettings(IgnoreApi = true)]
		public TagUsageForApiContract[] PutAlbumTags(int albumId, [FromUri] string[] tags) {
			
			if (tags == null)
				throw new HttpResponseException(HttpStatusCode.BadRequest);

			return queries.SaveAlbumTags(albumId, tags, false)
				.Select(t => new TagUsageForApiContract { Name = t.TagName, Count = t.Count}).ToArray();

		}

		[Route("current/artistTags/{artistId:int}")]
		[Authorize]
		[ApiExplorerSettings(IgnoreApi = true)]
		public TagUsageForApiContract[] PutArtistTags(int artistId, [FromUri] string[] tags) {
			
			if (tags == null)
				throw new HttpResponseException(HttpStatusCode.BadRequest);

			return queries.SaveArtistTags(artistId, tags, false)
				.Select(t => new TagUsageForApiContract { Name = t.TagName, Count = t.Count}).ToArray();

		}

		/// <summary>
		/// Appends tags for a song, by the currently logged in user.
		/// This can only be used to add tags - existing tags will not be removed. 
		/// Nothing will be done for tags that are already applied by the current user for the song.
		/// Authorization cookie is required.
		/// </summary>
		/// <param name="songId">ID of the song to be tagged.</param>
		/// <param name="tag">List of tags to be appended.</param>
		[Route("current/songTags/{songId:int}")]
		[Authorize]
		[EnableCors(origins: "*", headers: "*", methods: "post", SupportsCredentials = true)]
		public void PostSongTags(int songId, [FromUri] string[] tag) {
			
			if (tag == null)
				throw new HttpResponseException(HttpStatusCode.BadRequest);

			queries.SaveSongTags(songId, tag, true);

		}

		[Route("current/songTags/{songId:int}")]
		[Authorize]
		[ApiExplorerSettings(IgnoreApi = true)]
		public TagUsageForApiContract[] PutSongTags(int songId, [FromUri] string[] tags) {
			
			if (tags == null)
				throw new HttpResponseException(HttpStatusCode.BadRequest);

			return queries.SaveSongTags(songId, tags, false)
				.Select(t => new TagUsageForApiContract { Name = t.TagName, Count = t.Count}).ToArray();

		}

		/// <summary>
		/// Posts a new comment.
		/// </summary>
		/// <param name="userId">ID of the user for whom to create the comment.</param>
		/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
		/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
		[Route("{userId:int}/profileComments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int userId, CommentForApiContract contract) {
			
			return queries.CreateComment(userId, contract.Message);

		}

		/// <summary>
		/// Updates user setting.
		/// </summary>
		/// <param name="userId">ID of the user to be updated. This must match the current user OR be unspecified (or 0) if the user is not logged in.</param>
		/// <param name="settingName">Name of the setting to be updated, for example 'showChatBox'.</param>
		/// <param name="settingValue">Setting value, for example 'false'.</param>
		[Route("{userId:int}/settings/{settingName}")]
		public void PostSetting(int userId, string settingName, [FromBody] string settingValue) {
			
			if (userId != 0 && userId != permissionContext.LoggedUserId)
				throw new HttpResponseException(HttpStatusCode.Unauthorized);

			IUserSetting setting = null;

			switch (settingName.ToLowerInvariant()) {
				case "languagepreference":
					setting = permissionContext.LanguagePreferenceSetting;
					break;
				case "showchatbox":
					setting = permissionContext.ShowChatbox;
					break;
			}

			if (setting == null) {
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			setting.ParseFromValue(settingValue);

			if (permissionContext.IsLoggedIn)
				queries.UpdateUserSetting(setting);

		}

	}
}