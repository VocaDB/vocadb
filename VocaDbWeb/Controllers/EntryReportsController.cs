#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Controllers
{
	/// <summary>
	/// Controller for <see cref="EntryReport"/>.
	/// </summary>
	public class EntryReportsController : Controller
	{
		private readonly EntryReportQueries _queries;

		public EntryReportsController(EntryReportQueries queries)
		{
			_queries = queries;
		}

		[Authorize]
		public int NewReportsCount()
		{
			return _queries.GetNewReportsCount();
		}
	}
}
