using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
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
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;

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
		private readonly IEntryImagePersisterOld entryImagePersister;

		public SongListApiController(SongListQueries queries, IUserIconFactory userIconFactory, IEntryImagePersisterOld entryImagePersister) {
			this.queries = queries;
			this.userIconFactory = userIconFactory;
			this.entryImagePersister = entryImagePersister;
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
		/// <param name="nameMatchMode">Match mode for song name (optional, defaults to Auto).</param>
		/// <param name="featuredCategory">Filter by a specific featured category. If empty, all categories are returned.</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">List sort rule. Possible values are Nothing, Date, CreateDate, Name.</param>
		/// <returns>List of song lists.</returns>
		[Route("featured")]
		public PartialFindResult<SongListForApiContract> GetFeaturedLists(
			string query = "",
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			SongListFeaturedCategory? featuredCategory = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			SongListSortRule sort = SongListSortRule.Name) {
			
			var ssl = WebHelper.IsSSL(Request);
			var textQuery = SearchTextQuery.Create(query, nameMatchMode);

			return queries.HandleQuery(ctx => {
				
				var listQuery = ctx.Query()
					.WhereHasFeaturedCategory(featuredCategory, false)
					.WhereHasName(textQuery);

				var count = getTotalCount ? query.Count() : 0;

				return new PartialFindResult<SongListForApiContract>(listQuery
					.OrderBy(sort)
					.Paged(new PagingProperties(start, maxResults, getTotalCount))
					.ToArray()
					.Select(s => new SongListForApiContract(s, userIconFactory, entryImagePersister, ssl, SongListOptionalFields.MainPicture))
					.ToArray(), count);

			});

		}

		/// <summary>
		/// Gets a list of songs in a song list.
		/// </summary>
		/// <param name="listId">ID of the song list.</param>
		/// <param name="pvServices">Filter by one or more PV services (separated by commas). The song will pass the filter if it has a PV for any of the matched services.</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">Song sort rule (optional, by default songs are sorted by song list order).</param>
		/// <param name="fields">
		/// List of optional fields (optional). Possible values are Albums, Artists, Names, PVs, Tags, ThumbUrl, WebLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of songs.</returns>
		[Route("{listId:int}/songs")]
		public PartialFindResult<SongInListForApiContract> GetSongs(int listId, 
			[FromUri] PVServices? pvServices = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			SongSortRule? sort = null,
			SongOptionalFields fields = SongOptionalFields.None, ContentLanguagePreference lang = ContentLanguagePreference.Default
			) {
			
			maxResults = Math.Min(maxResults, absoluteMax);

			return queries.GetSongsInList(
				new SongListQueryParams {
					ListId = listId, 
					Paging = new PagingProperties(start, maxResults, getTotalCount),
					PVServices = pvServices,
					SortRule = sort
				}, 
				songInList => new SongInListForApiContract(songInList, lang, fields));

		}

		[ApiExplorerSettings(IgnoreApi=true)]
		[Route("import")]
		public ImportedSongListContract GetImport(string url, bool parseAll = true) {

			try {
				return queries.Import(url, parseAll);
			} catch (UnableToImportException x) {
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = x.Message });				
			}

		}

		[ApiExplorerSettings(IgnoreApi=true)]
		[Route("import-songs")]
		public PartialImportedSongs GetImportSongs(string url, string pageToken, int maxResults = 20, bool parseAll = true) {

			try {
				return queries.ImportSongs(url, pageToken, maxResults, parseAll);
			} catch (UnableToImportException x) {
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = x.Message });
			}

		}

		[Route("")]
		public int Post(SongListForEditContract list) {
			
			return queries.UpdateSongList(list, null);

		}

	}

}