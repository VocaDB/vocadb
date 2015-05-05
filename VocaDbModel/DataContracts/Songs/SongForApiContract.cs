using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongForApiContract {

		public SongForApiContract() { }

		public SongForApiContract(Song song, SongMergeRecord mergeRecord, ContentLanguagePreference languagePreference, SongOptionalFields fields) {
			
			ArtistString = song.ArtistString[languagePreference];
			CreateDate = song.CreateDate;
			DefaultName = song.DefaultName;
			DefaultNameLanguage = song.Names.SortNames.DefaultLanguage;
			FavoritedTimes = song.FavoritedTimes;
			Id = song.Id;
			LengthSeconds = song.LengthSeconds;
			Name = song.Names.SortNames[languagePreference];
			PVServices = song.PVServices;
			RatingScore = song.RatingScore;
			SongType = song.SongType;
			Status = song.Status;
			Version = song.Version;

			if (languagePreference != ContentLanguagePreference.Default || fields.HasFlag(SongOptionalFields.AdditionalNames)) {
				AdditionalNames = song.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

			if (languagePreference != ContentLanguagePreference.Default) {
				LocalizedName = song.Names.SortNames[languagePreference];				
			}

			if (fields.HasFlag(SongOptionalFields.Albums))
				Albums = song.OnAlbums.Select(a => new AlbumContract(a, languagePreference)).ToArray();

			if (fields.HasFlag(SongOptionalFields.Artists))
				Artists = song.Artists.Select(a => new ArtistForSongContract(a, languagePreference)).ToArray();

			if (fields.HasFlag(SongOptionalFields.Lyrics))
				Lyrics = song.Lyrics.Select(l => new LyricsForSongContract(l)).ToArray();

			if (fields.HasFlag(SongOptionalFields.Names))
				Names = song.Names.Select(n => new LocalizedStringContract(n)).ToArray();

			if (song.HasOriginalVersion)
				OriginalVersionId = song.OriginalVersion.Id;

			if (fields.HasFlag(SongOptionalFields.PVs))
				PVs = song.PVs.Select(p => new PVContract(p)).ToArray();

			if (fields.HasFlag(SongOptionalFields.Tags))
				Tags = song.Tags.Usages.Select(u => new TagUsageForApiContract(u)).ToArray();

			if (fields.HasFlag(SongOptionalFields.ThumbUrl))
				ThumbUrl = VideoServiceHelper.GetThumbUrl(song.PVs.PVs);

			if (fields.HasFlag(SongOptionalFields.WebLinks))
				WebLinks = song.WebLinks.Select(w => new WebLinkContract(w)).ToArray();

			if (mergeRecord != null)
				MergedTo = mergeRecord.Target.Id;


		}

		public SongForApiContract(Song song, SongMergeRecord mergeRecord, ContentLanguagePreference languagePreference, 
			bool albums = true, bool artists = true, bool names = true, bool pvs = false, bool tags = true, bool thumbUrl = true, bool webLinks = false) {

			ArtistString = song.ArtistString[languagePreference];
			CreateDate = song.CreateDate;
			DefaultName = song.DefaultName;
			DefaultNameLanguage = song.Names.SortNames.DefaultLanguage;
			FavoritedTimes = song.FavoritedTimes;
			Id = song.Id;
			LengthSeconds = song.LengthSeconds;
			Name = song.Names.SortNames[languagePreference];
			PVServices = song.PVServices;
			RatingScore = song.RatingScore;
			SongType = song.SongType;
			Status = song.Status;
			Version = song.Version;

			if (languagePreference != ContentLanguagePreference.Default) {
				AdditionalNames = song.Names.GetAdditionalNamesStringForLanguage(languagePreference);
				LocalizedName = song.Names.SortNames[languagePreference];				
			}

			if (albums)
				Albums = song.Albums.Select(a => new AlbumContract(a.Album, languagePreference)).ToArray();

			if (artists)
				Artists = song.Artists.Select(a => new ArtistForSongContract(a, languagePreference)).ToArray();

			if (names)
				Names = song.Names.Select(n => new LocalizedStringContract(n)).ToArray();

			if (song.HasOriginalVersion)
				OriginalVersionId = song.OriginalVersion.Id;

			if (pvs)
				PVs = song.PVs.Select(p => new PVContract(p)).ToArray();

			if (tags)
				Tags = song.Tags.Usages.Select(u => new TagUsageForApiContract(u)).ToArray();

			if (thumbUrl)
				ThumbUrl = VideoServiceHelper.GetThumbUrl(song.PVs.PVs);

			if (webLinks)
				WebLinks = song.WebLinks.Select(w => new WebLinkContract(w)).ToArray();

			if (mergeRecord != null)
				MergedTo = mergeRecord.Target.Id;

		}

		/// <summary>
		/// Comma-separated list of all other names that aren't the display name.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set;}

		/// <summary>
		/// List of albums this song appears on. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public AlbumContract[] Albums { get; set; }

		/// <summary>
		/// List of artists for this song. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public ArtistForSongContract[] Artists { get; set; }

		/// <summary>
		/// Artist string, for example "Tripshots feat. Hatsune Miku".
		/// </summary>
		[DataMember]
		public string ArtistString { get; set; }

		/// <summary>
		/// Date this entry was created.
		/// </summary>
		[DataMember]
		public DateTime CreateDate { get; set; }

		/// <summary>
		/// Name in default language.
		/// </summary>
		[DataMember]
		public string DefaultName { get; set; }

		/// <summary>
		/// Language selection of the original name.
		/// </summary>
		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		/// <summary>
		/// Number of times this song has been favorited.
		/// </summary>
		[DataMember]
		public int FavoritedTimes { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int LengthSeconds { get; set; }

		[Obsolete("Use Name")]
		[DataMember(EmitDefaultValue = false)]
		public string LocalizedName { get; set; }

		/// <summary>
		/// List of lyrics. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public LyricsForSongContract[] Lyrics { get; set; }

		/// <summary>
		/// Id of the song this song was merged to, if any.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public int MergedTo { get; set; }

		/// <summary>
		/// Display name (primary name in selected language, or default language).
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// List of all names for this entry. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringContract[] Names { get; set; }

		/// <summary>
		/// Id of the original (parent) song, if any.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public int OriginalVersionId { get; set; }

		/// <summary>
		/// List of PVs. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public PVContract[] PVs { get; set; }

		/// <summary>
		/// List of streaming services this song has PVs for.
		/// </summary>
		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public PVServices PVServices { get; set; }

		/// <summary>
		/// Total sum of ratings.
		/// </summary>
		[DataMember]
		public int RatingScore { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public SongType SongType { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryStatus Status { get; set; }

		/// <summary>
		/// List of tags. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public TagUsageForApiContract[] Tags { get; set; }

		/// <summary>
		/// URL to the thumbnail. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string ThumbUrl { get; set; }

		[DataMember]
		public int Version { get; set; }

		/// <summary>
		/// List of external links. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public WebLinkContract[] WebLinks { get; set; }

	}

	[Flags]
	public enum SongOptionalFields {

		None = 0,
		AdditionalNames = 1,
		Albums = 2,
		Artists = 4,
		Lyrics = 8,
		Names = 16,
		PVs = 32,
		Tags = 64,
		ThumbUrl = 128,
		WebLinks = 256

	}

}
