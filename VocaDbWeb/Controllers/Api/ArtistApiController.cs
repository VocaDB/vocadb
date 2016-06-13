using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
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
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for artists.
	/// </summary>
	[RoutePrefix("api/artists")]
	public class ArtistApiController : ApiController {

		private const int absoluteMax = 50;
		private const int defaultMax = 10;
		private readonly ArtistQueries queries;
		private readonly ArtistService service;
		private readonly IEntryThumbPersister thumbPersister;

		public ArtistApiController(ArtistQueries queries, ArtistService service, IEntryThumbPersister thumbPersister) {
			this.queries = queries;
			this.service = service;
			this.thumbPersister = thumbPersister;
		}

		private ArtistForApiContract GetArtist(Artist a, ArtistMergeRecord m, 
			ArtistOptionalFields fields, 
			ArtistRelationsFields relations,
			ContentLanguagePreference lang,
			IDatabaseContext<Artist> ctx) {
			
			var contract = new ArtistForApiContract(a, lang, thumbPersister, WebHelper.IsSSL(Request), fields);

			if (relations != ArtistRelationsFields.None) {
				contract.Relations = new ArtistRelationsQuery(ctx, lang).GetRelations(a, relations);
			}

			return contract;

		}

		/// <summary>
		/// Deletes an artist.
		/// </summary>
		/// <param name="artistId">ID of the artist to be deleted.</param>
		/// <param name="notes">Notes.</param>
		[Route("{artistId:int}")]
		[Authorize]
		public void Delete(int artistId, string notes = "") {
			
			service.Delete(artistId, notes ?? string.Empty);

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
		/// Gets a list of comments for an artist.
		/// Note: pagination and sorting might be added later.
		/// </summary>
		/// <param name="artistId">ID of the artist whose comments to load.</param>
		/// <returns>List of comments in no particular order.</returns>
		[Route("{artistId:int}/comments")]
		public IEnumerable<CommentForApiContract> GetComments(int artistId) {
			
			return queries.GetComments(artistId);

		}

		[Route("{id:int}/for-edit")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public ArtistForEditContract GetForEdit(int id) {
			
			return service.GetArtistForEdit(id);

		}

		/// <summary>
		/// Gets an artist by Id.
		/// </summary>
		/// <param name="id">Artist ID (required).</param>
		/// <param name="fields">List of optional fields (optional). Possible values are Description, Groups, Members, Names, Tags, WebLinks.</param>
		/// <param name="relations">List of artist relations (optional). Possible values are LatestAlbums, PopularAlbums, LatestSongs, PopularSongs, All</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Artist data.</returns>
		/// <example>http://vocadb.net/api/artists/1</example>
		[Route("{id:int}")]
		public ArtistForApiContract GetOne(int id,
			ArtistOptionalFields fields = ArtistOptionalFields.None,
			ArtistRelationsFields relations = ArtistRelationsFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			var artist = queries.GetWithMergeRecord(id, (a, m, ctx) => GetArtist(a, m, fields, relations, lang, ctx));

			return artist;

		}

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
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
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
		[Route("")]
		public PartialFindResult<ArtistForApiContract> GetList(
			string query = "", 
			string artistTypes = null,
			bool allowBaseVoicebanks = true,
			[FromUri] string[] tagName = null,
			[FromUri] int[] tagId = null,
			bool childTags = false,
			int? followedByUserId = null,
			EntryStatus? status = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			ArtistSortRule sort = ArtistSortRule.Name,
			bool preferAccurateMatches = false,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			ArtistOptionalFields fields = ArtistOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			var textQuery = ArtistSearchTextQuery.Create(query, nameMatchMode);
			var types = EnumVal<ArtistType>.ParseMultiple(artistTypes);

			var param = new ArtistQueryParams(textQuery, types, start, Math.Min(maxResults, absoluteMax), getTotalCount, sort, preferAccurateMatches) {
				Tags = tagName,
				TagIds = tagId,
				ChildTags = childTags,
				UserFollowerId = followedByUserId ?? 0,
				AllowBaseVoicebanks = allowBaseVoicebanks
			};
			param.Common.EntryStatus = status;

			var ssl = WebHelper.IsSSL(Request);
			var artists = service.FindArtists(s => new ArtistForApiContract(s, lang, thumbPersister, ssl, fields), param);

			return artists;

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

		/// <summary>
		/// Gets a list of artist names. Ideal for autocomplete boxes.
		/// </summary>
		/// <param name="query">Text query.</param>
		/// <param name="nameMatchMode">Name match mode.</param>
		/// <param name="maxResults">Maximum number of results.</param>
		/// <returns>List of artist names.</returns>
		[Route("names")]
		public string[] GetNames(string query = "", NameMatchMode nameMatchMode = NameMatchMode.Auto, int maxResults = 15) {
			
			return service.FindNames(ArtistSearchTextQuery.Create(query, nameMatchMode), maxResults);

		}

		[ApiExplorerSettings(IgnoreApi = true)]
		[Route("{id:int}/tagSuggestions")]
		public IEnumerable<TagUsageForApiContract> GetTagSuggestions(int id) {
			return queries.GetTagSuggestions(id);
		}

		[Route("versions")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public EntryIdAndVersionContract[] GetVersions() {

			var versions = queries
				.HandleQuery(ctx => ctx.Query()
					.Where(a => !a.Deleted)
					.Select(a => new EntryIdAndVersionContract { Id = a.Id, Version = a.Version })
					.ToArray());

			return versions;

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
		/// <param name="artistId">ID of the artist for which to create the comment.</param>
		/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
		/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
		[Route("{artistId:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int artistId, CommentForApiContract contract) {
			
			return queries.CreateComment(artistId, contract);

		}

	}

}