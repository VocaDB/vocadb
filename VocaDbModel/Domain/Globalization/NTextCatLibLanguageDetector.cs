#nullable disable

using IvanAkcheurov.NTextCat.Lib;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Model.Domain.Globalization;

public class NTextCatLibLanguageDetector : ILanguageDetector
{
	public NTextCatLibLanguageDetector(IHttpContext context)
	{
		_context = context;
	}

	private readonly IHttpContext _context;

	private string LanguageFilePath => Path.Combine(_context.ServerPathMapper.MapPath("~"), "..", "App_Data/Core14.profile.xml");

	public ContentLanguageSelection Detect(string str, ContentLanguageSelection def = ContentLanguageSelection.Unspecified)
	{
		var factory = new RankedLanguageIdentifierFactory();
		var identifier = factory.Load(LanguageFilePath);
		var res = identifier.Identify(str).FirstOrDefault();

		if (res == null)
			return def;

		var langCode = res.Item1.Iso639_2T;

		if (string.Equals(langCode, "jpn", StringComparison.InvariantCultureIgnoreCase))
			return ContentLanguageSelection.Japanese;

		if (string.Equals(langCode, "eng", StringComparison.InvariantCultureIgnoreCase))
			return ContentLanguageSelection.English;

		return def;
	}
}
