using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongContract : IEntryWithStatus {

		string IEntryBase.DefaultName => Name;

		EntryType IEntryBase.EntryType => EntryType.Song;

		public SongContract() {}

		public SongContract(Song song, ContentLanguagePreference languagePreference, bool getThumbUrl = true)
			: this(song, languagePreference, VocaDbUrl.Empty) {

			if (getThumbUrl) {
				ThumbUrl = song.GetThumbUrl().ToAbsolute().Url;
			}

		}

		public SongContract(Song song, ContentLanguagePreference languagePreference, VocaDbUrl thumbUrl) {

			ParamIs.NotNull(() => song);

			AdditionalNames = song.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			ArtistString = song.ArtistString.GetBestMatch(languagePreference);
			CreateDate = song.CreateDate;
			Deleted = song.Deleted;
			FavoritedTimes = song.FavoritedTimes;
			Id = song.Id;
			LengthSeconds = song.LengthSeconds;
			Name = song.TranslatedName[languagePreference];
			NicoId = song.NicoId;
			PublishDate = song.PublishDate.DateTime;
			PVServices = song.PVServices;
			RatingScore = song.RatingScore;
			SongType = song.SongType;
			Status = song.Status;
			Version = song.Version;
			ThumbUrl = (thumbUrl ?? VocaDbUrl.Empty).ToAbsolute().Url;

		}

		[DataMember]
		public string AdditionalNames { get; set; }

		[DataMember]
		public string ArtistString { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public bool Deleted { get; set; }

		[DataMember]
		public int FavoritedTimes { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int LengthSeconds { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string NicoId { get; set; }

		[DataMember]
		public DateTime? PublishDate { get; set; }

		[DataMember]
		public PVServices PVServices { get; set; }

		[DataMember]
		public int RatingScore { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public SongType SongType { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryStatus Status { get; set; }

		[DataMember]
		public string ThumbUrl { get; set; }

		[DataMember]
		public int Version { get; set; }

		public override string ToString() {
			return string.Format("song '{0}' [{1}]", Name, Id);
		}

	}

}
