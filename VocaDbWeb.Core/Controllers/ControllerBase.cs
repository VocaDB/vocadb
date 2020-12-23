#nullable disable

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Utils;
using VocaDb.Web.Code;

namespace VocaDb.Web.Controllers
{
	// TODO: implement
	public class ControllerBase : Controller
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		protected static readonly TimeSpan ImageExpirationTime = TimeSpan.FromMinutes(5);
		protected const int EntriesPerPage = 30;
		protected const int InvalidId = 0;
		protected static readonly TimeSpan PictureCacheDuration = TimeSpan.FromDays(30);
		protected const int PictureCacheDurationSec = 30 * 24 * 60 * 60;
		protected const int StatsCacheDurationSec = 24 * 60 * 60;

		protected ControllerBase()
		{
			PageProperties.OpenGraph.Image = VocaUriBuilder.StaticResource("/img/vocaDB-title-large.png");
		}

		protected PagePropertiesData PageProperties => PagePropertiesData.Get(ViewBag);

		protected IUserPermissionContext PermissionContext => HttpContext.RequestServices.GetRequiredService<IUserPermissionContext>();

		protected ActionResult NoId()
		{
			return NotFound("No ID specified");
		}

		protected void SetSearchEntryType(EntryType entryType)
		{
			PageProperties.GlobalSearchType = entryType;
		}
	}
}