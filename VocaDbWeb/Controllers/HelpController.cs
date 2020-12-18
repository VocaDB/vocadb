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

			return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
			{
				"ja" => View("Index.ja"),
				"zh" => View("Index.zh-Hans"),
				_ => View(),
			};
		}
	}
}
