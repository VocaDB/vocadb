using System;
using System.Linq;
using System.Web.Http;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Web.Helpers;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for album release events.
	/// </summary>
	[RoutePrefix("api/releaseEvents")]
	public class ReleaseEventApiController : ApiController {

		private const int defaultMax = 10;
		private readonly EventQueries queries;
		private readonly IEventRepository repository;
		private readonly IEntryThumbPersister thumbPersister;

		public ReleaseEventApiController(EventQueries queries, IEventRepository repository, IEntryThumbPersister thumbPersister) {
			this.queries = queries;
			this.repository = repository;
			this.thumbPersister = thumbPersister;
		}

		/// <summary>
		/// Gets a list of albums for a specific release event.
		/// </summary>
		/// <param name="eventId">Release event ID.</param>
		/// <param name="fields">List of optional album fields.</param>
		/// <param name="lang">Content language preference.</param>
		/// <returns>List of albums.</returns>
		[Route("{eventId:int}/albums")]
		public AlbumForApiContract[] GetAlbums(int eventId, 
			AlbumOptionalFields fields = AlbumOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			return repository.HandleQuery(ctx => {
				
				var ev = ctx.Load(eventId);
				return ev.Albums.Select(a => new AlbumForApiContract(a, null, lang, thumbPersister, WebHelper.IsSSL(Request), fields, SongOptionalFields.None)).ToArray();

			});

		}

		/// <summary>
		/// Gets a page of release events.
		/// </summary>
		/// <param name="query">Event name query (optional).</param>
		/// <param name="nameMatchMode">Match mode for event name (optional, defaults to Auto).</param>
		/// <param name="seriesId">Filter by series Id.</param>
		/// <param name="afterDate">Filter by events after this date (inclusive).</param>
		/// <param name="beforeDate">Filter by events before this date (exclusive).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">
		/// Sort rule (optional, defaults to Name). 
		/// Possible values are None, Name, Date, SeriesName.
		/// </param>
		/// <param name="fields">
		/// Optional fields (optional). Possible values are Description, Series.
		/// </param>
		/// <returns>Page of events.</returns>
		/// <example>http://vocadb.net/api/releaseEvents?query=Voc@loid</example>
		[Route("")]
		public PartialFindResult<ReleaseEventForApiContract> GetList(
			string query = "",
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			int seriesId = 0,
			DateTime? afterDate = null,
			DateTime? beforeDate = null,
			int start = 0, 
			int maxResults = defaultMax,
			bool getTotalCount = false, 
			EventSortRule sort = EventSortRule.Name,
			ReleaseEventOptionalFields fields = ReleaseEventOptionalFields.None
			) {
			
			var textQuery = SearchTextQuery.Create(query, nameMatchMode);

			return queries.Find(e => new ReleaseEventForApiContract(e, fields, thumbPersister, WebHelper.IsSSL(Request)), textQuery, seriesId, afterDate, beforeDate, 
				start, maxResults, getTotalCount, sort);

		}

		/// <summary>
		/// Find event names by a part of name.
		/// 
		/// Matching is done anywhere from the name.
		/// </summary>
		/// <param name="query">Event name query, for example "Voc@loid".</param>
		/// <param name="maxResults">Maximum number of search results.</param>
		/// <returns>
		/// List of event names, for example "The Voc@loid M@ster 1", matching the query. Cannot be null.
		/// </returns>
		[Route("names")]
		public string[] GetNames(
			string query = "",
			int maxResults = 10) {
			
			var textQuery = SearchTextQuery.Create(query);

			return repository.HandleQuery(ctx => {

				return ctx.Query()
					.WhereHasName(textQuery)
					.OrderBy(r => r.Name)
					.Take(maxResults)
					.Select(r => r.Name)
					.ToArray();
				
			});

		}

		[Route("{id:int}")]
		public ReleaseEventForApiContract GetOne(int id, ReleaseEventOptionalFields fields) {

			return repository.HandleQuery(ctx => new ReleaseEventForApiContract(ctx.Load(id), fields, thumbPersister, WebHelper.IsSSL(Request)));

		}

	}

}