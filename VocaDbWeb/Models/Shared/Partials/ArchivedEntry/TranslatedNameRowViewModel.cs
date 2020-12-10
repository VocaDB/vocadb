using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{
	public class TranslatedNameRowViewModel
	{
		public TranslatedNameRowViewModel(ITranslatedString name, ITranslatedString compareToName = null)
		{
			Name = name;
			CompareToName = compareToName;
		}

		public ITranslatedString Name { get; set; }

		public ITranslatedString CompareToName { get; set; }
	}
}