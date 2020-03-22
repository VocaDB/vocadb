using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class VenueForApiContract : IEntryWithStatus {
		
		EntryType IEntryBase.EntryType => EntryType.Venue;
		string IEntryBase.DefaultName => Name;

		/// <summary>
		/// Comma-separated list of all other names that aren't the display name.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set; }

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public OptionalGeoPointContract Coordinates { get; set; }

		public bool Deleted { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ReleaseEventContract[] Events { get; set; }

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
		public string RegionCode { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public WebLinkForApiContract[] WebLinks { get; set; }

		public VenueForApiContract() { }

		public VenueForApiContract(Venue venue, ContentLanguagePreference languagePreference, VenueOptionalFields fields) {

			ParamIs.NotNull(() => venue);

			Id = venue.Id;
			Address = venue.Address;
			Coordinates = new OptionalGeoPointContract(venue.Coordinates);
			Deleted = venue.Deleted;
			Name = venue.TranslatedName[languagePreference];
			RegionCode = venue.RegionCode;
			Status = venue.Status;
			Version = venue.Version;

			if (fields.HasFlag(VenueOptionalFields.AdditionalNames)) {
				AdditionalNames = venue.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

			if (fields.HasFlag(VenueOptionalFields.Description)) {
				Description = venue.Description;
			}

			if (fields.HasFlag(VenueOptionalFields.Events)) {
				Events = venue.Events.OrderBy(e => e.SeriesNumber).ThenBy(e => e.Date.DateTime).Select(e => new ReleaseEventContract(e, languagePreference)).ToArray();
			}

			if (fields.HasFlag(VenueOptionalFields.Names)) {
				Names = venue.Names.Select(n => new LocalizedStringContract(n)).ToArray();
			}

			if (fields.HasFlag(VenueOptionalFields.WebLinks)) {
				WebLinks = venue.WebLinks.Links.Select(w => new WebLinkForApiContract(w, WebLinkOptionalFields.DescriptionOrUrl)).ToArray();
			}

		}

	}

	[Flags]
	public enum VenueOptionalFields {

		None = 0,
		AdditionalNames = 1,
		Description = 2,
		Events = 4,
		Names = 8,
		WebLinks = 16

	}

}
