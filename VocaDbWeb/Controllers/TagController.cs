using System;
using System.Globalization;
using System.Web.Mvc;
using ViewRes.Tag;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
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
    public class TagController : ControllerBase {

		private readonly IEnumTranslations enumTranslations;
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly MarkdownParser markdownParser;
		private readonly TagQueries queries;

		public TagController(TagQueries queries, IEntryLinkFactory entryLinkFactory, IEnumTranslations enumTranslations, MarkdownParser markdownParser) {

			this.queries = queries;
			this.entryLinkFactory = entryLinkFactory;
			this.enumTranslations = enumTranslations;
			this.markdownParser = markdownParser;

		}

		public ActionResult ArchivedVersionXml(int id) {

			var doc = queries.GetVersionXml(id);
			var content = XmlHelper.SerializeToUTF8XmlString(doc);

			return Xml(content);

		}

		public ActionResult Restore(int id) {

			queries.Restore(id);

			return RedirectToAction("DetailsById", new { id });

		}

		private ActionResult RenderDetails(TagDetailsContract contract) {

			if (contract == null)
				return HttpNotFound();

			PageProperties.GlobalSearchType = EntryType.Tag;
			PageProperties.PageTitle = contract.Name;
			PageProperties.Title = contract.Name;
			PageProperties.Subtitle = DetailsStrings.Tag;
			PageProperties.CanonicalUrl = entryLinkFactory.GetFullEntryUrl(EntryType.Tag, contract.Id, contract.UrlSlug);
			PageProperties.OpenGraph.ShowTwitterCard = true;

			return View("Details", contract);

		}

		// Kept for now since there's external references.
		[Obsolete]
		public ActionResult Details(string id) {

			if (string.IsNullOrEmpty(id))
				return NoId();

			var tagId = queries.GetTagByName(id, t => t.Id, invalidId);

			if (tagId == invalidId)
				return HttpNotFound();

			return RedirectToActionPermanent("DetailsById", new { id = tagId, slug = id });

		}

		public ActionResult DetailsById(int id = invalidId, string slug = null) {

			if (id == invalidId)
				return NoId();

			// TODO: write test for null slug
			slug = slug ?? string.Empty;

			var tagName = queries.LoadTag(id, t => t.UrlSlug ?? string.Empty);

			if (slug != tagName) {
				return RedirectToActionPermanent("DetailsById", new { id, slug = tagName });
			}

			var contract = queries.GetDetails(id);

			var prop = PageProperties;

			var thumbUrl = Url.EntryImageOld(contract.Thumb, ImageSize.Original);
			if (!string.IsNullOrEmpty(thumbUrl)) {
				PageProperties.OpenGraph.Image = thumbUrl;
			}

			prop.Description = markdownParser.GetPlainText(contract.Description.EnglishOrOriginal);

			return RenderDetails(contract);

		}

        [Authorize]
        public ActionResult Edit(int id)
        {

			CheckConcurrentEdit(EntryType.Tag, id);

			var model = new TagEditViewModel(queries.GetTagForEdit(id), PermissionContext);
			return View(model);

		}

		private ActionResult RenderEdit(TagEditViewModel model) {
			var contract = queries.GetTagForEdit(model.Id);
			model.CopyNonEditableProperties(contract, PermissionContext);
			return View("Edit", model);
		}

		[HttpPost]
        [Authorize]
        public ActionResult Edit(TagEditViewModel model)
        {

			var coverPicUpload = Request.Files["thumbPicUpload"];
			UploadedFileContract uploadedPicture = null;
			if (coverPicUpload != null && coverPicUpload.ContentLength > 0) {

				CheckUploadedPicture(coverPicUpload, "thumbPicUpload");
				uploadedPicture = new UploadedFileContract { Mime = coverPicUpload.ContentType, Stream = coverPicUpload.InputStream };

			}

			try {
				model.CheckModel();
			} catch (InvalidFormException x) {
				AddFormSubmissionError(x.Message);
			}

			if (!ModelState.IsValid) {
				return RenderEdit(model);
            }

			TagBaseContract result;

			try {
				result = queries.Update(model.ToContract(), uploadedPicture);
			} catch (DuplicateTagNameException x) {
				ModelState.AddModelError("Names", x.Message);
				return RenderEdit(model);
			}

			return RedirectToAction("DetailsById", new { id = result.Id, slug = result.UrlSlug });

		}

		public ActionResult Index(string filter = null) {

			if (!string.IsNullOrEmpty(filter)) {

				var tag = queries.GetTagByName(filter, t => new { t.Id, t.UrlSlug });

				if (tag != null) {
					return RedirectToAction("DetailsById", new { id = tag.Id, slug = tag.UrlSlug });
				}

				return RedirectToAction("Index", "Search", new SearchIndexViewModel(EntryType.Tag, filter));

			}

			var tags = queries.GetTagsByCategories();
			return View(tags);

		}

		public ActionResult Merge(int id) {

			var tag = queries.LoadTag(id, t => new TagBaseContract(t, PermissionContext.LanguagePreference));
			return View(tag);

		}

		[HttpPost]
		public ActionResult Merge(int id, int? targetTagId) {

			if (targetTagId == null) {
				ModelState.AddModelError("targetTagId", "Tag must be selected");
				return Merge(id);
			}

			queries.Merge(id, targetTagId.Value);

			return RedirectToAction("Edit", new { id = targetTagId.Value });

		}

		public ActionResult Versions(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var contract = queries.GetTagWithArchivedVersions(id);
			return View(new Versions(contract, enumTranslations));

		}

		public ActionResult ViewVersion(int id, int? ComparedVersionId) {

			var contract = queries.GetVersionDetails(id, ComparedVersionId ?? 0);

			return View(new ViewVersion<ArchivedTagVersionDetailsContract>(contract, enumTranslations, contract.ComparedVersionId));

		}

	}
}
