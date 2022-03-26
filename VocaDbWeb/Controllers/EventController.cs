#nullable disable

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Translations;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Event;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Controllers
{
	public class EventController : ControllerBase
	{
		private readonly IEnumTranslations _enumTranslations;
		private readonly IEntryLinkFactory _entryLinkFactory;
		private readonly IAggregatedEntryImageUrlFactory _thumbPersister;
		private readonly EventQueries _queries;
		private readonly ReleaseEventService _service;
		private readonly MarkdownParser _markdownParser;

		private ReleaseEventService Service => _service;

		public EventController(
			EventQueries queries,
			ReleaseEventService service,
			IEnumTranslations enumTranslations,
			IEntryLinkFactory entryLinkFactory,
			IAggregatedEntryImageUrlFactory thumbPersister,
			MarkdownParser markdownParser
		)
		{
			_queries = queries;
			_service = service;
			_enumTranslations = enumTranslations;
			_entryLinkFactory = entryLinkFactory;
			_thumbPersister = thumbPersister;
			_markdownParser = markdownParser;
		}

		public ActionResult ArchivedSeriesVersionXml(int id)
		{
			var doc = _queries.GetVersionXml<ArchivedReleaseEventSeriesVersion>(id);
			var contract = doc != null ? XmlHelper.SerializeToUTF8XmlString(doc) : string.Empty;

			return Xml(contract);
		}

		public ActionResult ArchivedVersionXml(int id)
		{
			var doc = _queries.GetVersionXml<ArchivedReleaseEventVersion>(id);
			var content = doc != null ? XmlHelper.SerializeToUTF8XmlString(doc) : string.Empty;

			return Xml(content);
		}

#nullable enable
		public ActionResult Details(int id = InvalidId, string? slug = null)
		{
			if (id == InvalidId)
				return NoId();

			slug ??= string.Empty;

			var ev = _queries.GetDetails(id);

			if (slug != ev.UrlSlug)
			{
				return RedirectToActionPermanent("Details", new { id, slug = ev.UrlSlug });
			}

			var inheritedCategory = ev.InheritedCategory;
			string? subtitle;

			if (inheritedCategory == EventCategory.Unspecified || inheritedCategory == EventCategory.Other)
			{
				subtitle = ViewRes.Event.DetailsStrings.Event;
			}
			else
			{
				subtitle = Translate.ReleaseEventCategoryNames[inheritedCategory];
			}

			var pictureData = ev.MainPicture ?? ev.Series?.MainPicture;

			PageProperties.PageTitle = $"{ev.Name} ({subtitle})";
			PageProperties.Title = ev.Name;
			PageProperties.Subtitle = subtitle;
			PageProperties.CanonicalUrl = _entryLinkFactory.GetFullEntryUrl(EntryType.ReleaseEvent, ev.Id, ev.UrlSlug);
			PageProperties.OpenGraph.Image = Url.ImageThumb(pictureData, ImageSize.Original);

			var descriptionStripped = _markdownParser.GetPlainText(ev.Description);

			PageProperties.Description = descriptionStripped;
			PageProperties.Robots = ev.Deleted ? PagePropertiesData.Robots_Noindex_Follow : string.Empty;

			return View("React/Index");
		}
#nullable disable

		[Authorize]
		public ActionResult Edit(int? id, int? seriesId, int? venueId)
		{
			if (id != null)
			{
				CheckConcurrentEdit(EntryType.ReleaseEvent, id.Value);
			}

			var model = (id != null ? new EventEdit(_queries.GetEventForEdit(id.Value), PermissionContext)
				: new EventEdit(
					seriesId.HasValue ? Service.GetReleaseEventSeriesForEdit(seriesId.Value) : null,
					venueId.HasValue ? _service.GetVenueForEdit(venueId.Value) : null,
					PermissionContext));

			return View(model);
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> Edit(EventEdit model, IFormFile pictureUpload = null)
		{
			ActionResult RenderEdit()
			{
				if (model.Id != 0)
				{
					var contract = _queries.GetEventForEdit(model.Id);
					model.CopyNonEditableProperties(contract, PermissionContext);
				}
				else
				{
					model.CopyNonEditableProperties(null, PermissionContext);
				}

				return View("Edit", model);
			}

			// Either series or name must be specified. If series is specified, name is generated automatically.
			if (model.Series.IsNullOrDefault() || model.CustomName)
			{
				// Note: name is allowed to be whitespace, but not empty.
				if (model.Names == null || model.Names.All(n => string.IsNullOrEmpty(n?.Value)))
				{
					ModelState.AddModelError("Names", "Name cannot be empty");
				}
			}

			if (!ModelState.IsValid)
			{
				return RenderEdit();
			}

			var pictureData = ParsePicture(pictureUpload, "pictureUpload", ImagePurpose.Main);

			if (!ModelState.IsValid)
			{
				return RenderEdit();
			}

			int id;

			try
			{
				id = (await _queries.Update(model.ToContract(), pictureData)).Id;
			}
			catch (DuplicateEventNameException x)
			{
				ModelState.AddModelError("Names", x.Message);
				return RenderEdit();
			}

			return RedirectToAction("Details", new { id });
		}

		[Authorize]
		public ActionResult EditSeries(int? id)
		{
			if (id != null)
			{
				CheckConcurrentEdit(EntryType.ReleaseEventSeries, id.Value);
			}

			var contract = (id != null ? Service.GetReleaseEventSeriesForEdit(id.Value) : new ReleaseEventSeriesForEditContract());
			return View(new SeriesEdit(contract, PermissionContext));
		}

		[HttpPost]
		[Authorize]
		public ActionResult EditSeries(SeriesEdit model, IFormFile pictureUpload = null)
		{
			ActionResult RenderEdit()
			{
				model.AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(PermissionContext).ToArray();
				return View("EditSeries", model);
			}

			// Note: name is allowed to be whitespace, but not empty.
			if (model.Names == null || model.Names.All(n => string.IsNullOrEmpty(n?.Value)))
			{
				ModelState.AddModelError("Names", "Name cannot be empty");
			}

			if (!ModelState.IsValid)
			{
				return RenderEdit();
			}

			var pictureData = ParsePicture(pictureUpload, "Picture", ImagePurpose.Main);

			int id;
			try
			{
				id = _queries.UpdateSeries(model.ToContract(), pictureData);
			}
			catch (DuplicateEventNameException x)
			{
				ModelState.AddModelError("Names", x.Message);
				return RenderEdit();
			}

			return RedirectToAction("SeriesDetails", new { id });
		}

		public ActionResult EventsByDate()
		{
			PageProperties.Title = ViewRes.SharedStrings.ReleaseEvents;

			return View(_queries.List(EventSortRule.Date, SortDirection.Descending));
		}

		public ActionResult EventsBySeries()
		{
			var events = Service.GetReleaseEventsBySeries();

			PageProperties.Title = ViewRes.SharedStrings.ReleaseEvents;

			return View(events);
		}

		public ActionResult EventsByVenue()
		{
			var events = _queries.GetReleaseEventsByVenue();

			PageProperties.Title = ViewRes.SharedStrings.ReleaseEvents;

			return View(events);
		}

		//
		// GET: /Event/

		public ActionResult Index()
		{
			PageProperties.Title = ViewRes.SharedStrings.ReleaseEvents;

			return View("React/Index");
		}

		[Authorize]
		public ActionResult ManageTagUsages(int id)
		{
			var releaseEvent = _queries.GetEntryWithTagUsages(id);

			PageProperties.Title = "Manage tag usages - " + releaseEvent.DefaultName;

			return View(releaseEvent);
		}

		[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3600, VaryByQueryKeys = new[] { "*" })]
		public ActionResult PopupContent(
			int id = InvalidId,
			string culture = InterfaceLanguage.DefaultCultureCode)
		{
			if (id == InvalidId)
				return NotFound();

			var releaseEvent = _queries.Load(id, ReleaseEventOptionalFields.AdditionalNames | ReleaseEventOptionalFields.MainPicture | ReleaseEventOptionalFields.Series);
			return PartialView("_EventPopupContent", releaseEvent);
		}

		[Authorize]
		public ActionResult RemoveTagUsage(long id)
		{
			var eventId = _queries.RemoveTagUsage(id);
			TempData.SetStatusMessage("Tag usage removed");

			return RedirectToAction("ManageTagUsages", new { id = eventId });
		}

		public ActionResult Restore(int id)
		{
			_queries.Restore(id);

			return RedirectToAction("Edit", new { id });
		}

		public ActionResult RestoreSeries(int id)
		{
			_queries.RestoreSeries(id);

			return RedirectToAction("EditSeries", new { id });
		}

		public ActionResult SeriesDetails(int id = InvalidId, string slug = null)
		{
			if (id == InvalidId)
				return NoId();

			slug ??= string.Empty;

			var series = _queries.GetSeriesDetails(id);

			if (slug != series.UrlSlug)
			{
				return RedirectToActionPermanent("SeriesDetails", new { id, slug = series.UrlSlug });
			}

			string subtitle;

			if (series.Category == EventCategory.Unspecified || series.Category == EventCategory.Other)
			{
				subtitle = ViewRes.MiscStrings.EventSeries;
			}
			else
			{
				subtitle = Translate.ReleaseEventCategoryNames[series.Category];
			}

			PageProperties.PageTitle = $"{series.Name} ({subtitle})";
			PageProperties.Title = series.Name;
			PageProperties.Subtitle = subtitle;
			PageProperties.OpenGraph.Image = Url.ImageThumb(series.MainPicture, ImageSize.Original);

			var descriptionStripped = _markdownParser.GetPlainText(series.Description);

			PageProperties.Description = descriptionStripped;
			PageProperties.Robots = series.Deleted ? PagePropertiesData.Robots_Noindex_Follow : string.Empty;

			return View("React/Index");
		}

		public ActionResult SeriesVersions(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var contract = Service.GetReleaseEventSeriesWithArchivedVersions(id);

			PageProperties.Title = ViewRes.EntryDetailsStrings.Revisions + " - " + contract.Entry.Name;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View(new Versions<ReleaseEventSeriesContract>(contract, _enumTranslations));
		}

		public ActionResult UpdateSeriesVersionVisibility(int archivedVersionId, bool hidden)
		{
			_queries.UpdateVersionVisibility<ArchivedReleaseEventSeriesVersion>(archivedVersionId, hidden);

			return RedirectToAction("ViewSeriesVersion", new { id = archivedVersionId });
		}

		public ActionResult UpdateVersionVisibility(int archivedVersionId, bool hidden)
		{
			_queries.UpdateVersionVisibility<ArchivedReleaseEventVersion>(archivedVersionId, hidden);

			return RedirectToAction("ViewVersion", new { id = archivedVersionId });
		}

		public ActionResult ViewSeriesVersion(int id, int? ComparedVersionId)
		{
			var contract = _queries.GetSeriesVersionDetails(id, ComparedVersionId ?? 0);

			PageProperties.Title = "Revision " + contract.ArchivedVersion.Version + " for " + contract.Name;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View(new ViewVersion<ArchivedEventSeriesVersionDetailsContract>(contract, _enumTranslations, contract.ComparedVersionId));
		}

		public ActionResult Versions(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var contract = Service.GetReleaseEventWithArchivedVersions(id);

			PageProperties.Title = ViewRes.EntryDetailsStrings.Revisions + " - " + contract.Name;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View(new Versions(contract, _enumTranslations));
		}

		public ActionResult ViewVersion(int id, int? ComparedVersionId)
		{
			var contract = _queries.GetVersionDetails(id, ComparedVersionId ?? 0);

			PageProperties.Title = "Revision " + contract.ArchivedVersion.Version + " for " + contract.Name;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View(new ViewVersion<ArchivedEventVersionDetailsContract>(contract, _enumTranslations, contract.ComparedVersionId));
		}
	}
}
