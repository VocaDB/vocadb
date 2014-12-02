using System.Web.Mvc;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.NewsEntry;

namespace VocaDb.Web.Controllers
{
    public class NewsEntryController : ControllerBase
    {

		private NewsEntryService Service {
			get { return Services.NewsEntry; }
		}

		public ActionResult Delete(int id) {

			Service.Delete(id);
			TempData.SetStatusMessage("Entry deleted");
			return RedirectToAction("Index");

		}

		public ActionResult Edit(int? id) {

			var model =  (id != null ? new NewsEntryEdit(Service.GetNewsEntry(id.Value)) : new NewsEntryEdit());
			return View(model);

		}

		[HttpPost]
		public ActionResult Edit(NewsEntryEdit model) {

			if (!ModelState.IsValid) {
				return View(model);
			}

			var contract = model.ToContract();
			Service.UpdateNewsEntry(contract);

			TempData.SetStatusMessage("Entry edited");
			return RedirectToAction("Index");

		}

        //
        // GET: /NewsEntry/

        public ActionResult Index()
        {

			var entries = Service.GetNewsEntries(500);
            return View(entries);

        }

    }
}
