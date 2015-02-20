using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Tag;

namespace VocaDb.Web.Controllers
{
    public class TagController : ControllerBase {

	    private readonly TagQueries queries;

		public TagController(TagQueries queries) {

			this.queries = queries;

		}

		public ActionResult Delete(string id) {

			queries.Delete(id);

			TempData.SetStatusMessage("Tag deleted");

			return RedirectToAction("Index");

		}

		public ActionResult Details(string id) {

			if (string.IsNullOrEmpty(id))
				return NoId();

			var contract = queries.GetDetails(id);

			if (contract == null)
				return HttpNotFound();

			return View(contract);

		}

        [Authorize]
        public ActionResult Edit(string id)
        {
			var model = new TagEdit(queries.GetTagForEdit(id));
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
