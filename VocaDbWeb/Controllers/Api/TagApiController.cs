#nullable disable

using AspNetCore.CacheOutput;
using IvanAkcheurov.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api;

/// <summary>
/// API queries for tags.
/// </summary>
[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
[Route("api/tags")]
[ApiController]
public class TagApiController : ApiController
{
	private const int AbsoluteMax = 100;
	private const int DefaultMax = 10;

	private readonly TagQueries _queries;
	private readonly IAggregatedEntryImageUrlFactory _thumbPersister;
	private readonly IUserPermissionContext _permissionContext;
	private readonly EditRateLimitService _rateLimitService;

	public TagApiController(
		TagQueries queries,
		IAggregatedEntryImageUrlFactory thumbPersister,
		IUserPermissionContext permissionContext,
		EditRateLimitService rateLimitService
	)
	{
		_queries = queries;
		_thumbPersister = thumbPersister;
		_permissionContext = permissionContext;
		_rateLimitService = rateLimitService;
	}

	/// <summary>
	/// Deletes a tag.
	/// </summary>
	/// <param name="id">ID of the tag to be deleted.</param>
	/// <param name="notes">Notes (optional).</param>
	/// <param name="hardDelete">
	/// If true, the entry is hard deleted. Hard deleted entries cannot be restored normally, but they will be moved to trash.
	/// If false, the entry is soft deleted, meaning it can still be restored.
	/// </param>
	[HttpDelete("{id:int}")]
	[Authorize]
	[OriginHeaderCheck]
	public void Delete(int id, string notes = "", bool hardDelete = false)
	{
		notes ??= string.Empty;

		if (hardDelete)
		{
			_queries.MoveToTrash(id, notes);
		}
		else
		{
			_queries.Delete(id, notes);
		}
	}

	/// <summary>
	/// Deletes a comment.
	/// Normal users can delete their own comments, moderators can delete all comments.
	/// Requires login.
	/// </summary>
	/// <param name="commentId">ID of the comment to be deleted.</param>
	[HttpDelete("comments/{commentId:int}")]
	[Authorize]
	public void DeleteComment(int commentId) => _queries.DeleteComment(commentId);

	/// <summary>
	/// Gets a tag by ID.
	/// </summary>
	/// <param name="id">Tag ID (required).</param>
	/// <param name="fields">
	/// List of optional fields (optional). 
	/// </param>
	/// <param name="lang">Content language preference (optional).</param>
	/// <example>http://vocadb.net/api/tags/1</example>
	/// <returns>Tag data.</returns>
	[HttpGet("{id:int}")]
	public TagForApiContract GetById(
		int id,
		TagOptionalFields fields = TagOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	)
	{
		return _queries.LoadTag(id, t => new TagForApiContract(
			t,
			_thumbPersister,
			lang,
			_permissionContext,
			fields
		));
	}

	/// <summary>
	/// DEPRECATED. Gets a tag by name.
	/// </summary>
	/// <param name="name">Tag name (required).</param>
	/// <param name="fields">
	/// List of optional fields (optional). 
	/// </param>
	/// <param name="lang">Content language preference (optional).</param>
	/// <example>http://vocadb.net/api/tags/byName/vocarock</example>
	/// <returns>Tag data.</returns>
	[HttpGet("byName/{name}")]
	[Obsolete]
	public TagForApiContract GetByName(
		string name,
		TagOptionalFields fields = TagOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	)
	{
		return _queries.GetTagByName(name, t => new TagForApiContract(
			t,
			_thumbPersister,
			lang,
			_permissionContext,
			fields
		));
	}

	/// <summary>
	/// Gets a list of tag category names.
	/// </summary>
	[HttpGet("categoryNames")]
	[CacheOutput(ClientTimeSpan = 86400, ServerTimeSpan = 86400)]
	public string[] GetCategoryNamesList(
		string query = "",
		NameMatchMode nameMatchMode = NameMatchMode.Auto
	) => _queries.FindCategories(SearchTextQuery.Create(query, nameMatchMode));

	/// <summary>
	/// Gets a list of child tags for a tag.
	/// Only direct children will be included.
	/// </summary>
	/// <param name="tagId">ID of the tag whose children to load.</param>
	/// <param name="fields">List of optional fields (optional).</param>
	/// <param name="lang">Content language preference (optional).</param>
	/// <returns>List of child tags.</returns>
	/// <example>http://vocadb.net/api/tags/481/children</example>
	[HttpGet("{tagId:int}/children")]
	public TagForApiContract[] GetChildTags(
		int tagId,
		TagOptionalFields fields = TagOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	) => _queries.GetChildTags(tagId, fields, lang);

	/// <summary>
	/// Gets a list of comments for a tag.
	/// Note: pagination and sorting might be added later.
	/// </summary>
	/// <param name="tagId">ID of the tag whose comments to load.</param>
	/// <returns>List of comments in no particular order.</returns>
	[HttpGet("{tagId:int}/comments")]
	public PartialFindResult<CommentForApiContract> GetComments(int tagId) => new PartialFindResult<CommentForApiContract>(_queries.GetComments(tagId), 0);

#nullable enable
	/// <summary>
	/// Find tags.
	/// </summary>
	/// <param name="query">Tag name query (optional).</param>
	/// <param name="allowChildren">Whether to allow child tags. If this is false, only root tags (that aren't children of any other tag) will be included.</param>
	/// <param name="categoryName">Filter tags by category (optional). If specified, this must be an exact match (case insensitive).</param>
	/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
	/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 30).</param>
	/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
	/// <param name="nameMatchMode">Match mode for song name (optional, defaults to Exact).</param>
	/// <param name="sort">Sort rule (optional, by default tags are sorted by name).Possible values are Name and UsageCount.</param>
	/// <param name="preferAccurateMatches">
	/// Whether the search should prefer accurate matches. 
	/// If this is true, entries that match by prefix will be moved first, instead of being sorted alphabetically.
	/// Requires a text query. Does not support pagination.
	/// This is mostly useful for autocomplete boxes.
	/// </param>
	/// <param name="fields">List of optional fields (optional).</param>
	/// <param name="lang">Content language preference (optional).</param>
	/// <returns>Page of tags.</returns>
	/// <example>http://vocadb.net/api/tags?query=voca&amp;nameMatchMode=StartsWith</example>
	[HttpGet("")]
	public PartialFindResult<TagForApiContract> GetList(
		string query = "",
		bool allowChildren = true,
		string categoryName = "",
		int start = 0, int maxResults = DefaultMax, bool getTotalCount = false,
		NameMatchMode nameMatchMode = NameMatchMode.Exact,
		TagSortRule? sort = null,
		bool preferAccurateMatches = false,
		TagOptionalFields fields = TagOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default,
		string? target = null,
		bool deleted = false
	)
	{
		maxResults = Math.Min(maxResults, fields != TagOptionalFields.None ? AbsoluteMax : int.MaxValue);
		var queryParams = new TagQueryParams(new CommonSearchParams(TagSearchTextQuery.Create(query, nameMatchMode), false, preferAccurateMatches),
			new PagingProperties(start, maxResults, getTotalCount))
		{
			AllowChildren = allowChildren,
			CategoryName = categoryName,
			SortRule = sort ?? TagSortRule.Name,
			LanguagePreference = lang,
			Target = target,
			Deleted = deleted
		};

		var tags = _queries.Find(queryParams, fields, lang);

		return tags;
	}
#nullable disable

	[HttpGet("entry-type-mappings")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public TagEntryMappingContract[] GetEntryMappings() => _queries.GetEntryMappings();

	[HttpGet("mappings")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public PartialFindResult<TagMappingContract> GetMappings(
		int start = 0,
		int maxEntries = DefaultMax,
		bool getTotalCount = false
	) => _queries.GetMappings(new PagingProperties(start, maxEntries, getTotalCount));

	/// <summary>
	/// Find tag names by a part of name.
	/// 
	/// Matching is done anywhere from the name.
	/// </summary>
	/// <param name="query">Tag name query, for example "rock".</param>
	/// <param name="allowAliases">
	/// Whether to find tags that are aliases of other tags as well. 
	/// If false, only tags that are not aliases will be listed.
	/// </param>
	/// <param name="maxResults">Maximum number of search results.</param>
	/// <returns>
	/// List of tag names, for example "vocarock", matching the query. Cannot be null.
	/// </returns>
	[HttpGet("names")]
	public string[] GetNames(
		string query = "",
		bool allowAliases = true,
		int maxResults = 10
	) => _queries.FindNames(TagSearchTextQuery.Create(query), allowAliases, maxResults);

	/// <summary>
	/// Gets the most common tags in a category.
	/// </summary>
	/// <param name="categoryName">Tag category, for example "Genres". Optional - if not specified, no filtering is done.</param>
	/// <param name="entryType">Tag usage entry type. Optional - if not specified, all entry types are included.</param>
	/// <param name="maxResults">Maximum number of tags to return.</param>
	/// <param name="lang">Content language preference (optional).</param>
	/// <returns>List of names of the most commonly used tags in that category.</returns>
	[HttpGet("top")]
	[CacheOutput(ClientTimeSpan = 86400, ServerTimeSpan = 86400)]
	public TagBaseContract[] GetTopTags(
		string categoryName = null,
		EntryType? entryType = null,
		int maxResults = 15,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	) => _queries.GetTopTags(categoryName, entryType, maxResults, lang);

	/// <summary>
	/// Creates a new report.
	/// </summary>
	/// <param name="tagId">Tag to be reported.</param>
	/// <param name="reportType">Report type.</param>
	/// <param name="notes">Notes. Optional.</param>
	/// <param name="versionNumber">Version to be reported. Optional.</param>
	[HttpPost("{tagId:int}/reports")]
	[RestrictBannedIP]
	public async Task<IActionResult> PostReport(int tagId, TagReportType reportType, string notes, int? versionNumber)
	{
		var (created, _) = await _queries.CreateReport(tagId, reportType, WebHelper.GetRealHost(Request), notes ?? string.Empty, versionNumber);

		return created ? NoContent() : BadRequest();
	}

	/// <summary>
	/// Creates a new tag.
	/// </summary>
	/// <param name="name">Tag English name. Tag names must be unique.</param>
	/// <returns>The created tag.</returns>
	/// <response code="200">OK</response>		
	/// <response code="400">If tag name is already in use</response>
	[HttpPost("")]
	[Authorize]
	public async Task<ActionResult<TagBaseContract>> PostNewTag(string name)
	{
		try
		{
			return await _queries.Create(name);
		}
		catch (DuplicateTagNameException)
		{
			return BadRequest("Tag name is already in use");
		}
	}

	/// <summary>
	/// Updates a comment.
	/// Normal users can edit their own comments, moderators can edit all comments.
	/// Requires login.
	/// </summary>
	/// <param name="commentId">ID of the comment to be edited.</param>
	/// <param name="contract">New comment data. Only message can be edited.</param>
	[HttpPost("comments/{commentId:int}")]
	[Authorize]
	public void PostEditComment(int commentId, CommentForApiContract contract) => _queries.PostEditComment(commentId, contract);

	/// <summary>
	/// Posts a new comment.
	/// </summary>
	/// <param name="tagId">ID of the tag for which to create the comment.</param>
	/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
	/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
	[HttpPost("{tagId:int}/comments")]
	[Authorize]
	public CommentForApiContract PostNewComment(int tagId, CommentForApiContract contract) => _queries.CreateComment(tagId, contract);

	[Authorize]
	[HttpPut("entry-type-mappings")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public IActionResult PutEntryMappings([FromBody] IEnumerable<TagEntryMappingContract> mappings)
	{
		if (mappings == null)
			return BadRequest("Mappings cannot be null");

		_queries.UpdateEntryMappings(mappings.ToArray());
		return NoContent();
	}

	[Authorize]
	[HttpPut("mappings")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public IActionResult PutMappings([FromBody] IEnumerable<TagMappingContract> mappings)
	{
		if (mappings == null)
			return BadRequest("Mappings cannot be null");

		_queries.UpdateMappings(mappings.ToArray());
		return NoContent();
	}

	[HttpGet("by-categories")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public TagCategoryForApiContract[] GetTagsByCategories() => _queries.GetTagsByCategories();

#nullable enable
	[HttpGet("{id:int}/details")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public Task<TagDetailsForApiContract> GetDetails(int id) => _queries.GetDetailsAsync(id);

	[HttpGet("{id:int}/versions")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public EntryWithArchivedVersionsForApiContract<TagForApiContract> GetTagWithArchivedVersions(int id) =>
		_queries.GetTagWithArchivedVersionsForApi(id);

	[HttpGet("versions/{id:int}")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ArchivedTagVersionDetailsForApiContract GetVersionDetails(int id, int comparedVersionId = 0) =>
		_queries.GetVersionDetailsForApi(id, comparedVersionId);

	[HttpGet("{id:int}/for-edit")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public TagForEditForApiContract GetForEdit(int id) => _queries.GetTagForEdit(id);

	[HttpPost("{id:int}")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[OriginHeaderCheck]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult<int> Edit(
		[ModelBinder(BinderType = typeof(JsonModelBinder))] TagForEditForApiContract contract
	)
	{
		_rateLimitService.RegisterEdit(_permissionContext);
		
		var coverPicUpload = Request.Form.Files["thumbPicUpload"];
		UploadedFileContract? uploadedPicture = null;
		if (coverPicUpload is not null && coverPicUpload.Length > 0)
		{
			ControllerBase.CheckUploadedPicture(this, coverPicUpload, "thumbPicUpload");
			uploadedPicture = new UploadedFileContract { Mime = coverPicUpload.ContentType, Stream = coverPicUpload.OpenReadStream() };
		}

		try
		{
			static void CheckModel(TagForEditForApiContract contract)
			{
				if (contract.Description is null)
					throw new InvalidFormException("Description was null");

				if (contract.Names is null)
					throw new InvalidFormException("Names list was null");

				if (contract.WebLinks is null)
					throw new InvalidFormException("WebLinks list was null");
				
				string[] possibleTagTargetTypes = {"album", "artist", "releaseevent", "song"};
				
				// TODO: Filter by allowed types (server config)
				var possibleTagTargetSubtypes = Enum.GetValues<SongType>()
					.Select(val => val.ToString())
					.Concat(Enum.GetValues<EventCategory>().Select(val => val.ToString()))
					.Concat(Enum.GetValues<DiscType>().Select(val => val.ToString()))
					.Concat(Enum.GetValues<ArtistType>().Select(val => val.ToString()))
					.Select(val => val.ToLower());
				contract.NewTargets.ForEach(target =>
				{
					var elements = target.Split(":");
					if (elements.Length != 1 && elements.Length != 2)
						throw new InvalidFormException("Invalid tag targets");
					if (!possibleTagTargetTypes.Contains(elements[0]))
						throw new InvalidFormException($"Invalid tag target main type {elements[0]}");
					if (elements.Length != 2) return;
					if (!possibleTagTargetSubtypes.Contains(elements[1]))
						throw new InvalidFormException($"Invalid tag target sub type {elements[1]}");
				});
			}

			CheckModel(contract);
		}
		catch (InvalidFormException x)
		{
			ControllerBase.AddFormSubmissionError(this, x.Message);
		}

		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		TagBaseContract result;

		try
		{
			result = _queries.Update(contract, uploadedPicture);
		}
		catch (DuplicateTagNameException x)
		{
			ModelState.AddModelError("Names", x.Message);
			return ValidationProblem(ModelState);
		}

		return result.Id;
	}

	[HttpPost("{id:int}/merge")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[OriginHeaderCheck]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult Merge(int id, int? targetTagId)
	{
		if (targetTagId == null)
		{
			ModelState.AddModelError("targetTagId", "Tag must be selected");
			return ValidationProblem(ModelState);
		}

		_queries.Merge(id, targetTagId.Value);

		return NoContent();
	}

	[HttpPost("versions/{archivedVersionId:int}/update-visibility")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[OriginHeaderCheck]
	[ApiExplorerSettings(IgnoreApi = true)]
	public ActionResult UpdateVersionVisibility(int archivedVersionId, bool hidden)
	{
		_queries.UpdateVersionVisibility<ArchivedTagVersion>(archivedVersionId, hidden);

		return NoContent();
	}
#nullable disable
}
