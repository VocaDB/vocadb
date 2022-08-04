using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Web.Code.Security;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api;

[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
[Route("api/antiforgery")]
[ApiController]
public class AntiforgeryApiController : ApiController
{
	private readonly IAntiforgery _antiforgery;

	public AntiforgeryApiController(IAntiforgery antiforgery)
	{
		_antiforgery = antiforgery;
	}

	[HttpGet("token")]
	[Authorize]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[IgnoreAntiforgeryToken]
	[ApiExplorerSettings(IgnoreApi = true)]
	public IActionResult GetToken()
	{
		// Code from: https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-6.0#javascript-1.
		var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
		Response.Cookies.Append("XSRF-TOKEN", tokenSet.RequestToken!, new CookieOptions { HttpOnly = false });

		return NoContent();
	}
}
