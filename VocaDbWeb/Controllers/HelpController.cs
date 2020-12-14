#nullable disable

using System.Globalization;
using System.Web.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Service;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Web.Controllers
{
	public class HelpController : ControllerBase
	{
		private readonly VdbConfigManager config;
		private readonly TagQueries tagQueries;

		public HelpController(VdbConfigManager config, TagQueries tagQueries)
		{
			this.config = config;
			this.tagQueries = tagQueries;
		}

		//
		// GET: /Help/

		public ActionResult Index()
		{
			if (!string.IsNullOrEmpty(AppConfig.ExternalHelpPath))
				return View("External");

			ViewBag.FreeTagId = config.SpecialTags.Free;
			ViewBag.InstrumentalTagId = tagQueries.InstrumentalTagId;

			switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
			{
				case "ja":
					return View("Index.ja");
				case "zh":
					return View("Index.zh-Hans");
				default:
					return View();
			}
		}
	}
}
