#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Models.Shared;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// API queries for artists.
	/// </summary>
	[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
	[Route("api/artists")]
	[ApiController]
	public class ArtistApiController : ApiController
	{
		private const int AbsoluteMax = 100;
		private const int DefaultMax = 10;
		private readonly ObjectCache _cache;
		private readonly ArtistQueries _queries;
		private readonly ArtistService _service;
		private readonly IAggregatedEntryImageUrlFactory _thumbPersister;

		public ArtistApiController(ArtistQueries queries, ArtistService service, IAggregatedEntryImageUrlFactory thumbPersister, ObjectCache cache)
		{
			_queries = queries;
			_service = service;
			_thumbPersister = thumbPersister;
			_cache = cache;
		}

		private ArtistForApiContract GetArtist(Artist a, ArtistMergeRecord m,
			ArtistOptionalFields fields,
			ArtistRelationsFields relations,
			ContentLanguagePreference lang,
			IDatabaseContext<Artist> ctx)
		{
			var contract = new ArtistForApiContract(a, lang, _thumbPersister, fields);

			if (relations != ArtistRelationsFields.None)
			{
				contract.Relations = new ArtistRelationsQuery(ctx, lang, _cache, _thumbPersister).GetRelations(a, relations);
			}

			return contract;
		}

		/// <summary>
		/// Deletes an artist.
		/// </summary>
		/// <param name="id">ID of the artist to be deleted.</param>
		/// <param name="notes">Notes.</param>
		[HttpDelete("{id:int}")]
		[Authorize]
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
		/// Gets a list of comments for an artist.
		/// </summary>
		/// <param name="id">ID of the artist whose comments to load.</param>
		/// <returns>List of comments in no particular order.</returns>
		/// <remarks>
		/// Pagination and sorting might be added later.
		/// </remarks>
		[HttpGet("{id:int}/comments")]
		public IEnumerable<CommentForApiContract> GetComments(int id) => _queries.GetComments(id);

		[HttpGet("{id:int}/for-edit")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public ArtistForEditContract GetForEdit(int id) => _queries.GetArtistForEdit(id);

		/// <summary>
		/// Gets an artist by Id.
		/// </summary>
		/// <param name="id">Artist ID (required).</param>
		/// <param name="fields">List of optional fields (optional). Possible values are Description, Groups, Members, Names, Tags, WebLinks.</param>
		/// <param name="relations">List of artist relations (optional). Possible values are LatestAlbums, PopularAlbums, LatestSongs, PopularSongs, All</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Artist data.</returns>
		/// <example>http://vocadb.net/api/artists/1</example>
		[HttpGet("{id:int}")]
		public ArtistForApiContract GetOne(int id,
			ArtistOptionalFields fields = ArtistOptionalFields.None,
			ArtistRelationsFields relations = ArtistRelationsFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) => _queries.GetWithMergeRecord(id, (a, m, ctx) => GetArtist(a, m, fields, relations, lang, ctx));

#nullable enable
		/// <summary>
		/// Find artists.
		/// </summary>
		/// <param name="query">Artist name query (optional).</param>
		/// <param name="artistTypes">Filtered artist type (optional).</param>
		/// <param name="allowBaseVoicebanks">Allow base voicebanks. If false, only root voicebanks will be allowed. Only affects voice synthesizers that can have base voicebanks.</param>
		/// <param name="tagId">Filter by tag Id (optional). This filter can be specified multiple times.</param>
		/// <param name="tagName">Filter by tag name (optional).</param>
		/// <param name="childTags">Include child tags, if the tags being filtered by have any.</param>
		/// <param name="followedByUserId">Filter by user following the artists. By default there is no filtering.</param>
		/// <param name="status">Filter by entry status (optional).</param>
		/// <param name="advancedFilters">List of advanced filters (optional).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 100).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">Sort rule (optional, defaults to Name). Possible values are None, Name, AdditionDate, AdditionDateAsc.</param>
		/// <param name="preferAccurateMatches">
		/// Whether the search should prefer accurate matches. 
		/// If this is true, entries that match by prefix will be moved first, instead of being sorted alphabetically.
		/// Requires a text query. Does not support pagination.
		/// This is mostly useful for autocomplete boxes.
		/// </param>
		/// <param name="nameMatchMode">Match mode for artist name (optional, defaults to Exact).</param>
		/// <param name="fields">List of optional fields (optional). Possible values are Description, Groups, Members, Names, Tags, WebLinks.</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of artists.</returns>
		/// <example>http://vocadb.net/api/artists?query=Tripshots&amp;artistTypes=Producer</example>
		[HttpGet("")]
		public PartialFindResult<ArtistForApiContract> GetList(
			string query = "",
			string? artistTypes = null,
			bool allowBaseVoicebanks = true,
			[FromQuery(Name = "tagName[]")] string[]? tagName = null,
			[FromQuery(Name = "tagId[]")] int[]? tagId = null,
			bool childTags = false,
			int? followedByUserId = null,
			EntryStatus? status = null,
			[FromQuery(Name = "advancedFilters")] AdvancedSearchFilterParams[]? advancedFilters = null,
			int start = 0, int maxResults = DefaultMax, bool getTotalCount = false,
			ArtistSortRule sort = ArtistSortRule.Name,
			bool preferAccurateMatches = false,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			ArtistOptionalFields fields = ArtistOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default)
		{
			var textQuery = ArtistSearchTextQuery.Create(query, nameMatchMode);
			var types = EnumVal<ArtistType>.ParseMultiple(artistTypes);

			var param = new ArtistQueryParams(textQuery, types, start, Math.Min(maxResults, AbsoluteMax), getTotalCount, sort, preferAccurateMatches)
			{
				Tags = tagName,
				TagIds = tagId,
				ChildTags = childTags,
				UserFollowerId = followedByUserId ?? 0,
				AllowBaseVoicebanks = allowBaseVoicebanks,
				AdvancedFilters = advancedFilters?.Select(advancedFilter => advancedFilter.ToAdvancedSearchFilter()).ToArray(),
			};
			param.Common.EntryStatus = status;

			var artists = _service.FindArtists(s => new ArtistForApiContract(s, lang, _thumbPersister, fields), param);

			return artists;
		}
#nullable disable

		[HttpGet("ids")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public IEnumerable<int> GetIds() => _queries.GetIds();

		/// <summary>
		/// Gets a list of artist names. Ideal for autocomplete boxes.
		/// </summary>
		/// <param name="query">Text query.</param>
		/// <param name="nameMatchMode">Name match mode.</param>
		/// <param name="maxResults">Maximum number of results.</param>
		/// <returns>List of artist names.</returns>
		[HttpGet("names")]
		public string[] GetNames(string query = "", NameMatchMode nameMatchMode = NameMatchMode.Auto, int maxResults = 15) => _service.FindNames(ArtistSearchTextQuery.Create(query, nameMatchMode), maxResults);

		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpGet("{id:int}/tagSuggestions")]
		public IEnumerable<TagUsageForApiContract> GetTagSuggestions(int id) => _queries.GetTagSuggestions(id);

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
		/// <param name="id">ID of the artist for which to create the comment.</param>
		/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
		/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
		[HttpPost("{id:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int id, CommentForApiContract contract) => _queries.CreateComment(id, contract);
	}
}