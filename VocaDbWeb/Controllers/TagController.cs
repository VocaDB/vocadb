using System.Web.Mvc;
using ViewRes.Tag;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;
using VocaDb.Model.Utils;
using VocaDb.Web.Controllers.DataAccess;
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

		public ActionResult Delete(string id) {

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
			PageProperties.CanonicalUrl = entryLinkFactory.GetFullEntryUrl(EntryType.Tag, contract.Id, contract.Name);
			PageProperties.OpenGraph.ShowTwitterCard = true;

			return View("Details", contract);

		}

		public ActionResult Details(string id) {

			if (string.IsNullOrEmpty(id))
				return NoId();

			var tagId = queries.GetTag(id, t => t != null ? t.Id : invalidId);

			if (tagId == invalidId)
				return HttpNotFound();

			return RedirectToActionPermanent("DetailsById", new { id = tagId, slug = id });

		}

		public ActionResult DetailsById(int id = invalidId, string slug = null) {

			if (id == invalidId)
				return NoId();

			var tagName = queries.GetTagNameById(id);

			if (slug != tagName) {
				return RedirectToActionPermanent("DetailsById", new { id, slug = tagName });
			}

			var contract = queries.GetDetails(tagName);

			return RenderDetails(contract);

		}

        [Authorize]
        public ActionResult Edit(string id)
        {
			var model = new TagEdit(queries.GetTagForEdit(id), PermissionContext);
			return View(model);
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
				var contract = queries.GetTagForEdit(model.Name);
				model.CopyNonEditableProperties(contract);
				return View(model);
			}

			queries.Update(model.ToContract(), uploadedPicture);

			return RedirectToAction("Details", new { id = model.Name });

		}

		public ActionResult Index(string filter = null) {

			if (!string.IsNullOrEmpty(filter)) {

				var tagName = queries.GetTag(filter, t => t != null ? t.Name : null);

				if (tagName != null) {
					return RedirectToAction("Details", new { id = tagName });
				}

				return RedirectToAction("Index", "Search", new SearchIndexViewModel(EntryType.Tag, filter));

			}

			var tags = queries.GetTagsByCategories();
			return View(tags);

		}

		public ActionResult Versions(string id) {

			if (string.IsNullOrEmpty(id))
				return NoId();

			var contract = queries.GetTagWithArchivedVersions(id);

			if (contract == null)
				return HttpNotFound();

			return View(contract);

		}

    }
}
