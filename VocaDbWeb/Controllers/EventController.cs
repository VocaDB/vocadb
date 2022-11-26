#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Translations;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Helpers;
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

		[Authorize]
		public ActionResult Edit(int? id)
		{
			return View("React/Index");
		}

		[Authorize]
		public ActionResult EditSeries(int? id)
		{
			return View("React/Index");
		}
#nullable disable

		public ActionResult EventsByDate()
		{
			PageProperties.Title = ViewRes.SharedStrings.ReleaseEvents;

			return View("React/Index");
		}

		public ActionResult EventsBySeries()
		{
			PageProperties.Title = ViewRes.SharedStrings.ReleaseEvents;

			return View("React/Index");
		}

		public ActionResult EventsByVenue()
		{
			PageProperties.Title = ViewRes.SharedStrings.ReleaseEvents;

			return View("React/Index");
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

			return View("React/Index");
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

			return View("React/Index");
		}

		public ActionResult Versions(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var contract = Service.GetReleaseEventWithArchivedVersions(id);

			PageProperties.Title = ViewRes.EntryDetailsStrings.Revisions + " - " + contract.Name;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View("React/Index");
		}

		public ActionResult ViewVersion(int id, int? ComparedVersionId)
		{
			var contract = _queries.GetVersionDetails(id, ComparedVersionId ?? 0);

			PageProperties.Title = "Revision " + contract.ArchivedVersion.Version + " for " + contract.Name;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View("React/Index");
		}
	}
}
