using System.Globalization;
using System.Web.Mvc;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Controllers
{
    public class HelpController : ControllerBase
    {

        //
        // GET: /Help/

        public ActionResult Index()
        {
			
			if (!string.IsNullOrEmpty(AppConfig.ExternalHelpPath))
				return View("External");

			if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja")
				return View("Index.ja");
			else
				return View();

        }

    }
}
