using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record VenueForEditForApiContract
{
	[DataMember]
	public string Address { get; init; }

	[DataMember]
	public string AddressCountryCode { get; init; }

	[DataMember]
	public OptionalGeoPointContract? Coordinates { get; init; }

	[DataMember]
	public ContentLanguageSelection DefaultNameLanguage { get; init; }

	[DataMember]
	public bool Deleted { get; init; }

	[DataMember]
	public string Description { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public LocalizedStringWithIdContract[] Names { get; init; }

	[DataMember]
	public EntryStatus Status { get; init; }

	[DataMember]
	public WebLinkForApiContract[] WebLinks { get; init; }

	public VenueForEditForApiContract()
	{
		Address = string.Empty;
		AddressCountryCode = string.Empty;
		Description = string.Empty;
		Name = string.Empty;
		Names = Array.Empty<LocalizedStringWithIdContract>();
		WebLinks = Array.Empty<WebLinkForApiContract>();
	}

	public VenueForEditForApiContract(Venue venue, ContentLanguagePreference languagePreference)
	{
		Address = venue.Address;
		AddressCountryCode = venue.AddressCountryCode;
		Coordinates = new OptionalGeoPointContract(venue.Coordinates);
		DefaultNameLanguage = venue.TranslatedName.DefaultLanguage;
		Deleted = venue.Deleted;
		Description = venue.Description;
		Id = venue.Id;
		Name = venue.TranslatedName[languagePreference];
		Names = venue.Names
			.Select(n => new LocalizedStringWithIdContract(n))
			.ToArray();
		Status = venue.Status;
		WebLinks = venue.WebLinks.Links
			.Select(w => new WebLinkForApiContract(w))
			.OrderBy(w => w.DescriptionOrUrl)
			.ToArray();
	}
}
