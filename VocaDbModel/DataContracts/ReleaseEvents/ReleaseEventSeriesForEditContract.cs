using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventSeriesForEditContract : ReleaseEventSeriesContract {

		public ReleaseEventSeriesForEditContract() {
			Names = new LocalizedStringWithIdContract[] {};
			WebLinks = new WebLinkContract[0];
		}

		public ReleaseEventSeriesForEditContract(ReleaseEventSeries series, ContentLanguagePreference languagePreference) : base(series, languagePreference) {

			DefaultNameLanguage = series.TranslatedName.DefaultLanguage;
			Names = series.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			WebLinks = series.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		public LocalizedStringWithIdContract[] Names { get; set; }

	}

}
