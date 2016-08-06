using System;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Translations;
using VocaDb.Web.Models.Event;

namespace VocaDb.Web.Controllers
{
    public class EventController : ControllerBase
    {

		private readonly AlbumService albumService;
		private readonly IEnumTranslations enumTranslations;
		private readonly EventQueries queries;
		private readonly ReleaseEventService service;

		private ReleaseEventService Service {
			get { return service; }
		}

		public EventController(EventQueries queries, ReleaseEventService service, AlbumService albumService, IEnumTranslations enumTranslations) {
			this.queries = queries;
			this.service = service;
			this.albumService = albumService;
			this.enumTranslations = enumTranslations;
		}

		[HttpPost]
		public PartialViewResult AliasForSeries(string name) {

			return PartialView("AliasForSeries", name);

		}

		public ActionResult Delete(int id) {

			queries.Delete(id);

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

			PageProperties.PageTitle = string.Format("{0} ({1})", ev.Name, ViewRes.Event.DetailsStrings.Event);
			PageProperties.Title = ev.Name;
			PageProperties.Subtitle = ViewRes.Event.DetailsStrings.Event;

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
			if (string.IsNullOrEmpty(model.Name) && (model.Series.IsNullOrDefault() || model.CustomName)) {
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
        public ActionResult EditSeries(SeriesEdit model, HttpPostedFileBase pictureUpload = null)
        {

			if (!ModelState.IsValid) {
				return View(model);
			}

			var pictureData = ParsePicture(pictureUpload, "Picture");

			var id = queries.UpdateSeries(model.ToContract(), pictureData);

			return RedirectToAction("SeriesDetails", new { id });

		}

		public ActionResult EventsByDate() {
			
			return View(queries.List(EventSortRule.Date));

		}

		public ActionResult EventsBySeries() {

			var events = Service.GetReleaseEventsBySeries();
			return View(events);

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

			return View(new Versions(contract, enumTranslations));

		}

	}
}
