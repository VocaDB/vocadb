using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
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

			queries.DeleteSeries(id);

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

			var inheritedCategory = ev.Series != null ? ev.Series.Category : ev.Category;
			string subtitle;

			if (inheritedCategory == EventCategory.Unspecified || inheritedCategory == EventCategory.Other) {
				subtitle = ViewRes.Event.DetailsStrings.Event;
			} else {
				subtitle = Translate.ReleaseEventCategoryNames[inheritedCategory];
			}

			var pictureData = !string.IsNullOrEmpty(ev.PictureMime) ? (IEntryImageInformation)ev : ev.Series;

			PageProperties.PageTitle = string.Format("{0} ({1})", ev.Name, subtitle);
			PageProperties.Title = ev.Name;
			PageProperties.Subtitle = subtitle;
			PageProperties.CanonicalUrl = entryLinkFactory.GetFullEntryUrl(EntryType.ReleaseEvent, ev.Id, ev.UrlSlug);
			PageProperties.OpenGraph.Image = Url.ImageThumb(pictureData, ImageSize.Original);
			// Note: description is set in view

			return View(ev);

		}

        [Authorize]
        public ActionResult Edit(int? id, int? seriesId)
        {

			var model = (id != null ? new EventEdit(Service.GetReleaseEventForEdit(id.Value), PermissionContext) 
				: new EventEdit(seriesId != null ? Service.GetReleaseEventSeriesForEdit(seriesId.Value) : null, PermissionContext));

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
			return View(new SeriesEdit(contract, PermissionContext));

		}

		[HttpPost]
        [Authorize]
        public ActionResult EditSeries(SeriesEdit model, HttpPostedFileBase pictureUpload = null)
        {

			if (!ModelState.IsValid) {
				model.AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(PermissionContext).ToArray();
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
				new ReleaseEventForApiContract(e, ReleaseEventOptionalFields.MainPicture | ReleaseEventOptionalFields.Series, thumbPersister, WebHelper.IsSSL(Request)),
				queryParams);

			return View(events.Items);

        }

	    [OutputCache(Location = System.Web.UI.OutputCacheLocation.Any, Duration = 3600)]
	    public ActionResult PopupContent(
		    int id = invalidId,
		    string culture = InterfaceLanguage.DefaultCultureCode) {

		    if (id == invalidId)
			    return HttpNotFound();

		    var releaseEvent = queries.Load(id, ReleaseEventOptionalFields.MainPicture | ReleaseEventOptionalFields.Series);
		    return PartialView("_EventPopupContent", releaseEvent);

	    }

		public ActionResult SeriesDetails(int id = invalidId, string slug = null) {

			if (id == invalidId)
				return NoId();

			var series = Service.GetReleaseEventSeriesDetails(id);
			string subtitle;

			if (series.Category == EventCategory.Unspecified || series.Category == EventCategory.Other) {
				subtitle = ViewRes.MiscStrings.EventSeries;
			} else {
				subtitle = Translate.ReleaseEventCategoryNames[series.Category];
			}

			PageProperties.PageTitle = string.Format("{0} ({1})", series.Name, subtitle);
			PageProperties.Title = series.Name;
			PageProperties.Subtitle = subtitle;
			PageProperties.OpenGraph.Image = Url.ImageThumb(series, ImageSize.Original);

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
