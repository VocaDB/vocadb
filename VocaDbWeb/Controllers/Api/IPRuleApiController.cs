#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Code.Security;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api;

/// <summary>
/// Manages <see cref="IPRule"/>s
/// </summary>
[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/ip-rules")]
[ApiController]
public class IPRuleApiController : ApiController
{
	private readonly IPRuleManager _ipRuleManager;
	private readonly IRepository _repo;
	private readonly IUserPermissionContext _userContext;
	private readonly AdminService _adminService;

	public IPRuleApiController(
		IUserPermissionContext userContext,
		IRepository repo,
		IPRuleManager ipRuleManager,
		AdminService adminService
	)
	{
		_userContext = userContext;
		_repo = repo;
		_ipRuleManager = ipRuleManager;
		_adminService = adminService;
	}

	[HttpDelete("{id:int}")]
	public async Task DeleteIPRule(int ruleID)
	{
		_userContext.VerifyPermission(PermissionToken.ManageIPRules);

		await _repo.HandleTransactionAsync(async ctx =>
		{
			var rule = await ctx.LoadAsync<IPRule>(ruleID);
			await ctx.DeleteAsync(rule);
			_ipRuleManager.RemovePermBannedIP(rule.Address);
			ctx.AuditLogger.SysLog($"removed {rule.Address} from banned IPs");
		});
	}

	[HttpGet("")]
	public async Task<IEnumerable<IPRule>> GetIPRules()
	{
		_userContext.VerifyPermission(PermissionToken.ManageIPRules);

		return await _repo.HandleQueryAsync(async ctx =>
		{
			return await ctx.Query<IPRule>().ToListAsync();
		});
	}

	[HttpPost("")]
	public ActionResult<bool> PostNewIPRule(IPRule rule)
	{
		_userContext.VerifyPermission(PermissionToken.ManageIPRules);

		if (string.IsNullOrEmpty(rule?.Address))
			return BadRequest();

		bool result = false;

		_repo.HandleTransaction(ctx =>
		{
			result = _ipRuleManager.AddPermBannedIP(ctx, rule);
			ctx.AuditLogger.SysLog($"added {rule.Address} to banned IPs");
		});

		return result;
	}

	[Authorize]
	[HttpPut("")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public IActionResult PutIPRules([FromBody] IEnumerable<IPRule> rules)
	{
		_userContext.VerifyPermission(PermissionToken.ManageIPRules);

		_adminService.UpdateIPRules(rules.ToArray());
		_ipRuleManager.Reset(rules.Select(i => i.Address));

		return NoContent();
	}
}