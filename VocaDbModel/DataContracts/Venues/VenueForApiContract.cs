using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues {

	public class VenueForApiContract {
		
		/// <summary>
		/// Comma-separated list of all other names that aren't the display name.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// List of all names for this entry. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public WebLinkForApiContract[] WebLinks { get; set; }

		public VenueForApiContract() { }

		public VenueForApiContract(Venue venue, ContentLanguagePreference languagePreference, VenueOptionalFields fields) {

			Id = venue.Id;
			Name = venue.TranslatedName[languagePreference];
			Status = venue.Status;
			Version = venue.Version;

			if (fields.HasFlag(VenueOptionalFields.AdditionalNames)) {
				AdditionalNames = venue.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

			if (fields.HasFlag(VenueOptionalFields.Description)) {
				Description = venue.Description;
			}

			if (fields.HasFlag(VenueOptionalFields.Names)) {
				Names = venue.Names.Select(n => new LocalizedStringContract(n)).ToArray();
			}

			if (fields.HasFlag(VenueOptionalFields.WebLinks)) {
				WebLinks = venue.WebLinks.Select(w => new WebLinkForApiContract(w)).ToArray();
			}

		}

	}

	[Flags]
	public enum VenueOptionalFields {

		None = 0,
		AdditionalNames = 1,
		Description = 2,
		Names = 4,
		WebLinks = 8

	}

}
