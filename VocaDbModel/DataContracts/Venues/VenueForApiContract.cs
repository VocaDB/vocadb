#nullable disable

using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class VenueForApiContract : IEntryWithStatus
	{
		EntryType IEntryBase.EntryType => EntryType.Venue;
		string IEntryBase.DefaultName => Name;

		/// <summary>
		/// Comma-separated list of all other names that aren't the display name.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; init; }

		/// <summary>
		/// Venue address, without country, for example "2-1, Nakase, Mihama-ku, Chiba-city, 261-8550".
		/// </summary>
		[DataMember]
		public string Address { get; init; }

		/// <summary>
		/// The two-letter code defined in ISO 3166 for the country/region.
		/// </summary>
		[DataMember]
		public string AddressCountryCode { get; init; }

		[DataMember]
		public OptionalGeoPointContract Coordinates { get; init; }

		public bool Deleted { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public ReleaseEventContract[] Events { get; init; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; init; }

		/// <summary>
		/// List of all names for this entry. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringContract[] Names { get; init; }

		[DataMember]
		public EntryStatus Status { get; init; }

		[DataMember]
		public int Version { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public WebLinkForApiContract[] WebLinks { get; init; }

		public VenueForApiContract() { }

		public VenueForApiContract(Venue venue, ContentLanguagePreference languagePreference, VenueOptionalFields fields)
		{
			ParamIs.NotNull(() => venue);

			Id = venue.Id;
			Address = venue.Address;
			AddressCountryCode = venue.AddressCountryCode;
			Coordinates = new OptionalGeoPointContract(venue.Coordinates);
			Deleted = venue.Deleted;
			Name = venue.TranslatedName[languagePreference];
			Status = venue.Status;
			Version = venue.Version;

			if (fields.HasFlag(VenueOptionalFields.AdditionalNames))
			{
				AdditionalNames = venue.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

			if (fields.HasFlag(VenueOptionalFields.Description))
			{
				Description = venue.Description;
			}

			if (fields.HasFlag(VenueOptionalFields.Events))
			{
				Events = venue.Events.OrderBy(e => e.Date.DateTime).ThenBy(e => e.SeriesNumber).Select(e => new ReleaseEventContract(e, languagePreference)).ToArray();
			}

			if (fields.HasFlag(VenueOptionalFields.Names))
			{
				Names = venue.Names.Select(n => new LocalizedStringContract(n)).ToArray();
			}

			if (fields.HasFlag(VenueOptionalFields.WebLinks))
			{
				WebLinks = venue.WebLinks.Links.Select(w => new WebLinkForApiContract(w, WebLinkOptionalFields.DescriptionOrUrl)).ToArray();
			}
		}
	}

	[Flags]
	public enum VenueOptionalFields
	{
		None = 0,
		AdditionalNames = 1 << 0,
		Description = 1 << 1,
		Events = 1 << 2,
		Names = 1 << 3,
		WebLinks = 1 << 4,
	}
}
