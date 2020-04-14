using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.DataContracts.SongLists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.SongImport;
using VocaDb.Web.Code.Exceptions;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for song lists.
	/// </summary>
	[RoutePrefix("api/songLists")]
	public class SongListApiController : ApiController { 

		private const int absoluteMax = 100;
		private const int defaultMax = 10;
		private readonly SongListQueries queries;
		private readonly IUserIconFactory userIconFactory;
		private readonly IAggregatedEntryImageUrlFactory entryImagePersister;

		public SongListApiController(SongListQueries queries, IUserIconFactory userIconFactory, IAggregatedEntryImageUrlFactory entryImagePersister) {
			this.queries = queries;
			this.userIconFactory = userIconFactory;
			this.entryImagePersister = entryImagePersister;
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
		[Route("{id:int}")]
		[Authorize]
		public void Delete(int id, string notes = "", bool hardDelete = false) {

			notes = notes ?? string.Empty;

			if (hardDelete) {
				queries.MoveToTrash(id);
			} else {
				queries.Delete(id, notes);
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
		[Route("comments/{commentId:int}")]
		[Authorize]
		public void DeleteComment(int commentId) {

			queries.HandleTransaction(ctx => queries.Comments(ctx).Delete(commentId));

		}

		/// <summary>
		/// Gets a list of comments for a song list.
		/// </summary>
		/// <param name="listId">ID of the list whose comments to load.</param>
		/// <returns>List of comments in no particular order.</returns>
		[Route("{listId:int}/comments")]
		public PartialFindResult<CommentForApiContract> GetComments(int listId) {

			return new PartialFindResult<CommentForApiContract>(queries.GetComments(listId), 0);

		}

		[Route("{id:int}/for-edit")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public SongListForEditContract GetForEdit(int id) {
			
			return queries.GetSongListForEdit(id);

		}

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
		/// <returns>List of song lists.</returns>
		[Route("featured")]
		public PartialFindResult<SongListForApiContract> GetFeaturedLists(
			string query = "",
			[FromUri] int[] tagId = null,
			bool childTags = false,
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			SongListFeaturedCategory? featuredCategory = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			SongListSortRule sort = SongListSortRule.Name) {
			
			var textQuery = SearchTextQuery.Create(query, nameMatchMode);
			var queryParams = new SongListQueryParams {
				TextQuery = textQuery,
				FeaturedCategory = featuredCategory,
				Paging = new PagingProperties(start, maxResults, getTotalCount),
				SortRule = sort,
				TagIds = tagId,
				ChildTags = childTags
			};

			return queries.Find(s => new SongListForApiContract(s, userIconFactory, entryImagePersister, SongListOptionalFields.MainPicture), queryParams);

		}

		/// <summary>
		/// Gets a list of featuedd list names. Ideal for autocomplete boxes.
		/// </summary>
		/// <param name="query">Text query.</param>
		/// <param name="nameMatchMode">Name match mode. Words is treated the same as Partial.</param>
		/// <param name="featuredCategory">Filter by a specific featured category. If empty, all categories are returned.</param>
		/// <param name="maxResults">Maximum number of results.</param>
		/// <returns>List of list names.</returns>
		[Route("featured/names")]
		public IEnumerable<string> GetFeaturedListNames(string query = "", 
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			SongListFeaturedCategory? featuredCategory = null,
			int maxResults = 10) {

			var textQuery = SearchTextQuery.Create(query, nameMatchMode);

			return queries.HandleQuery(ctx => {

				return ctx.Query()
					.WhereNotDeleted()
					.WhereHasFeaturedCategory(featuredCategory, false)
					.WhereHasName(textQuery)
					.Select(l => l.Name)
					.OrderBy(n => n)
					.Take(maxResults)
					.ToArray();

			});

		}

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
		[Route("{listId:int}/songs")]
		public PartialFindResult<SongInListForApiContract> GetSongs(int listId,
			string query = "",
			string songTypes = null,
			[FromUri] PVServices? pvServices = null,
			[FromUri] int[] tagId = null,
			[FromUri] int[] artistId = null,
			bool childVoicebanks = false,
			[FromUri] AdvancedSearchFilter[] advancedFilters = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			SongSortRule? sort = null,
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			SongOptionalFields fields = SongOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default
			) {
			
			maxResults = Math.Min(maxResults, absoluteMax);
			var types = EnumVal<SongType>.ParseMultiple(songTypes);

			return queries.GetSongsInList(
				new SongInListQueryParams {
					TextQuery = SearchTextQuery.Create(query, nameMatchMode),
					ListId = listId, 
					Paging = new PagingProperties(start, maxResults, getTotalCount),
					PVServices = pvServices,
					ArtistIds = artistId,
					ChildVoicebanks = childVoicebanks,
					TagIds = tagId,
					SortRule = sort,
					AdvancedFilters = advancedFilters,
					SongTypes = types
				}, 
				songInList => new SongInListForApiContract(songInList, lang, fields));

		}

		[ApiExplorerSettings(IgnoreApi=true)]
		[Route("import")]
		public async Task<ImportedSongListContract> GetImport(string url, bool parseAll = true) {

			try {
				return await queries.Import(url, parseAll);
			} catch (UnableToImportException x) {
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = x.Message });				
			}

		}

		[ApiExplorerSettings(IgnoreApi=true)]
		[Route("import-songs")]
		public async Task<PartialImportedSongs> GetImportSongs(string url, string pageToken, int maxResults = 20, bool parseAll = true) {

			try {
				return await queries.ImportSongs(url, pageToken, maxResults, parseAll);
			} catch (UnableToImportException x) {
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = x.Message });
			}

		}

		/// <summary>
		/// Creates a song list.
		/// </summary>
		/// <param name="list">Song list properties.</param>
		/// <returns>ID of the created list.</returns>
		[Route("")]
		[Authorize]
		public int Post(SongListForEditContract list) {

			if (list == null)
				throw new HttpBadRequestException();

			return queries.UpdateSongList(list, null);

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
		[System.Web.Http.Route("comments/{commentId:int}")]
		[System.Web.Http.Authorize]
		public void PostEditComment(int commentId, CommentForApiContract contract) {

			queries.HandleTransaction(ctx => queries.Comments(ctx).Update(commentId, contract));

		}

		/// <summary>
		/// Posts a new comment.
		/// </summary>
		/// <param name="listId">ID of the song list for which to create the comment.</param>
		/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
		/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
		[Route("{listId:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int listId, CommentForApiContract contract) {

			return queries.CreateComment(listId, contract);

		}

	}

}