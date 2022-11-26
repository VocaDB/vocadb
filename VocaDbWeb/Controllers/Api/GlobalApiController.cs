using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Utils.Config;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Models.Shared;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api;

[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
[Route("api/globals")]
[ApiController]
public class GlobalApiController : ApiController
{
	private readonly BrandableStringsManager _brandableStrings;
	private readonly VdbConfigManager _config;
	private readonly IUserPermissionContext _userContext;

	public GlobalApiController(
		BrandableStringsManager brandableStrings,
		VdbConfigManager config,
		IUserPermissionContext userContext
	)
	{
		_brandableStrings = brandableStrings;
		_config = config;
		_userContext = userContext;
	}

	[HttpGet("resources")]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ApiExplorerSettings(IgnoreApi = true)]
	public GlobalResources GetResources()
	{
		return new GlobalResources(_brandableStrings);
	}

	[HttpGet("values")]
	[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
	[ApiExplorerSettings(IgnoreApi = true)]
	public GlobalValues GetValues()
	{
		return new GlobalValues(_brandableStrings, _config, _userContext);
	}
}
