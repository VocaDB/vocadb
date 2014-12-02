using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace VocaDb.Web.Controllers.Api {

	[RoutePrefix("api")]
	public class ApiRootController : ApiController {

		[Route("")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public RedirectToRouteResult Get() {
			
			return RedirectToRoute("HelpPage_Default", new {});

		}

	}

}