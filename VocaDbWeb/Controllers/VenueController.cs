using System.Web.Mvc;
using VocaDb.Model.Database.Queries;

namespace VocaDb.Web.Controllers {

	public class VenueController : ControllerBase {

		private readonly VenueQueries queries;

		public VenueController(VenueQueries queries) {

			this.queries = queries;

		}

		public ActionResult Details(int id = invalidId) {

			return View();

		}

		[Authorize]
		public ActionResult Edit(int? id) {

			return View();

		}

		public ActionResult Versions(int id = invalidId) {

			return View();

		}

		public ActionResult ViewVersion(int id, int? ComparedVersionId) {

			return View();

		}

	}

}