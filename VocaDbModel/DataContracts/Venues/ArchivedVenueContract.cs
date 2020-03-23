using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Utils;

namespace VocaDb.Model.DataContracts.Venues {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedVenueContract {

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public OptionalGeoPointContract Coordinates { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public string AddressCountryCode { get; set; }

		[DataMember]
		public ArchivedTranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

		public ArchivedVenueContract() { }

		public ArchivedVenueContract(Venue venue, VenueDiff diff) {

			ParamIs.NotNull(() => venue);

			Address = venue.Address;
			Coordinates = new OptionalGeoPointContract(venue.Coordinates);
			Description = venue.Description;
			Id = venue.Id;
			Names = diff.IncludeNames ? venue.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray() : null;
			AddressCountryCode = venue.AddressCountryCode;
			TranslatedName = new ArchivedTranslatedStringContract(venue.TranslatedName);
			WebLinks = diff.IncludeWebLinks ? venue.WebLinks.Links.Select(l => new ArchivedWebLinkContract(l)).ToArray() : null;

		}

		private static void DoIfExists(ArchivedVenueVersion version, VenueEditableFields field, XmlCache<ArchivedVenueContract> xmlCache, Action<ArchivedVenueContract> func) {

			var versionWithField = version.GetLatestVersionWithField(field);

			if (versionWithField?.Data != null) {
				var data = xmlCache.Deserialize(versionWithField.Version, versionWithField.Data);
				func(data);
			}

		}

		public static ArchivedVenueContract GetAllProperties(ArchivedVenueVersion version) {

			var data = new ArchivedVenueContract();
			var xmlCache = new XmlCache<ArchivedVenueContract>();
			var thisVersion = version.Data != null ? xmlCache.Deserialize(version.Version, version.Data) : new ArchivedVenueContract();

			data.Address = thisVersion.Address;
			data.Coordinates = thisVersion.Coordinates;
			data.Description = thisVersion.Description;
			data.Id = thisVersion.Id;
			data.AddressCountryCode = thisVersion.AddressCountryCode;
			data.TranslatedName = thisVersion.TranslatedName;

			DoIfExists(version, VenueEditableFields.Names, xmlCache, v => data.Names = v.Names);
			DoIfExists(version, VenueEditableFields.WebLinks, xmlCache, v => data.WebLinks = v.WebLinks);

			return data;

		}

	}

}
