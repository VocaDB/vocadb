using System;
using System.Web.Mvc;
using ViewRes.Tag;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Search;
using VocaDb.Web.Models.Tag;

namespace VocaDb.Web.Controllers
{
    public class TagController : ControllerBase {

		private readonly IEntryLinkFactory entryLinkFactory;
	    private readonly TagQueries queries;

		public TagController(TagQueries queries, IEntryLinkFactory entryLinkFactory) {

			this.queries = queries;
			this.entryLinkFactory = entryLinkFactory;

		}

		public ActionResult Delete(int id) {

			queries.Delete(id);

			TempData.SetStatusMessage("Tag deleted");

			return RedirectToAction("Index");

		}

		private ActionResult RenderDetails(TagDetailsContract contract) {

			if (contract == null)
				return HttpNotFound();

			PageProperties.GlobalSearchType = EntryType.Tag;
			PageProperties.PageTitle = string.Format("{0} - {1}", DetailsStrings.TagDetails, contract.Name);
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

			return RenderDetails(contract);

		}

        [Authorize]
        public ActionResult Edit(int id)
        {
			var model = new TagEdit(queries.GetTagForEdit(id), PermissionContext);
			return View(model);
		}

		private ActionResult RenderEdit(TagEdit model) {
			var contract = queries.GetTagForEdit(model.Id);
			model.CopyNonEditableProperties(contract, PermissionContext);
			return View("Edit", model);
		}

		[HttpPost]
        [Authorize]
        public ActionResult Edit(TagEdit model)
        {

			var coverPicUpload = Request.Files["thumbPicUpload"];
			UploadedFileContract uploadedPicture = null;
			if (coverPicUpload != null && coverPicUpload.ContentLength > 0) {

				CheckUploadedPicture(coverPicUpload, "thumbPicUpload");
				uploadedPicture = new UploadedFileContract { Mime = coverPicUpload.ContentType, Stream = coverPicUpload.InputStream };

			}

			if (!ModelState.IsValid) {
				return RenderEdit(model);
            }

			TagBaseContract result;

			try {
				result = queries.Update(model.ToContract(), uploadedPicture);
			} catch (DuplicateTagNameException x) {
				ModelState.AddModelError("EnglishName", x.Message);
				return RenderEdit(model);
			}

			return RedirectToAction("DetailsById", new { id = result.Id, slug = result.UrlSlug });

		}

		public ActionResult Index(string filter = null) {

			if (!string.IsNullOrEmpty(filter)) {

				var tag = queries.GetTagByName(filter, t => new { t.Id, t.EnglishName });

				if (tag != null) {
					return RedirectToAction("DetailsById", new { id = tag.Id, slug = tag.EnglishName });
				}

				return RedirectToAction("Index", "Search", new SearchIndexViewModel(EntryType.Tag, filter));

			}

			var tags = queries.GetTagsByCategories();
			return View(tags);

		}

		public ActionResult Versions(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var contract = queries.GetTagWithArchivedVersions(id);
			return View(contract);

		}

    }
}
