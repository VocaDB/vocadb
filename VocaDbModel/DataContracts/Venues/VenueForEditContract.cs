#nullable disable

using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues;

[Obsolete]
public class VenueForEditContract : VenueContract
{
	public ContentLanguageSelection DefaultNameLanguage { get; init; }

	public LocalizedStringWithIdContract[] Names { get; init; }

	public VenueForEditContract() { }

	public VenueForEditContract(Venue venue, ContentLanguagePreference languagePreference) : base(venue, languagePreference, true)
	{
		DefaultNameLanguage = venue.TranslatedName.DefaultLanguage;
		Names = venue.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
	}
}
