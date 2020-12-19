#nullable disable

using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using NLog;
using ViewRes.Tag;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Translations;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Search;
using VocaDb.Web.Models.Shared;
using VocaDb.Web.Models.Tag;

namespace VocaDb.Web.Controllers
{
	public class TagController : ControllerBase
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		private readonly IEnumTranslations _enumTranslations;
		private readonly IEntryLinkFactory _entryLinkFactory;
		private readonly MarkdownParser _markdownParser;
		private readonly TagQueries _queries;
		private readonly IAggregatedEntryImageUrlFactory _entryThumbPersister;

		public TagController(TagQueries queries, IEntryLinkFactory entryLinkFactory, IEnumTranslations enumTranslations, MarkdownParser markdownParser,
			IAggregatedEntryImageUrlFactory entryThumbPersister)
		{
			_queries = queries;
			_entryLinkFactory = entryLinkFactory;
			_enumTranslations = enumTranslations;
			_markdownParser = markdownParser;
			_entryThumbPersister = entryThumbPersister;
		}

		public ActionResult ArchivedVersionXml(int id)
		{
			var doc = _queries.GetVersionXml<ArchivedTagVersion>(id);
			var content = doc != null ? XmlHelper.SerializeToUTF8XmlString(doc) : string.Empty;

			return Xml(content);
		}

		public ActionResult Restore(int id)
		{
			_queries.Restore(id);

			return RedirectToAction("DetailsById", new { id });
		}

		private ActionResult RenderDetails(TagDetailsContract contract)
		{
			if (contract == null)
				return HttpNotFound();

			PageProperties.GlobalSearchType = EntryType.Tag;
			PageProperties.PageTitle = contract.Name;
			PageProperties.Title = contract.Name;
			PageProperties.Subtitle = DetailsStrings.Tag;
			PageProperties.CanonicalUrl = _entryLinkFactory.GetFullEntryUrl(EntryType.Tag, contract.Id, contract.UrlSlug);
			PageProperties.OpenGraph.ShowTwitterCard = true;

			return View("Details", contract);
		}

		// Kept for now since there's external references.
		[Obsolete]
		public ActionResult Details(string id)
		{
			if (string.IsNullOrEmpty(id))
				return NoId();

			var tagId = _queries.GetTagByName(id, t => t.Id, InvalidId);

			if (tagId == InvalidId)
			{
				s_log.Info("Tag not found: {0}, referrer {1}", id, Request.UrlReferrer);
				return HttpNotFound();
			}

			return RedirectToActionPermanent("DetailsById", new { id = tagId, slug = id });
		}

		/// <summary>
		/// Redirects to entry type tag based on entry type and possible sub-type.
		/// As fallback, redirects to tags index if no tag is found.
		/// </summary>
		public ActionResult DetailsByEntryType(EntryType entryType, string subType = "")
		{
			var tag = _queries.FindTagForEntryType(new EntryTypeAndSubType(entryType, subType), (tag, lang) => new TagBaseContract(tag, lang));

			if (tag != null)
			{
				return RedirectToAction("DetailsById", new { id = tag.Id, slug = tag.UrlSlug });
			}
			else
			{
				return RedirectToAction("Index");
			}
		}

		public async Task<ActionResult> DetailsById(int id = InvalidId, string slug = null)
		{
			if (id == InvalidId)
				return NoId();

			// TODO: write test for null slug
			slug ??= string.Empty;

			var tagName = await _queries.LoadTagAsync(id, t => t.UrlSlug ?? string.Empty);

			if (slug != tagName)
			{
				return RedirectToActionPermanent("DetailsById", new { id, slug = tagName });
			}

			var contract = await _queries.GetDetailsAsync(id);

			var prop = PageProperties;

			var thumbUrl = Url.ImageThumb(contract.Thumb, ImageSize.Original);
			if (!string.IsNullOrEmpty(thumbUrl))
			{
				PageProperties.OpenGraph.Image = thumbUrl;
			}

			prop.Description = _markdownParser.GetPlainText(contract.Description.EnglishOrOriginal);

			return RenderDetails(contract);
		}

