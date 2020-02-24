using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues {

	public class VenueContract : IEntryWithStatus {

		EntryType IEntryBase.EntryType => EntryType.Venue;
		string IEntryBase.DefaultName => Name;

		public string AdditionalNames { get; set; }

		public OptionalGeoPointContract Coordinates { get; set; }

		public bool Deleted { get; set; }

		public string Description { get; set; } = string.Empty;

		public int Id { get; set; }

		public string Name { get; set; }

		public EntryStatus Status { get; set; }

		public int Version { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

		public VenueContract() { }

		public VenueContract(Venue venue, ContentLanguagePreference languagePreference, bool includeLinks = false) {

			ParamIs.NotNull(() => venue);

			AdditionalNames = venue.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			Coordinates = new OptionalGeoPointContract(venue.Coordinates);
			Deleted = venue.Deleted;
			Description = venue.Description;
			Id = venue.Id;
			Name = venue.TranslatedName[languagePreference];
			Status = venue.Status;
			Version = venue.Version;

			if (includeLinks) {
				WebLinks = venue.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();
			}

		}

	}

}
