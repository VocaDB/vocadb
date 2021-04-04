#nullable disable

using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.Artists
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistContract : IEntryWithStatus, IEntryImageInformation
	{
		string IEntryBase.DefaultName => Name;

		EntryType IEntryBase.EntryType => EntryType.Artist;

		EntryType IEntryImageInformation.EntryType => EntryType.Artist;
		string IEntryImageInformation.Mime => PictureMime;
		ImagePurpose IEntryImageInformation.Purpose => ImagePurpose.Main;

		public ArtistContract() { }

#nullable enable
		public ArtistContract(Artist artist, ContentLanguagePreference preference)
		{
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

		public ArtistContract(TranslatedArtistContract artist, ContentLanguagePreference preference)
		{
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
#nullable disable

		[DataMember]
		public string AdditionalNames { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistType ArtistType { get; init; }

		[DataMember]
		public bool Deleted { get; init; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		public string PictureMime { get; set; }

		[DataMember]
		public DateTime? ReleaseDate { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryStatus Status { get; init; }

		[DataMember]
		public int Version { get; init; }

		public override string ToString()
		{
			return $"Artist {Name} [{Id}]";
		}
	}
}
