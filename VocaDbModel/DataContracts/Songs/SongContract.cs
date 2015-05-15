using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongContract : IEntryWithStatus {

		string IEntryBase.DefaultName {
			get { return Name; }
		}

		EntryType IEntryBase.EntryType {
			get { return EntryType.Song; }
		}

		public SongContract() {}

		public SongContract(Song song, ContentLanguagePreference languagePreference, bool getThumbUrl = true)
			: this(song, languagePreference, string.Empty) {

			if (getThumbUrl) {
				// TODO: Used on the front page. Should be moved elsewhere.
				ThumbUrl = !string.IsNullOrEmpty(song.ThumbUrl) ? song.ThumbUrl : VideoServiceHelper.GetThumbUrl(song.PVs.PVs);
			}

		}

		public SongContract(Song song, ContentLanguagePreference languagePreference, string thumbUrl) {

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
			PublishDate = song.PublishDate != null ? (DateTime?)(DateTime.SpecifyKind(song.PublishDate.Value, DateTimeKind.Utc).Date) : null;
			PVServices = song.PVServices;
			RatingScore = song.RatingScore;
			SongType = song.SongType;
			Status = song.Status;
			Version = song.Version;
			ThumbUrl = thumbUrl ?? string.Empty;

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
