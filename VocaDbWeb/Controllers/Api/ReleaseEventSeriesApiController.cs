using System.Web.Http;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search;

namespace VocaDb.Web.Controllers.Api {

	[RoutePrefix("api/releaseEventSeries")]
	public class ReleaseEventSeriesApiController : ApiController {

		private readonly EventQueries queries;

		public ReleaseEventSeriesApiController(EventQueries queries) {
			this.queries = queries;
		}

		[Route("")]
		public PartialFindResult<ReleaseEventSeriesContract> GetList(string query = "") {

			return queries.FindSeries(s => new ReleaseEventSeriesContract(s), SearchTextQuery.Create(query), PagingProperties.CreateFromPage(0, 10, false));

		}

	}

}