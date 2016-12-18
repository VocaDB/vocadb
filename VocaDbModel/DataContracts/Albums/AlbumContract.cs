using System;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumContract : IEntryWithStatus, IEquatable<AlbumContract>, IEntryImageInformation {

		string IEntryBase.DefaultName => Name;

		bool IDeletableEntry.Deleted => false;

		EntryType IEntryBase.EntryType => EntryType.Album;

		EntryType IEntryImageInformation.EntryType => EntryType.Album;

		string IEntryImageInformation.Mime => CoverPictureMime;

		public AlbumContract() { }

		public AlbumContract(Album album, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => album);

			AdditionalNames = album.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			ArtistString = album.ArtistString.GetBestMatch(languagePreference);
			CoverPictureMime = album.CoverPictureMime;
			CreateDate = album.CreateDate;
			Deleted = album.Deleted;
			DiscType = album.DiscType;
			Id = album.Id;
			Name = album.TranslatedName[languagePreference];
			RatingAverage = album.RatingAverage;
			RatingCount = album.RatingCount;
			ReleaseDate = new OptionalDateTimeContract(album.OriginalReleaseDate);
			ReleaseEvent = album.OriginalReleaseEvent != null ? new ReleaseEventForApiContract(album.OriginalReleaseEvent, ReleaseEventOptionalFields.None) : null;
			Status = album.Status;
			Version = album.Version;

		}

		public AlbumContract(TranslatedAlbumContract album, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => album);

			AdditionalNames = album.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			ArtistString = album.TranslatedArtistString.GetBestMatch(languagePreference);
			CoverPictureMime = album.CoverPictureMime;
			CreateDate = album.CreateDate;
			Deleted = album.Deleted;
			DiscType = album.DiscType;
			Id = album.Id;
			Name = album.Names.SortNames[languagePreference];
			RatingAverage = album.RatingAverage;
			RatingCount = album.RatingCount;
			ReleaseDate = album.ReleaseDate;
			ReleaseEvent = album.ReleaseEvent;
			Status = album.Status;
			Version = album.Version;

		}

		[DataMember]
		public string AdditionalNames { get; set; }

		[DataMember]
		public string ArtistString { get; set; }

		[DataMember]
		public string CoverPictureMime { get; set;}

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public bool Deleted { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public DiscType DiscType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public double RatingAverage { get; set; }

		[DataMember]
		public int RatingCount { get; set; }

		/// <summary>
		/// Release date. Cannot be null (but can be empty).
		/// </summary>
		[DataMember]
		public OptionalDateTimeContract ReleaseDate { get; set; }

		[DataMember]
		public ReleaseEventForApiContract ReleaseEvent { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public int Version { get; set; }

		public bool Equals(AlbumContract another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

	    public override bool Equals(object obj) {
	        return Equals(obj as AlbumContract);
	    }

	    public override int GetHashCode() {
	        return Id.GetHashCode();
	    }

		public override string ToString() {
			return string.Format("album '{0}' [{1}]", Name, Id);
		}

	}

}
