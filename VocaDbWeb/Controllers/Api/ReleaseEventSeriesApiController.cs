using System.Web.Http;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for album release event series.
	/// </summary>
	[RoutePrefix("api/releaseEventSeries")]
	public class ReleaseEventSeriesApiController : ApiController {

		private const int defaultMax = 10;
		private readonly EventQueries queries;

		public ReleaseEventSeriesApiController(EventQueries queries) {
			this.queries = queries;
		}

		[Route("")]
		public PartialFindResult<ReleaseEventSeriesContract> GetList(
			string query = "", 
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			return queries.FindSeries(s => new ReleaseEventSeriesContract(s, lang), SearchTextQuery.Create(query, nameMatchMode), 
				new PagingProperties(start, maxResults, getTotalCount));

		}

	}

}