using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.User;
using VocaDb.Model.Domain.Artists;
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
		private readonly IUserPermissionContext permissionContext;
		private readonly UserQueries queries;
		private readonly UserService service;
		private readonly IEntryThumbPersister thumbPersister;

		public UserApiController(UserQueries queries, UserService service, IUserPermissionContext permissionContext, IEntryThumbPersister thumbPersister) {
			this.queries = queries;
			this.service = service;
			this.permissionContext = permissionContext;
			this.thumbPersister = thumbPersister;
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
		/// <param name="userId">ID of the user whose followed artists are to be browsed.</param>
		/// <param name="artistType">Filter by artist type.</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="fields">List of optional fields (optional). Possible values are Description, Groups, Members, Names, Tags, WebLinks.</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of artists.</returns>
		[Route("{userId:int}/followedArtists")]
		public PartialFindResult<ArtistForUserForApiContract> GetFollowedArtists(
			int userId,
			ArtistType artistType = ArtistType.Unknown,
			int start = 0, 
			int maxResults = defaultMax,
			bool getTotalCount = false, 
			ArtistOptionalFields fields = ArtistOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			maxResults = Math.Min(maxResults, absoluteMax);
			var ssl = WebHelper.IsSSL(Request);

			var queryParams = new FollowedArtistQueryParams {
				UserId = userId,
				ArtistType = artistType,
				Paging = new PagingProperties(start, maxResults, getTotalCount),
			};

			var artists = queries.GetArtists(queryParams, afu => 
				new ArtistForUserForApiContract(afu, lang, thumbPersister, ssl, fields));

			return artists;

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
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			SongSortRule? sort = null,
			NameMatchMode nameMatchMode = NameMatchMode.Auto, 
			SongOptionalFields fields = SongOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			maxResults = Math.Min(maxResults, absoluteMax);
			var textQuery = SearchTextQuery.Create(query, nameMatchMode);

			var queryParams = new RatedSongQueryParams(userId, new PagingProperties(start, maxResults, getTotalCount)) {
				TextQuery = textQuery,
				SortRule = sort ?? SongSortRule.Name,
				ArtistId = artistId ?? 0,
				ChildVoicebanks = childVoicebanks,
				FilterByRating = rating ?? SongVoteRating.Nothing,
				GroupByRating = groupByRating,
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
		/// <returns>List of song lists.</returns>
		[Route("{userId:int}/songLists")]
		public SongListBaseContract[] GetSongLists(int userId) {
			
			return queries.GetCustomSongLists(userId);

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
			
			return queries.GetSongRating(permissionContext.LoggedUserId, songId);

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

			var tags = tag.Where(t => !string.IsNullOrEmpty(t) && Tag.IsValidTagName(t)).ToArray();

			if (!tags.Any())
				return;

			queries.AddSongTags(songId, tags);

		}

	}
}