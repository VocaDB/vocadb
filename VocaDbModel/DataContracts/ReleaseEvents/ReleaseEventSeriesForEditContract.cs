#nullable disable

using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents;

[Obsolete]
public class ReleaseEventSeriesForEditContract : ReleaseEventSeriesContract
{
	public ReleaseEventSeriesForEditContract()
	{
		Names = Array.Empty<LocalizedStringWithIdContract>();
		WebLinks = Array.Empty<WebLinkContract>();
	}

	public ReleaseEventSeriesForEditContract(ReleaseEventSeries series, ContentLanguagePreference languagePreference) : base(series, languagePreference)
	{
		DefaultNameLanguage = series.TranslatedName.DefaultLanguage;
		Names = series.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
		WebLinks = series.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();
	}

	public ContentLanguageSelection DefaultNameLanguage { get; init; }

	public LocalizedStringWithIdContract[] Names { get; init; }
}
