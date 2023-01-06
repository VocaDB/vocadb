#nullable disable

using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents;

[Obsolete]
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

	public TagUsageForApiContract[] Tags { get; init; }

	public TranslatedStringContract TranslatedName { get; init; }
}
