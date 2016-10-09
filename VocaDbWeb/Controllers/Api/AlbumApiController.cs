using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for albums.
	/// </summary>
	[RoutePrefix("api/albums")]
	public class AlbumApiController : ApiController {

		private const int absoluteMax = 50;
		private const int defaultMax = 10;
		private readonly IEntryThumbPersister thumbPersister;
		private readonly AlbumQueries queries;
		private readonly AlbumService service;

		public AlbumApiController(AlbumQueries queries, AlbumService service, IEntryThumbPersister thumbPersister) {			
			this.queries = queries;
			this.service = service;
			this.thumbPersister = thumbPersister;
		}

		/// <summary>
		/// Deletes an album.
		/// </summary>
		/// <param name="id">ID of the album to be deleted.</param>
		/// <param name="notes">Notes.</param>
		[Route("{id:int}")]
		[Authorize]
		public void Delete(int id, string notes = "") {
			
			service.Delete(id, notes ?? string.Empty);

		}

		/// <summary>
		/// Deletes a comment.
		/// </summary>
		/// <param name="commentId">ID of the comment to be deleted.</param>
		/// <remarks>
		/// Normal users can delete their own comments, moderators can delete all comments.
		/// Requires login.
		/// </remarks>
		[Route("comments/{commentId:int}")]
		[Authorize]
		public void DeleteComment(int commentId) {
			
			queries.HandleTransaction(ctx => queries.Comments(ctx).Delete(commentId));

		}

		/// <summary>
		/// Gets a list of comments for an album.
		/// </summary>
		/// <param name="id">ID of the album whose comments to load.</param>
		/// <returns>List of comments in no particular order.</returns>
		/// <remarks>
		/// Pagination and sorting might be added later.
		/// </remarks>
		[Route("{id:int}/comments")]
		public IEnumerable<CommentForApiContract> GetComments(int id) {
			
			return queries.GetComments(id);

		}

		[Route("{id:int}/for-edit")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public AlbumForEditContract GetForEdit(int id) {
			
			return service.GetAlbumForEdit(id);

		}

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
		/// <example>http://vocadb.net/api/albums/1</example>
		/// <returns>Album data.</returns>
		[Route("{id:int}")]
		public AlbumForApiContract GetOne(
			int id, 
			AlbumOptionalFields fields = AlbumOptionalFields.None, 
			SongOptionalFields songFields = SongOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			var ssl = WebHelper.IsSSL(Request);
			var album = service.GetAlbumWithMergeRecord(id, (a, m) => new AlbumForApiContract(a, m, lang, thumbPersister, ssl, fields, songFields));

			return album;

		}

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
		/// <param name="deleted">Whether to search for deleted entries. If this is true, only deleted entries will be returned.</param>
		/// <param name="nameMatchMode">Match mode for artist name (optional, defaults to Exact).</param>
		/// <param name="fields">
		/// Optional fields (optional). Possible values are artists, names, pvs, tags, tracks, webLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of albums.</returns>
		/// <example>http://vocadb.net/api/albums?query=Synthesis&amp;discTypes=Album</example>
		[Route("")]
		public PartialFindResult<AlbumForApiContract> GetList(
			string query = "", 
			DiscType discTypes = DiscType.Unknown,
			[FromUri] string[] tagName = null,
			[FromUri] int[] tagId = null,
			bool childTags = false,
			[FromUri] int[] artistId = null,
			ArtistAlbumParticipationStatus artistParticipationStatus = ArtistAlbumParticipationStatus.Everything,
			bool childVoicebanks = false,
			string barcode = null,
			EntryStatus? status = null,
			DateTime? releaseDateAfter = null,
			DateTime? releaseDateBefore = null,
			[FromUri] AdvancedSearchFilter[] advancedFilters = null,
			int start = 0, 
			int maxResults = defaultMax,
			bool getTotalCount = false, 
			AlbumSortRule? sort = null,
			bool preferAccurateMatches = false,
			bool deleted = false,
			NameMatchMode nameMatchMode = NameMatchMode.Exact, 
			AlbumOptionalFields fields = AlbumOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			var textQuery = SearchTextQuery.Create(query, nameMatchMode);

			var queryParams = new AlbumQueryParams(textQuery, discTypes, start, Math.Min(maxResults, absoluteMax), getTotalCount, sort ?? AlbumSortRule.Name, preferAccurateMatches) {
				Tags = tagName,
				TagIds = tagId,
				ChildTags = childTags,
				ArtistIds = artistId,
				ArtistParticipationStatus = artistParticipationStatus,
				ChildVoicebanks = childVoicebanks,
				Barcode = barcode,
				Deleted = deleted,
				ReleaseDateAfter = releaseDateAfter,
				ReleaseDateBefore = releaseDateBefore,
				AdvancedFilters = advancedFilters
			};
			queryParams.Common.EntryStatus = status;

			var ssl = WebHelper.IsSSL(Request);

			var entries = service.Find(a => new AlbumForApiContract(a, null, lang, thumbPersister, ssl, fields, SongOptionalFields.None), queryParams);
			
			return entries;

		}

		/// <summary>
		/// Gets a list of album names. Ideal for autocomplete boxes.
		/// </summary>
		/// <param name="query">Text query.</param>
		/// <param name="nameMatchMode">Name match mode.</param>
		/// <param name="maxResults">Maximum number of results.</param>
		/// <returns>List of album names.</returns>
		[Route("names")]
		public string[] GetNames(string query = "", NameMatchMode nameMatchMode = NameMatchMode.Auto, int maxResults = 15) {
			
			return service.FindNames(SearchTextQuery.Create(query, nameMatchMode), maxResults);

		}

		[ApiExplorerSettings(IgnoreApi = true)]
		[Route("{id:int}/tagSuggestions")]
		public IEnumerable<TagUsageForApiContract> GetTagSuggestions(int id) {
			return queries.GetTagSuggestions(id);
		}

		/// <summary>
		/// Gets tracks for an album.
		/// </summary>
		/// <param name="id">Album ID (required).</param>
		/// <param name="fields">
		/// List of optional fields (optional). Possible values are Albums, Artists, Names, PVs, Tags, ThumbUrl, WebLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>List of tracks for the album.</returns>
		/// <example>http://vocadb.net/api/albums/1/tracks</example>
		[Route("{id:int}/tracks")]
		public SongInAlbumForApiContract[] GetTracks(
			int id, 
			SongOptionalFields fields = SongOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			var tracks = service.GetAlbum(id, a => a.Songs.Select(s => new SongInAlbumForApiContract(s, lang, fields)).ToArray());

			return tracks;

		}

		/// <summary>
		/// Gets tracks for an album formatted using the CSV format string.
		/// </summary>
		/// <param name="id">Album ID.</param>
		/// <param name="field">Field to be included, for example "featvocalists" or "url". Can be specified multiple times.</param>
		/// <param name="lang">Language preference.</param>
		/// <returns>List of songs with the specified fields.</returns>
		[Route("{id:int}/tracks/fields")]
		public IEnumerable<Dictionary<string, string>> GetTracksFormatted(int id, [FromUri] string[] field = null, ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			return queries.GetTracksFormatted(id, field, lang);

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
		/// Gets a complete list of album versions and Ids.
		/// Intended for integration to other systems.
		/// </summary>
		/// <returns>List of album IDs with versions.</returns>
		[Route("versions")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public EntryIdAndVersionContract[] GetVersions() {

			var versions = queries
				.HandleQuery(ctx => ctx.Query()
					.Where(a => !a.Deleted)
					.Select(a => new { a.Id, a.Version })
					.ToArray()
					.Select(v => new EntryIdAndVersionContract(v.Id, v.Version))
					.ToArray());

			return versions;

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
		[Route("comments/{commentId:int}")]
		[Authorize]
		public void PostEditComment(int commentId, CommentForApiContract contract) {
			
			queries.HandleTransaction(ctx => queries.Comments(ctx).Update(commentId, contract));

		}

		/// <summary>
		/// Posts a new comment.
		/// </summary>
		/// <param name="id">ID of the album for which to create the comment.</param>
		/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
		/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
		[Route("{id:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int id, CommentForApiContract contract) {
			
			return queries.CreateComment(id, contract);

		}

	}

}