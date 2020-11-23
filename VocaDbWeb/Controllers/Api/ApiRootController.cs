using System;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace VocaDb.Web.Controllers.Api
{

	[System.Web.Http.RoutePrefix("api")]
	public class ApiRootController : ApiController
	{

		[System.Web.Http.Route("")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public RedirectResult Get() => Redirect(new Uri("/swagger", UriKind.Relative));

	}

}