		[Authorize]
		public ActionResult Edit(int id)
		{
			CheckConcurrentEdit(EntryType.Tag, id);

			var model = new TagEditViewModel(_queries.GetTagForEdit(id), PermissionContext);
			return View(model);
		}

		private ActionResult RenderEdit(TagEditViewModel model)
		{
			var contract = _queries.GetTagForEdit(model.Id);
			model.CopyNonEditableProperties(contract, PermissionContext);
			return View("Edit", model);
		}

		[HttpPost]
		[Authorize]
		public ActionResult Edit(TagEditViewModel model)
		{
			var coverPicUpload = Request.Files["thumbPicUpload"];
			UploadedFileContract uploadedPicture = null;
			if (coverPicUpload != null && coverPicUpload.ContentLength > 0)
			{
				CheckUploadedPicture(coverPicUpload, "thumbPicUpload");
				uploadedPicture = new UploadedFileContract { Mime = coverPicUpload.ContentType, Stream = coverPicUpload.InputStream };
			}

			try
			{
				model.CheckModel();
			}
			catch (InvalidFormException x)
			{
				AddFormSubmissionError(x.Message);
			}

			if (!ModelState.IsValid)
			{
				return RenderEdit(model);
			}

			TagBaseContract result;

			try
			{
				result = _queries.Update(model.ToContract(), uploadedPicture);
			}
			catch (DuplicateTagNameException x)
			{
				ModelState.AddModelError("Names", x.Message);
				return RenderEdit(model);
			}

			return RedirectToAction("DetailsById", new { id = result.Id, slug = result.UrlSlug });
		}

		public ActionResult Index(string filter = null)
		{
			if (!string.IsNullOrEmpty(filter))
			{
				var tag = _queries.GetTagByName(filter, t => new { t.Id, t.UrlSlug });

				if (tag != null)
				{
					return RedirectToAction("DetailsById", new { id = tag.Id, slug = tag.UrlSlug });
				}

				return RedirectToAction("Index", "Search", new SearchIndexViewModel(EntryType.Tag, filter));
			}

			var tags = _queries.GetTagsByCategories();
			return View(tags);
		}

		public ActionResult Merge(int id)
		{
			var tag = _queries.LoadTag(id, t => new TagBaseContract(t, PermissionContext.LanguagePreference));
			return View(tag);
		}

		[HttpPost]
		public ActionResult Merge(int id, int? targetTagId)
		{
			if (targetTagId == null)
			{
				ModelState.AddModelError("targetTagId", "Tag must be selected");
				return Merge(id);
			}

			_queries.Merge(id, targetTagId.Value);

			return RedirectToAction("Edit", new { id = targetTagId.Value });
		}

		[OutputCache(Location = System.Web.UI.OutputCacheLocation.Any, Duration = 3600)]
		public ActionResult PopupContent(
			int id = InvalidId,
			ContentLanguagePreference lang = ContentLanguagePreference.Default,
			string culture = InterfaceLanguage.DefaultCultureCode)
		{
			if (id == InvalidId)
				return HttpNotFound();

			var tag = _queries.LoadTag(id, t => new TagForApiContract(t, _entryThumbPersister,
				lang, TagOptionalFields.AdditionalNames | TagOptionalFields.Description | TagOptionalFields.MainPicture));
			return PartialView("_TagPopupContent", tag);
		}

		public ActionResult UpdateVersionVisibility(int archivedVersionId, bool hidden)
		{
			_queries.UpdateVersionVisibility<ArchivedTagVersion>(archivedVersionId, hidden);

			return RedirectToAction("ViewVersion", new { id = archivedVersionId });
		}

		public ActionResult Versions(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var contract = _queries.GetTagWithArchivedVersions(id);
			return View(new Versions(contract, _enumTranslations));
		}

		public ActionResult ViewVersion(int id, int? ComparedVersionId)
		{
			var contract = _queries.GetVersionDetails(id, ComparedVersionId ?? 0);

			return View(new ViewVersion<ArchivedTagVersionDetailsContract>(contract, _enumTranslations, contract.ComparedVersionId));
		}
	}
}
