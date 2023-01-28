#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.DataContracts.SongLists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.SongImport;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Models.Shared;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api;

/// <summary>
/// API queries for song lists.
/// </summary>
[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
[Route("api/songLists")]
[ApiController]
public class SongListApiController : ApiController
{
	private const int AbsoluteMax = 100;
	private const int DefaultMax = 10;
	private readonly SongListQueries _queries;
	private readonly IUserIconFactory _userIconFactory;
	private readonly IAggregatedEntryImageUrlFactory _entryImagePersister;

	public SongListApiController(SongListQueries queries, IUserIconFactory userIconFactory, IAggregatedEntryImageUrlFactory entryImagePersister)
	{
		_queries = queries;
		_userIconFactory = userIconFactory;
		_entryImagePersister = entryImagePersister;
	}

	/// <summary>
	/// Deletes a song list.
	/// </summary>
	/// <param name="id">ID of the list to be deleted.</param>
	/// <param name="notes">Notes.</param>
	/// <param name="hardDelete">
	/// If true, the entry is hard deleted. Hard deleted entries cannot be restored normally, but they will be moved to trash.
	/// If false, the entry is soft deleted, meaning it can still be restored.
	/// </param>
	[HttpDelete("{id:int}")]
	[Authorize]
	[ValidateAntiForgeryToken]
	public void Delete(int id, string notes = "", bool hardDelete = false)
	{
		notes ??= string.Empty;

		if (hardDelete)
		{
			_queries.MoveToTrash(id);
		}
		else
		{
			_queries.Delete(id, notes);
		}
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
	public void DeleteComment(int commentId) => _queries.DeleteComment(commentId);

	/// <summary>
	/// Gets a list of comments for a song list.
	/// </summary>
	/// <param name="listId">ID of the list whose comments to load.</param>
	/// <returns>List of comments in no particular order.</returns>
	[HttpGet("{listId:int}/comments")]
	public PartialFindResult<CommentForApiContract> GetComments(int listId) => new PartialFindResult<CommentForApiContract>(_queries.GetComments(listId), 0);

	[HttpGet("{id:int}/for-edit")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public SongListForEditForApiContract GetForEdit(int id) => _queries.GetSongListForEdit(id);

#nullable enable
	/// <summary>
	/// Gets a list of featured song lists.
	/// </summary>
	/// <param name="query">Song list name query (optional).</param>
	/// <param name="tagId">Filter by one or more tag Ids (optional).</param>
	/// <param name="childTags">Include child tags, if the tags being filtered by have any.</param>
	/// <param name="nameMatchMode">Match mode for list name (optional, defaults to Auto).</param>
	/// <param name="featuredCategory">Filter by a specific featured category. If empty, all categories are returned.</param>
	/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
	/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
	/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
	/// <param name="sort">List sort rule. Possible values are Nothing, Date, CreateDate, Name.</param>
	/// <param name="fields">List of optional fields (optional).</param>
	/// <param name="lang">Content language preference (optional).</param>
	/// <returns>List of song lists.</returns>
	[HttpGet("featured")]
	public PartialFindResult<SongListForApiContract> GetFeaturedLists(
		string query = "",
		[FromQuery(Name = "tagId[]")] int[]? tagId = null,
		bool childTags = false,
		NameMatchMode nameMatchMode = NameMatchMode.Auto,
		SongListFeaturedCategory? featuredCategory = null,
		int start = 0, int maxResults = DefaultMax, bool getTotalCount = false,
		SongListSortRule sort = SongListSortRule.Name,
		SongListOptionalFields fields = SongListOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	)
	{
		var textQuery = SearchTextQuery.Create(query, nameMatchMode);
		var queryParams = new SongListQueryParams
		{
			TextQuery = textQuery,
			FeaturedCategory = featuredCategory,
			Paging = new PagingProperties(start, maxResults, getTotalCount),
			SortRule = sort,
			TagIds = tagId,
			ChildTags = childTags
		};

		return _queries.Find(s => new SongListForApiContract(s, lang, _userIconFactory, _entryImagePersister, fields), queryParams);
	}
#nullable disable

	/// <summary>
	/// Gets a list of featuedd list names. Ideal for autocomplete boxes.
	/// </summary>
	/// <param name="query">Text query.</param>
	/// <param name="nameMatchMode">Name match mode. Words is treated the same as Partial.</param>
	/// <param name="featuredCategory">Filter by a specific featured category. If empty, all categories are returned.</param>
	/// <param name="maxResults">Maximum number of results.</param>
	/// <returns>List of list names.</returns>
	[HttpGet("featured/names")]
	public IEnumerable<string> GetFeaturedListNames(string query = "",
		NameMatchMode nameMatchMode = NameMatchMode.Auto,
		SongListFeaturedCategory? featuredCategory = null,
		int maxResults = 10) => _queries.GetFeaturedListNames(query, nameMatchMode, featuredCategory, maxResults);

#nullable enable
	/// <summary>
	/// Gets a list of songs in a song list.
	/// </summary>
	/// <param name="listId">ID of the song list.</param>
	/// <param name="query">Song name query (optional).</param>
	/// <param name="songTypes">
	/// Filtered song types (optional). 
	/// </param>
	/// <param name="pvServices">Filter by one or more PV services (separated by commas). The song will pass the filter if it has a PV for any of the matched services.</param>
	/// <param name="tagId">Filter by one or more tag Ids (optional).</param>
	/// <param name="artistId">Filter by artist Id.</param>
	/// <param name="childVoicebanks">Include child voicebanks, if the artist being filtered by has any.</param>
	/// <param name="advancedFilters">List of advanced filters (optional).</param>
	/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
	/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
	/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
	/// <param name="sort">Song sort rule (optional, by default songs are sorted by song list order).</param>
	/// <param name="nameMatchMode">Match mode for song name (optional, defaults to Auto).</param>
	/// <param name="fields">
	/// List of optional fields (optional). Possible values are Albums, Artists, Names, PVs, Tags, ThumbUrl, WebLinks.
	/// </param>
	/// <param name="lang">Content language preference (optional).</param>
	/// <returns>Page of songs.</returns>
	[HttpGet("{listId:int}/songs")]
	public PartialFindResult<SongInListForApiContract> GetSongs(
		int listId,
		string query = "",
		string? songTypes = null,
		PVServices? pvServices = null,
		[FromQuery(Name = "tagId[]")] int[]? tagId = null,
		[FromQuery(Name = "artistId[]")] int[]? artistId = null,
		bool childVoicebanks = false,
		[FromQuery(Name = "advancedFilters")] AdvancedSearchFilterParams[]? advancedFilters = null,
		int start = 0, int maxResults = DefaultMax, bool getTotalCount = false,
		SongSortRule? sort = null,
		NameMatchMode nameMatchMode = NameMatchMode.Auto,
		SongOptionalFields fields = SongOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	)
	{
		maxResults = Math.Min(maxResults, AbsoluteMax);
		var types = EnumVal<SongType>.ParseMultiple(songTypes);

		return _queries.GetSongsInList(
			new SongInListQueryParams
			{
				TextQuery = SearchTextQuery.Create(query, nameMatchMode),
				ListId = listId,
				Paging = new PagingProperties(start, maxResults, getTotalCount),
				PVServices = pvServices,
				ArtistIds = artistId,
				ChildVoicebanks = childVoicebanks,
				TagIds = tagId,
				SortRule = sort,
				AdvancedFilters = advancedFilters?.Select(advancedFilter => advancedFilter.ToAdvancedSearchFilter()).ToArray(),
				SongTypes = types,
			},
			songInList => new SongInListForApiContract(songInList, lang, fields)
		);
	}
#nullable disable

	[ApiExplorerSettings(IgnoreApi = true)]
	[HttpGet("import")]
	public async Task<ActionResult<ImportedSongListContract>> GetImport(string url, bool parseAll = true)
	{
		try
		{
			return await _queries.Import(url, parseAll);
		}
		catch (UnableToImportException x)
		{
			return BadRequest(x.Message);
		}
	}

	[ApiExplorerSettings(IgnoreApi = true)]
	[HttpGet("import-songs")]
	public async Task<ActionResult<PartialImportedSongs>> GetImportSongs(string url, string pageToken, int maxResults = 20, bool parseAll = true)
	{
		try
		{
			return await _queries.ImportSongs(url, pageToken, maxResults, parseAll);
		}
		catch (UnableToImportException x)
		{
			return BadRequest(x.Message);
		}
	}

	/// <summary>
	/// Creates a song list.
	/// </summary>
	/// <param name="list">Song list properties.</param>
	/// <returns>ID of the created list.</returns>
	[HttpPost("")]
	[Authorize]
	public ActionResult<int> Post(SongListForEditForApiContract list)
	{
		if (list == null)
			return BadRequest();

		return _queries.UpdateSongList(list, uploadedFile: null);
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
	[HttpPost("comments/{commentId:int}")]
	[Authorize]
	public void PostEditComment(int commentId, CommentForApiContract contract) => _queries.PostEditComment(commentId, contract);

	/// <summary>
	/// Posts a new comment.
	/// </summary>
	/// <param name="listId">ID of the song list for which to create the comment.</param>
	/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
	/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
	[HttpPost("{listId:int}/comments")]
	[Authorize]
	public CommentForApiContract PostNewComment(int listId, CommentForApiContract contract) => _queries.CreateComment(listId, contract);

#nullable enable
	[HttpGet("{id:int}/details")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public SongListForApiContract GetDetails(int id) => _queries.GetDetails(id);

	[HttpGet("{id:int}/versions")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public EntryWithArchivedVersionsForApiContract<SongListForApiContract> GetSongListWithArchivedVersions(int id) =>
		_queries.GetSongListWithArchivedVersionsForApi(id);

	[HttpGet("{id:int}")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public SongListBaseContract GetOne(int id) => _queries.GetOne(id);

	[HttpPost("{id:int}")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ValidateAntiForgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult<int> Edit(
		[ModelBinder(BinderType = typeof(JsonModelBinder))] SongListForEditForApiContract contract
	)
	{
		if (contract is null)
		{
			return BadRequest("View model was null - probably JavaScript is disabled");
		}

		var coverPicUpload = Request.Form.Files["thumbPicUpload"];
		UploadedFileContract? uploadedPicture = null;
		if (coverPicUpload is not null && coverPicUpload.Length > 0)
		{
			ControllerBase.CheckUploadedPicture(this, coverPicUpload, "thumbPicUpload");
			uploadedPicture = new UploadedFileContract { Mime = coverPicUpload.ContentType, Stream = coverPicUpload.OpenReadStream() };
		}

		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var listId = _queries.UpdateSongList(contract, uploadedPicture);

		return listId;
	}
#nullable disable
}
