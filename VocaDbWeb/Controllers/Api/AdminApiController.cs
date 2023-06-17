using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Security;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Code.Security;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api;

[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/admin")]
[ApiController]
public class AdminApiController : ApiController
{
	private readonly IPRuleManager _ipRuleManager;
	private readonly IUserPermissionContext _userContext;
	private readonly AdminService _adminService;

	public AdminApiController(
		IUserPermissionContext userContext,
		IPRuleManager ipRuleManager,
		AdminService adminService
	)
	{
		_userContext = userContext;
		_ipRuleManager = ipRuleManager;
		_adminService = adminService;
	}

	[HttpGet("tempBannedIPs")]
	public string[] GetTempBannedIPs()
	{
		_userContext.VerifyPermission(PermissionToken.ManageIPRules);

		var hosts = _ipRuleManager.TempBannedIPs.Hosts;
		return hosts.ToArray();
	}

	[HttpGet("audit-logs")]
	public AuditLogEntryForApiContract[] GetAuditLogEntries(
		string excludeUsers,
		string filter,
		AuditLogUserGroupFilter groupId,
		bool onlyNewUsers,
		string userName,
		int start = 0
	)
	{
		_userContext.VerifyPermission(PermissionToken.ViewAuditLog);

		var cutoffDays = (string.IsNullOrEmpty(userName) ? 365 : 0);

		return _adminService.GetAuditLog(
			filter,
			start,
			200,
			cutoffDays,
			userName,
			excludeUsers: !string.IsNullOrEmpty(excludeUsers)
				? excludeUsers.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
					.Select(u => u.Trim())
					.ToArray()
				: Array.Empty<string>(),
			onlyNewUsers,
			groupId
		);
	}

	[HttpGet("reports")]
	public EntryReportForApiContract[] GetEntryReports(ReportStatus status) =>
		_adminService.GetEntryReports(status);

	[HttpDelete("reports/{id:int}")]
	[OriginHeaderCheck]
	public ActionResult DeleteEntryReport(int id)
	{
		_userContext.VerifyPermission(PermissionToken.ManageEntryReports);

		_adminService.DeleteEntryReports(new[] { id });

		return NoContent();
	}

	[HttpGet("activeEditors")]
	public ActiveEditorForApiContract[] GetActiveEditors() => _adminService.GetActiveEditors();

	[HttpGet("pvsByAuthor")]
	public PVForSongContract[] GetPVsByAuthor(string author, int maxResults = 50) => _adminService.GetSongPVsByAuthor(author ?? string.Empty, maxResults);

	[HttpDelete("pvsByAuthor/{author}")]
	public ActionResult DeletePVsByAuthor(string author)
	{

		var count = _adminService.DeletePVsByAuthor(author, PVService.Youtube);

		return NoContent();
	}
}
