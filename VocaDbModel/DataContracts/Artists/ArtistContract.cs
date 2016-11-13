using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistContract : IEntryWithStatus, IEntryImageInformation {

		string IEntryBase.DefaultName => Name;

		EntryType IEntryBase.EntryType => EntryType.Artist;

		EntryType IEntryImageInformation.EntryType => EntryType.Artist;

		string IEntryImageInformation.Mime => PictureMime;

		public ArtistContract() {}

		public ArtistContract(Artist artist, ContentLanguagePreference preference) {

			ParamIs.NotNull(() => artist);

			AdditionalNames = artist.Names.GetAdditionalNamesStringForLanguage(preference);
			ArtistType = artist.ArtistType;
			Deleted = artist.Deleted;
			Id = artist.Id;
			Name = artist.TranslatedName[preference];
			PictureMime = artist.PictureMime;
			ReleaseDate = artist.ReleaseDate.DateTime;
			Status = artist.Status;
			Version = artist.Version;

		}

		public ArtistContract(TranslatedArtistContract artist, ContentLanguagePreference preference) {

			ParamIs.NotNull(() => artist);

			AdditionalNames = artist.Names.GetAdditionalNamesStringForLanguage(preference);
			ArtistType = artist.ArtistType;
			Deleted = artist.Deleted;
			Id = artist.Id;
			Name = artist.Names.SortNames[preference];
			PictureMime = artist.PictureMime;
			ReleaseDate = artist.ReleaseDate;
			Status = artist.Status;
			Version = artist.Version;

		}

		[DataMember]
		public string AdditionalNames { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistType ArtistType { get; set; }

		[DataMember]
		public bool Deleted { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PictureMime { get; set;}

		[DataMember]
		public DateTime? ReleaseDate { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryStatus Status { get; set; }

		[DataMember]
		public int Version { get; set; }

		public override string ToString() {
			return string.Format("Artist {0} [{1}]", Name, Id);
		}

	}

}
