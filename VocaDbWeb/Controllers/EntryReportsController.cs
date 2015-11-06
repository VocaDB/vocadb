using System.Web.Mvc;
using VocaDb.Model.Domain;
using VocaDb.Model.Queries;

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
