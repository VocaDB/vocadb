using System.Globalization;
using System.Web.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Controllers
{
    public class HelpController : ControllerBase
    {

		private readonly TagQueries tagQueries;

		public HelpController(TagQueries tagQueries) {
			this.tagQueries = tagQueries;
		}

        //
        // GET: /Help/

        public ActionResult Index()
        {
			
			if (!string.IsNullOrEmpty(AppConfig.ExternalHelpPath))
				return View("External");

			ViewBag.FreeTagId = tagQueries.GetTagIdByName(Tag.CommonTag_Free);

			if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja")
				return View("Index.ja");
			else
				return View();

        }

    }
}
