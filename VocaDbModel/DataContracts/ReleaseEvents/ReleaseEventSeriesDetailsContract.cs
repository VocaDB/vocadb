#nullable disable

using System.Linq;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ReleaseEventSeriesDetailsContract : ReleaseEventSeriesWithEventsContract
	{
		public ReleaseEventSeriesDetailsContract() { }

		public ReleaseEventSeriesDetailsContract(ReleaseEventSeries series, ContentLanguagePreference languagePreference)
			: base(series, languagePreference)
		{
			Tags = series.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).OrderByDescending(t => t.Count).ToArray();
			TranslatedName = new TranslatedStringContract(series.TranslatedName);
			WebLinks = series.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();
		}

		public TagUsageForApiContract[] Tags { get; set; }

		public TranslatedStringContract TranslatedName { get; set; }
	}
}
