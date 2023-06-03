#nullable disable

using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Web.Controllers;

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
			return ReactIndex.File(PageProperties);

		ViewBag.FreeTagId = _config.SpecialTags.Free;
		ViewBag.InstrumentalTagId = _tagQueries.InstrumentalTagId;

		switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
		{
			case "ja":
				PageProperties.Title = "サポート / DBについて";

				return ReactIndex.File(PageProperties);

			case "zh":
				PageProperties.Title = "Help / About";

				return ReactIndex.File(PageProperties);

			default:
				PageProperties.Title = "Help / About";

				return ReactIndex.File(PageProperties);
		}
	}
}
