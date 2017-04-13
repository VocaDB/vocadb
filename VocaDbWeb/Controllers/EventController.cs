using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Events;
using VocaDb.Model.Service.Translations;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Event;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Controllers
{
    public class EventController : ControllerBase
    {
		
		private readonly IEnumTranslations enumTranslations;
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEntryThumbPersister thumbPersister;
		private readonly EventQueries queries;
		private readonly ReleaseEventService service;

		private ReleaseEventService Service => service;

	    public EventController(EventQueries queries, ReleaseEventService service, IEnumTranslations enumTranslations, IEntryLinkFactory entryLinkFactory,
			IEntryThumbPersister thumbPersister) {
			this.queries = queries;
			this.service = service;
			this.enumTranslations = enumTranslations;
			this.entryLinkFactory = entryLinkFactory;
			this.thumbPersister = thumbPersister;
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

		public ActionResult Details(int id = invalidId, string slug = null) {

		    if (id == invalidId)
		        return NoId();

			slug = slug ?? string.Empty;

			var ev = queries.GetDetails(id);

			if (slug != ev.UrlSlug) {
				return RedirectToActionPermanent("Details", new { id, slug = ev.UrlSlug });
			}

			PageProperties.PageTitle = string.Format("{0} ({1})", ev.Name, ViewRes.Event.DetailsStrings.Event);
			PageProperties.Title = ev.Name;
			PageProperties.CanonicalUrl = entryLinkFactory.GetFullEntryUrl(EntryType.ReleaseEvent, ev.Id, ev.UrlSlug);

			if (ev.Category == EventCategory.Unspecified || ev.Category == EventCategory.Other) {
				PageProperties.Subtitle = ViewRes.Event.DetailsStrings.Event;
			} else {
				PageProperties.Subtitle = Translate.ReleaseEventCategoryNames[ev.Category];
			}

			return View(ev);

		}

        [Authorize]
        public ActionResult Edit(int? id, int? seriesId)
        {

			var model = (id != null ? new EventEdit(Service.GetReleaseEventForEdit(id.Value), PermissionContext) 
				: new EventEdit(seriesId != null ? Service.GetReleaseEventSeriesForEdit(seriesId.Value): null));

			return View(model);

		}

		[HttpPost]
        [Authorize]
        public ActionResult Edit(EventEdit model, HttpPostedFileBase pictureUpload = null)
        {

			// Either series or name must be specified. If series is specified, name is generated automatically.
			if (string.IsNullOrEmpty(model.Name) && (model.Series.IsNullOrDefault() || model.CustomName)) {
				ModelState.AddModelError("Name", "Name cannot be empty");
			}

			if (!ModelState.IsValid) {

				if (model.Id != 0) {
					var contract = Service.GetReleaseEventForEdit(model.Id);
					model.CopyNonEditableProperties(contract, PermissionContext);					
				}

				return View(model);
			}

	        var pictureData = ParsePicture(pictureUpload, "pictureUpload");
			var id = queries.Update(model.ToContract(), pictureData).Id;

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
			
			return View(queries.List(EventSortRule.Date, SortDirection.Descending));

		}

		public ActionResult EventsBySeries() {

			var events = Service.GetReleaseEventsBySeries();
			return View(events);

		}

        //
        // GET: /Event/

        public ActionResult Index()
        {

	        var queryParams = new EventQueryParams {
		        AfterDate = DateTime.Now.AddDays(-7),
		        Paging = new PagingProperties(0, 12, false),
		        SortRule = EventSortRule.Date,
				SortDirection = SortDirection.Ascending
			};

			var events = queries.Find(e =>
				new ReleaseEventForApiContract(e, ReleaseEventOptionalFields.MainPicture, thumbPersister, WebHelper.IsSSL(Request)),
				queryParams);

			return View(events.Items);

        }

		public ActionResult SeriesDetails(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var series = Service.GetReleaseEventSeriesDetails(id);
			return View(series);

		}

		public ActionResult SeriesVersions(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var contract = Service.GetReleaseEventSeriesWithArchivedVersions(id);

			return View(new Versions<ReleaseEventSeriesContract>(contract, enumTranslations));

		}

		public ActionResult Versions(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var contract = Service.GetReleaseEventWithArchivedVersions(id);

			return View(new Versions(contract, enumTranslations));

		}

	}
}
