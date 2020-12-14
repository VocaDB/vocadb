#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.Html
{
	public class LanguageFlagViewModel
	{
		public LanguageFlagViewModel(string languageCode)
		{
			LanguageCode = languageCode;
		}

		public string LanguageCode { get; set; }
	}
}