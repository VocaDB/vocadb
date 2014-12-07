using System.Web.Mvc;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Models.Event;

namespace VocaDb.Web.Controllers
{
    public class EventController : ControllerBase
    {

		private readonly EventQueries queries;
		private readonly ReleaseEventService service;

		private ReleaseEventService Service {
			get { return service; }
		}

		public EventController(EventQueries queries, ReleaseEventService service) {
			this.queries = queries;
			this.service = service;
		}

		[HttpPost]
		public PartialViewResult AliasForSeries(string name) {

			return PartialView("AliasForSeries", name);

		}

		public ActionResult Delete(int id) {

			Service.DeleteEvent(id);

			return RedirectToAction("EventsBySeries");

		}

		public ActionResult DeleteSeries(int id) {

			Service.DeleteSeries(id);

			return RedirectToAction("EventsBySeries");

		}

		public ActionResult Details(int id = invalidId) {

		    if (id == invalidId)
		        return NoId();

			var ev = Service.GetReleaseEventDetails(id);
			return View(ev);

		}

        [Authorize]
        public ActionResult Edit(int? id, int? seriesId)
        {

			var model = (id != null ? new EventEdit(Service.GetReleaseEventForEdit(id.Value)) 
				: new EventEdit(seriesId != null ? Service.GetReleaseEventSeriesForEdit(seriesId.Value): null));

			return View(model);

		}

		[HttpPost]
        [Authorize]
        public ActionResult Edit(EventEdit model)
        {

			// Either series or name must be specified. If series is specified, name is generated automatically.
			if (!model.SeriesId.HasValue && string.IsNullOrEmpty(model.Name)) {
				ModelState.AddModelError("Name", "Name cannot be empty");
			}

			if (!ModelState.IsValid) {

				if (model.Id != 0) {
					var contract = Service.GetReleaseEventForEdit(model.Id);
					model.CopyNonEditableProperties(contract);					
				}

				return View(model);
			}

			var id = queries.Update(model.ToContract()).Id;

			return RedirectToAction("Details", new { id });

		}

        [Authorize]
        public ActionResult EditSeries(int? id)
        {

			var contract = (id != null ? Service.GetReleaseEventSeriesForEdit(id.Value) : new ReleaseEventSeriesForEditContract());
			return View(new SeriesEdit(contract));

		}

		[HttpPost]
        [Authorize]
        public ActionResult EditSeries(SeriesEdit model)
        {

			if (!ModelState.IsValid) {
				return View(model);
			}

			Service.UpdateSeries(model.ToContract());

			return RedirectToAction("EventsBySeries");

		}

		public ActionResult EventsByDate() {
			
			return View(queries.List(EventSortRule.Date));

		}

		public ActionResult EventsBySeries() {

			var events = Service.GetReleaseEventsBySeries();
			return View(events);

		}

		public ActionResult Find(string query) {

			var result = Service.Find(query);

			if (result.EventId != 0) {

				if (result.EventName != query && PermissionContext.HasPermission(PermissionToken.ManageDatabase))
					Services.Albums.UpdateAllReleaseEventNames(query, result.EventName);

				return RedirectToAction("Details", new { id = result.EventId });

			}

			return View(result);

		}

		[HttpPost]
		public ActionResult Find(ReleaseEventFindResultContract model, string query, string EventTarget) {

			bool skipSeries = false;

			if (EventTarget != "Series") {

				skipSeries = true;

				if (string.IsNullOrEmpty(model.EventName))
					ModelState.AddModelError("EventName", "Name must be specified");

			}

			if (!ModelState.IsValid) {
				return View(model);
			}

			var contract = new ReleaseEventDetailsContract {
				Name = model.EventName, 
				Series = (skipSeries ? null : model.Series), 
				SeriesNumber = model.SeriesNumber,
				SeriesSuffix = string.Empty	// TODO: add support for entering this
			};

			var ev = queries.Update(contract);

			return RedirectToAction("Edit", new { id = ev.Id });

		}

        //
        // GET: /Event/

        public ActionResult Index(EventSortRule sortRule = EventSortRule.Date)
        {

			ViewBag.SortRule = sortRule;

			return View(queries.List(sortRule, true));
        }

		public ActionResult SeriesDetails(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var series = Service.GetReleaseEventSeriesDetails(id);
			return View(series);

		}

		public ActionResult Versions(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var contract = Service.GetReleaseEventWithArchivedVersions(id);

			return View(contract);

		}

    }
}
