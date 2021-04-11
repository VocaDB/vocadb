#nullable disable

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongContract : IEntryWithStatus
	{
		string IEntryBase.DefaultName => Name;

		EntryType IEntryBase.EntryType => EntryType.Song;

		public SongContract() { }

		public SongContract(Song song, ContentLanguagePreference languagePreference, bool getThumbUrl = true)
			: this(song, languagePreference, string.Empty)
		{
			if (getThumbUrl)
			{
				ThumbUrl = song.GetThumbUrl();
			}
		}

		public SongContract(Song song, ContentLanguagePreference languagePreference, string thumbUrl)
		{
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
			ThumbUrl = thumbUrl ?? string.Empty;
		}

		[DataMember]
		public string AdditionalNames { get; init; }

		[DataMember]
		public string ArtistString { get; init; }

		[DataMember]
		public DateTime CreateDate { get; init; }

		[DataMember]
		public bool Deleted { get; init; }

		[DataMember]
		public int FavoritedTimes { get; init; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int LengthSeconds { get; init; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		public string NicoId { get; init; }

		[DataMember]
		public DateTime? PublishDate { get; init; }

		[DataMember]
		public PVServices PVServices { get; init; }

		[DataMember]
		public int RatingScore { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public SongType SongType { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryStatus Status { get; init; }

		[DataMember]
		public string ThumbUrl { get; init; }

		[DataMember]
		public int Version { get; init; }

#nullable enable
		public override string ToString()
		{
			return $"song '{Name}' [{Id}]";
		}
#nullable disable
	}
}
