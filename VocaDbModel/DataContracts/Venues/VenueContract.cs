#nullable disable

using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues
{
	public class VenueContract : IEntryWithStatus
	{
		EntryType IEntryBase.EntryType => EntryType.Venue;
		string IEntryBase.DefaultName => Name;

		public string AdditionalNames { get; init; }

		public string Address { get; init; } = string.Empty;

		public string AddressCountryCode { get; init; } = string.Empty;

		public OptionalGeoPointContract Coordinates { get; init; }

		public bool Deleted { get; init; }

		public string Description { get; init; } = string.Empty;

		public int Id { get; set; }

		public string Name { get; init; }

		public EntryStatus Status { get; init; }

		public int Version { get; init; }

		public WebLinkContract[] WebLinks { get; init; }

		public VenueContract() { }

#nullable enable
		public VenueContract(Venue venue, ContentLanguagePreference languagePreference, bool includeLinks = false)
		{
			ParamIs.NotNull(() => venue);

			AdditionalNames = venue.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			Address = venue.Address;
			AddressCountryCode = venue.AddressCountryCode;
			Coordinates = new OptionalGeoPointContract(venue.Coordinates);
			Deleted = venue.Deleted;
			Description = venue.Description;
			Id = venue.Id;
			Name = venue.TranslatedName[languagePreference];
			Status = venue.Status;
			Version = venue.Version;

			if (includeLinks)
			{
				WebLinks = venue.WebLinks.Links.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();
			}
		}

		public override string ToString() => $"venue '{Name}' [{Id}]";
#nullable disable
	}
}
