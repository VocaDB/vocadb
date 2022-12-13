#nullable disable

using AspNetCore.CacheOutput;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Helpers;
using VocaDb.Model.Resources;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Shared;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// API queries for albums.
	/// </summary>
	[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
	[Route("api/albums")]
	[ApiController]
	public class AlbumApiController : ApiController
	{
		private const int HourInSeconds = 3600;
		private const int AbsoluteMax = 100;
		private const int DefaultMax = 10;
		private readonly IAggregatedEntryImageUrlFactory _thumbPersister;
		private readonly OtherService _otherService;
		private readonly AlbumQueries _queries;
		private readonly AlbumService _service;

		public AlbumApiController(
			AlbumQueries queries,
			AlbumService service,
			OtherService otherService,
			IAggregatedEntryImageUrlFactory thumbPersister
		)
		{
			_queries = queries;
			_service = service;
			_otherService = otherService;
			_thumbPersister = thumbPersister;
		}

		/// <summary>
		/// Deletes an album.
		/// </summary>
		/// <param name="id">ID of the album to be deleted.</param>
		/// <param name="notes">Notes.</param>
		[HttpDelete("{id:int}")]
		[Authorize]
		[ValidateAntiForgeryToken]
		public void Delete(int id, string notes = "") => _service.Delete(id, notes ?? string.Empty);

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
		public void DeleteComment(int commentId) => _queries.DeleteComment(commentId);

		/// <summary>
		/// Gets a list of comments for an album.
		/// </summary>
		/// <param name="id">ID of the album whose comments to load.</param>
		/// <returns>List of comments in no particular order.</returns>
		/// <remarks>
		/// Pagination and sorting might be added later.
		/// </remarks>
		[HttpGet("{id:int}/comments")]
		public IEnumerable<CommentForApiContract> GetComments(int id) => _queries.GetComments(id);

		[HttpGet("{id:int}/for-edit")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public AlbumForEditForApiContract GetForEdit(int id) => _queries.GetForEdit(id);

		/// <summary>
		/// Gets an album by Id.
		/// </summary>
		/// <param name="id">Album Id (required).</param>
		/// <param name="fields">
		/// Optional fields (optional). Possible values are artists, names, pvs, tags, tracks, webLinks.
		/// </param>
		/// <param name="songFields">
		/// Optional fields for tracks, if included (optional).
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <example>https://vocadb.net/api/albums/1</example>
		/// <returns>Album data.</returns>
		[HttpGet("{id:int}")]
		public AlbumForApiContract GetOne(
			int id,
			AlbumOptionalFields fields = AlbumOptionalFields.None,
			SongOptionalFields songFields = SongOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default
		) => _queries.GetAlbumWithMergeRecord(id, (a, m) => new AlbumForApiContract(a, m, lang, _thumbPersister, fields, songFields));

#nullable enable
		/// <summary>
		/// Gets a page of albums.
		/// </summary>
		/// <param name="query">Album name query (optional).</param>
		/// <param name="discTypes">
		/// Disc type. By default nothing. Possible values are Album, Single, EP, SplitAlbum, Compilation, Video, Other. Note: only one type supported for now.
		/// </param>
		/// <param name="tagId">Filter by tag Id (optional). This filter can be specified multiple times.</param>
		/// <param name="tagName">Filter by tag name (optional). This filter can be specified multiple times.</param>
		/// <param name="childTags">Include child tags, if the tags being filtered by have any.</param>
		/// <param name="artistId">Filter by artist Id (optional).</param>
		/// <param name="artistParticipationStatus">
		/// Filter by artist participation status. Only valid if artistId is specified.
		/// Everything (default): Show all albums by that artist (no filter).
		/// OnlyMainAlbums: Show only main albums by that artist.
		/// OnlyCollaborations: Show only collaborations by that artist.
		/// </param>
		/// <param name="childVoicebanks">Include child voicebanks, if the artist being filtered by has any.</param>
		/// <param name="includeMembers">Include members of groups. This applies if <paramref name="artistId"/> is a group.</param>
		/// <param name="barcode">Filter by album barcode (optional).</param>
		/// <param name="releaseDateAfter">Filter by albums whose release date is after this date (inclusive).</param>
		/// <param name="releaseDateBefore">Filter by albums whose release date is before this date (exclusive).</param>
		/// <param name="advancedFilters">List of advanced filters (optional).</param>
		/// <param name="status">Filter by entry status (optional).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">
		/// Sort rule (optional, defaults to Name). 
		/// Possible values are None, Name, ReleaseDate, ReleaseDateWithNulls, AdditionDate, RatingAverage, RatingTotal, NameThenReleaseDate.
		/// </param>
		/// <param name="preferAccurateMatches">
		/// Whether the search should prefer accurate matches. 
		/// If this is true, entries that match by prefix will be moved first, instead of being sorted alphabetically.
		/// Requires a text query. Does not support pagination.
		/// This is mostly useful for autocomplete boxes.
		/// </param>
		/// <param name="deleted">
		/// Whether to search for deleted entries.
		/// If this is true, only deleted entries will be returned.
		/// If this is false (default), deleted entries are not returned.
		/// </param>
		/// <param name="nameMatchMode">Match mode for artist name (optional, defaults to Exact).</param>
		/// <param name="fields">
		/// Optional fields (optional). Possible values are artists, names, pvs, tags, tracks, webLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of albums.</returns>
		/// <example>https://vocadb.net/api/albums?query=Synthesis&amp;discTypes=Album</example>
		[HttpGet("")]
		public PartialFindResult<AlbumForApiContract> GetList(
			string query = "",
			DiscType discTypes = DiscType.Unknown,
			[FromQuery(Name = "tagName[]")] string[]? tagName = null,
			[FromQuery(Name = "tagId[]")] int[]? tagId = null,
			bool childTags = false,
			[FromQuery(Name = "artistId[]")] int[]? artistId = null,
			ArtistAlbumParticipationStatus artistParticipationStatus = ArtistAlbumParticipationStatus.Everything,
			bool childVoicebanks = false,
			bool includeMembers = false,
			string? barcode = null,
			EntryStatus? status = null,
			DateTime? releaseDateAfter = null,
			DateTime? releaseDateBefore = null,
			[FromQuery(Name = "advancedFilters")] AdvancedSearchFilterParams[]? advancedFilters = null,
			int start = 0,
			int maxResults = DefaultMax,
			bool getTotalCount = false,
			AlbumSortRule? sort = null,
			bool preferAccurateMatches = false,
			bool deleted = false,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			AlbumOptionalFields fields = AlbumOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default
		)
		{
			var textQuery = SearchTextQuery.Create(query, nameMatchMode);

			var queryParams = new AlbumQueryParams(textQuery, discTypes, start, Math.Min(maxResults, AbsoluteMax), getTotalCount, sort ?? AlbumSortRule.Name, preferAccurateMatches)
			{
				ArtistParticipation =
				{
					ArtistIds = artistId,
					Participation = artistParticipationStatus,
					ChildVoicebanks = childVoicebanks,
					IncludeMembers = includeMembers
				},
				Tags = tagName,
				TagIds = tagId,
				ChildTags = childTags,
				Barcode = barcode,
				Deleted = deleted,
				ReleaseDateAfter = releaseDateAfter,
				ReleaseDateBefore = releaseDateBefore,
				AdvancedFilters = advancedFilters?.Select(advancedFilter => advancedFilter.ToAdvancedSearchFilter()).ToArray(),
				LanguagePreference = lang,
			};
			queryParams = queryParams with { Common = queryParams.Common with { EntryStatus = status } };

			var entries = _service.Find(a => new AlbumForApiContract(a, null, lang, _thumbPersister, fields, SongOptionalFields.None), queryParams);

			return entries;
		}
#nullable disable

		/// <summary>
		/// Gets a list of album names. Ideal for autocomplete boxes.
		/// </summary>
		/// <param name="query">Text query.</param>
		/// <param name="nameMatchMode">Name match mode.</param>
		/// <param name="maxResults">Maximum number of results.</param>
		/// <returns>List of album names.</returns>
		[HttpGet("names")]
		public string[] GetNames(
			string query = "",
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			int maxResults = 15
		) => _service.FindNames(SearchTextQuery.Create(query, nameMatchMode), maxResults);

		/// <summary>
		/// Gets list of upcoming or recent albums, same as front page.
		/// </summary>
		/// <remarks>
		/// Output is cached for 1 hour.
		/// </remarks>
		[HttpGet("new")]
		[CacheOutput(ClientTimeSpan = HourInSeconds, ServerTimeSpan = HourInSeconds)]
		public IEnumerable<AlbumForApiContract> GetNewAlbums(
			ContentLanguagePreference languagePreference = ContentLanguagePreference.Default,
			AlbumOptionalFields fields = AlbumOptionalFields.None
		) => _otherService.GetRecentAlbums(languagePreference, fields);

		[HttpGet("{id:int}/reviews")]
		public Task<IEnumerable<AlbumReviewContract>> GetReviews(int id, string languageCode = null) => _queries.GetReviews(id, languageCode);

		[HttpGet("{id:int}/user-collections")]
		public Task<IEnumerable<AlbumForUserForApiContract>> GetUserCollections(int id, ContentLanguagePreference languagePreference = ContentLanguagePreference.Default) => _queries.GetUserCollections(id, languagePreference);

		[Authorize]
		[HttpPost("{id:int}/reviews")]
		public AlbumReviewContract PostReview(int id, AlbumReviewContract reviewContract) => _queries.AddReview(id, reviewContract);

		[Authorize]
		[HttpDelete("{id:int}/reviews/{reviewId:int}")]
		public void DeleteReview(int reviewId) => _queries.DeleteReview(reviewId);

		/// <summary>
		/// Gets list of top rated albums, same as front page.
		/// </summary>
		/// <remarks>
		/// Output is cached for 1 hour.
		/// </remarks>
		[HttpGet("top")]
		[CacheOutput(ClientTimeSpan = HourInSeconds, ServerTimeSpan = HourInSeconds)]
		public IEnumerable<AlbumForApiContract> GetTopAlbums(
			[FromQuery(Name = "ignoreIds[]")] int[] ignoreIds = null,
			ContentLanguagePreference languagePreference = ContentLanguagePreference.Default,
			AlbumOptionalFields fields = AlbumOptionalFields.None
		)
		{
			ignoreIds ??= Array.Empty<int>();
			return _otherService.GetTopAlbums(languagePreference, fields, ignoreIds);
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpGet("{id:int}/tagSuggestions")]
		public Task<TagUsageForApiContract[]> GetTagSuggestions(int id) => _queries.GetTagSuggestions(id);

		/// <summary>
		/// Gets tracks for an album.
		/// </summary>
		/// <param name="id">Album ID (required).</param>
		/// <param name="fields">
		/// List of optional fields (optional). Possible values are Albums, Artists, Names, PVs, Tags, ThumbUrl, WebLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>List of tracks for the album.</returns>
		/// <example>https://vocadb.net/api/albums/1/tracks</example>
		[HttpGet("{id:int}/tracks")]
		public SongInAlbumForApiContract[] GetTracks(
			int id,
			SongOptionalFields fields = SongOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default
		) => _service.GetAlbum(id, a => a.Songs.Select(s => new SongInAlbumForApiContract(s, lang, fields)).ToArray());

		/// <summary>
		/// Gets tracks for an album formatted using the CSV format string.
		/// </summary>
		/// <param name="id">Album ID.</param>
		/// <param name="field">Field to be included, for example "featvocalists" or "url". Can be specified multiple times.</param>
		/// <param name="discNumber">Disc number to filter by. If not specified, all discs are included.</param>
		/// <param name="lang">Language preference.</param>
		/// <returns>List of songs with the specified fields.</returns>
		/// <example>https://vocadb.net/api/albums/5111/tracks/fields?field=title&field=featvocalists</example>
		[HttpGet("{id:int}/tracks/fields")]
		public IEnumerable<Dictionary<string, string>> GetTracksFields(
			int id,
			[FromQuery(Name = "field[]")] string[] field = null,
			int? discNumber = null,
			ContentLanguagePreference lang = ContentLanguagePreference.Default
		) => _queries.GetTracksFormatted(id, discNumber, field, lang);

		[HttpGet("ids")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public IEnumerable<int> GetIds() => _queries.GetIds();

		/// <summary>
		/// Gets a complete list of album versions and Ids.
		/// Intended for integration to other systems.
		/// </summary>
		/// <returns>List of album IDs with versions.</returns>
		[HttpGet("versions")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public EntryIdAndVersionContract[] GetVersions() => _queries.GetVersions();

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
		public void PostEditComment(int commentId, CommentForApiContract contract) => _queries.PostEditComment(commentId, contract);

		/// <summary>
		/// Posts a new comment.
		/// </summary>
		/// <param name="id">ID of the album for which to create the comment.</param>
		/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
		/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
		[HttpPost("{id:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int id, CommentForApiContract contract) => _queries.CreateComment(id, contract);

		[HttpPost("{id:int}/personal-description")]
		[ApiExplorerSettings(IgnoreApi = true)]
		[Authorize]
		public void PostPersonalDescription(int id, AlbumDetailsContract data) => _queries.UpdatePersonalDescription(id, data);

#nullable enable
		[HttpGet("{id:int}/details")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public AlbumDetailsForApiContract GetDetails(int id)
		{
			WebHelper.VerifyUserAgent(Request);

			return _queries.GetAlbumDetailsForApi(id: id, hostname: WebHelper.GetHostnameForValidHit(Request));
		}

		[HttpGet("{id:int}/versions")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public EntryWithArchivedVersionsForApiContract<AlbumForApiContract> GetAlbumWithArchivedVersions(int id) =>
			_queries.GetAlbumWithArchivedVersionsForApi(id);

		[HttpGet("versions/{id:int}")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public ArchivedAlbumVersionDetailsForApiContract GetVersionDetails(int id, int comparedVersionId = 0) =>
			_queries.GetVersionDetailsForApi(id, comparedVersionId);

		[HttpPost("")]
		[Authorize]
		[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
		[ValidateAntiForgeryToken]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<ActionResult<int>> Create(
			[ModelBinder(BinderType = typeof(JsonModelBinder))] CreateAlbumForApiContract contract
		)
		{
			if (contract.Names.All(name => string.IsNullOrWhiteSpace(name.Value)))
				ModelState.AddModelError("Names", ViewRes.EntryCreateStrings.NeedName);

			if (contract.Artists is null || !contract.Artists.Any())
				ModelState.AddModelError("Artists", ViewRes.Album.CreateStrings.NeedArtist);

			if (!ModelState.IsValid)
				return ValidationProblem(ModelState);

			var album = await _queries.Create(contract);

			return album.Id;
		}

		[HttpPost("{id:int}")]
		[Authorize]
		[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
		[ValidateAntiForgeryToken]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<ActionResult<int>> Edit(
			[ModelBinder(BinderType = typeof(JsonModelBinder))] AlbumForEditForApiContract contract
		)
		{
			// Unable to continue if viewmodel is null because we need the ID at least
			if (contract is null)
			{
				return BadRequest("Viewmodel was null - probably JavaScript is disabled");
			}

			try
			{
				static void CheckModel(AlbumForEditForApiContract contract)
				{
					if (contract is null)
						throw new InvalidFormException("Model was null");

					if (contract.ArtistLinks is null)
						throw new InvalidFormException("Artists list was null");

					if (contract.Identifiers is null)
						throw new InvalidFormException("Identifiers list was null");

					if (contract.Names is null)
						throw new InvalidFormException("Names list was null");

					if (contract.PVs is null)
						throw new InvalidFormException("PVs list was null");

					if (contract.Songs is null)
						throw new InvalidFormException("Tracks list was null");

					if (contract.WebLinks is null)
						throw new InvalidFormException("WebLinks list was null");
				}

				CheckModel(contract);
			}
			catch (InvalidFormException x)
			{
				ControllerBase.AddFormSubmissionError(this, x.Message);
			}

			// Note: name is allowed to be whitespace, but not empty.
			if (contract.Names is not null && contract.Names.All(n => n is null || string.IsNullOrEmpty(n.Value)))
			{
				ModelState.AddModelError("Names", AlbumValidationErrors.UnspecifiedNames);
			}

			if (
				contract.OriginalRelease is not null &&
				contract.OriginalRelease.ReleaseDate is not null &&
				!OptionalDateTime.IsValid(contract.OriginalRelease.ReleaseDate.Year, contract.OriginalRelease.ReleaseDate.Day, contract.OriginalRelease.ReleaseDate.Month)
			)
			{
				ModelState.AddModelError("ReleaseYear", "Invalid date");
			}

			var coverPicUpload = Request.Form.Files["coverPicUpload"];
			var pictureData = ControllerBase.ParsePicture(this, coverPicUpload, "CoverPicture", ImagePurpose.Main);

			if (contract.Pictures is null)
			{
				ControllerBase.AddFormSubmissionError(this, "List of pictures was null");
			}

			if (contract.Pictures is not null)
				ControllerBase.ParseAdditionalPictures(this, coverPicUpload, contract.Pictures);

			if (!ModelState.IsValid)
			{
				return ValidationProblem(ModelState);
			}

			try
			{
				await _queries.UpdateBasicProperties(contract, pictureData);
			}
			catch (InvalidPictureException)
			{
				ModelState.AddModelError("ImageError", "The uploaded image could not processed, it might be broken. Please check the file and try again.");
				return ValidationProblem(ModelState);
			}

			return contract.Id;
		}

		[HttpPost("{id:int}/merge")]
		[Authorize]
		[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
		[ValidateAntiForgeryToken]
		[ApiExplorerSettings(IgnoreApi = true)]
		public ActionResult Merge(int id, int targetAlbumId)
		{
			_queries.Merge(id, targetAlbumId);

			return NoContent();
		}

		[HttpPost("versions/{archivedVersionId:int}/update-visibility")]
		[Authorize]
		[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
		[ValidateAntiForgeryToken]
		[ApiExplorerSettings(IgnoreApi = true)]
		public ActionResult UpdateVersionVisibility(int archivedVersionId, bool hidden)
		{
			_queries.UpdateVersionVisibility<ArchivedAlbumVersion>(archivedVersionId, hidden);

			return NoContent();
		}
#nullable disable
	}
}
