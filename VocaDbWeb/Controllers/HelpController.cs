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
		private readonly VdbConfigManager _config;
		private readonly TagQueries _tagQueries;

		public HelpController(VdbConfigManager config, TagQueries tagQueries)
		{
			_config = config;
			_tagQueries = tagQueries;
		}

		//
		// GET: /Help/

		public ActionResult Index()
		{
			if (!string.IsNullOrEmpty(AppConfig.ExternalHelpPath))
				return View("External");

			ViewBag.FreeTagId = _config.SpecialTags.Free;
			ViewBag.InstrumentalTagId = _tagQueries.InstrumentalTagId;

			return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
			{
				"ja" => View("Index.ja"),
				"zh" => View("Index.zh-Hans"),
				_ => View(),
			};
		}
	}
}
