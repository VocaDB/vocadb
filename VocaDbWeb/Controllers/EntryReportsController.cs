using System.Web.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Controllers
{

	/// <summary>
	/// Controller for <see cref="EntryReport"/>.
	/// </summary>
    public class EntryReportsController : Controller
    {

		private readonly EntryReportQueries queries;

		public EntryReportsController(EntryReportQueries queries) {
			this.queries = queries;
		}
        
		[Authorize]
		public int NewReportsCount() {

			return queries.GetNewReportsCount();

		}

    }
}
