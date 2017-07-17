using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for PVs
	/// </summary>
	[RoutePrefix("api/pvs")]
	public class PVApiController : ApiController {

		private readonly IPVParser pvParser;
		private readonly IUserPermissionContext permissionContext;
		private readonly IRepository repository;

		public PVApiController(IPVParser pvParser, IUserPermissionContext permissionContext, IRepository repository) {
			this.pvParser = pvParser;
			this.permissionContext = permissionContext;
			this.repository = repository;
		}

		/// <summary>
		/// Gets a list of PVs for songs.
		/// </summary>
		/// <param name="author">Uploader name (optional).</param>
		/// <param name="maxResults">Maximum number of results.</param>
		/// <returns>List of PVs.</returns>
		[Route("for-songs")]
		public IEnumerable<PVContract> GetList(string author = null, int maxResults = 10) {

			return repository.HandleQuery(db => {

				var query = db.Query<PVForSong>();

				if (!string.IsNullOrEmpty(author)) {
					query = query.Where(p => p.Author == author);
				}

				query = query.Take(maxResults);

				var results = query.Select(p => new PVContract(p)).ToArray();
				return results;

			});

		}

		[Route("")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public PVContract GetPVByUrl(string pvUrl, PVType type = PVType.Original) {

			if (string.IsNullOrEmpty(pvUrl))
				throw new HttpResponseException(HttpStatusCode.BadRequest);

			var result = pvParser.ParseByUrl(pvUrl, true, permissionContext);

			if (!result.IsOk) {
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = result.Exception.Message });
			}

			var contract = new PVContract(result, type);
			return contract;

		}

	}

}