using System;
using System.Web.Mvc;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Web.Models.Tag;
using VocaDb.Web.Resources.Controllers;

namespace VocaDb.Web.Controllers
{
    public class TagController : ControllerBase {

	    private readonly TagQueries queries;
	    private readonly TagService service;

		private TagService Service {
			get { return service; }
		}

		public TagController(TagService service, TagQueries queries) {

			this.service = service;
			this.queries = queries;

		}

		public ActionResult Create(string name) {

			if (string.IsNullOrWhiteSpace(name))
				return Json(new GenericResponse<string>(false, TagControllerStrings.TagNameError));

			name = name.Trim().Replace(' ', '_');

			if (!Tag.IsValidTagName(name))
				return Json(new GenericResponse<string>(false, TagControllerStrings.TagNameError));

			var view = RenderPartialViewToString("TagSelection", new TagSelectionContract(name, true));

			return Json(new GenericResponse<string>(view));

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
			var model = new TagEdit(Service.GetTagForEdit(id));
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
				var contract = Service.GetTagForEdit(model.Name);
				model.CopyNonEditableProperties(contract);
				return View(model);
			}

			queries.Update(model.ToContract(), uploadedPicture);

			return RedirectToAction("Details", new { id = model.Name });

		}

		[Obsolete("Moved to Web API")]
		public ActionResult Find(string term, bool allowAliases = true) {

			return Json(queries.FindNames(TagSearchTextQuery.Create(term), allowAliases, false, 10));

		}

		public ActionResult FindCategories(string term) {

			return Json(Service.FindCategories(term));

		}

		public ActionResult Index(string filter = null) {

			var tags = Service.GetTagsByCategories();

			if (!string.IsNullOrEmpty(filter)) {

				var tag = Service.GetTag(filter);

				if (tag != null) {
					return RedirectToAction("Details", new { id = tag.Name});
				}

			}

			return View(tags);

		}

		public ActionResult Versions(string id) {

			if (string.IsNullOrEmpty(id))
				return NoId();

			var contract = Service.GetTagWithArchivedVersions(id);

			if (contract == null)
				return HttpNotFound();

			return View(contract);

		}

    }
}
