using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Web.Controllers.Api {

	[RoutePrefix("api/pvs")]
	[ApiExplorerSettings(IgnoreApi=true)]
	public class PVApiController : ApiController {

		private readonly IPVParser pvParser;
		private readonly IUserPermissionContext permissionContext;

		public PVApiController(IPVParser pvParser, IUserPermissionContext permissionContext) {
			this.pvParser = pvParser;
			this.permissionContext = permissionContext;
		}

		[Route("")]
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