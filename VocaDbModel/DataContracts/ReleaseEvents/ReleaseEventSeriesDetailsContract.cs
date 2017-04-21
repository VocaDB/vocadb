using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventSeriesDetailsContract : ReleaseEventSeriesWithEventsContract {

		public ReleaseEventSeriesDetailsContract() { }

		public ReleaseEventSeriesDetailsContract(ReleaseEventSeries series, ContentLanguagePreference languagePreference)
			: base(series, languagePreference) {

			AdditionalNames = series.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			TranslatedName = new TranslatedStringContract(series.TranslatedName);
			WebLinks = series.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		public string AdditionalNames { get; set; }

		public TranslatedStringContract TranslatedName { get; set; }

	}

}
