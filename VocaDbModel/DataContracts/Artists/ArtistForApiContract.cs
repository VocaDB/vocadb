using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForApiContract {

		public ArtistForApiContract() { }

		public ArtistForApiContract(Artist artist, 
			ContentLanguagePreference languagePreference, 
			IEntryThumbPersister thumbPersister,
			bool ssl,
			ArtistOptionalFields includedFields) {

			ArtistType = artist.ArtistType;
			BaseVoicebank = artist.BaseVoicebank != null ? new ArtistContract(artist.BaseVoicebank, languagePreference) : null;
			CreateDate = artist.CreateDate;
			DefaultName = artist.DefaultName;
			DefaultNameLanguage = artist.Names.SortNames.DefaultLanguage;
			Id = artist.Id;
			Name = artist.Names.SortNames[languagePreference];				
			PictureMime = artist.PictureMime;
			Status = artist.Status;
			Version = artist.Version;

			if (includedFields.HasFlag(ArtistOptionalFields.AdditionalNames)) {
				AdditionalNames = artist.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

			if (includedFields.HasFlag(ArtistOptionalFields.Description))
				Description = artist.Description[languagePreference];

			if (includedFields.HasFlag(ArtistOptionalFields.Groups))
				Groups = artist.Groups.Select(g => new ArtistContract(g.Group, languagePreference)).ToArray();

			if (includedFields.HasFlag(ArtistOptionalFields.Members))
				Members = artist.Members.Select(m => new ArtistContract(m.Member, languagePreference)).ToArray();

			if (includedFields.HasFlag(ArtistOptionalFields.Names))
				Names = artist.Names.Select(n => new LocalizedStringContract(n)).ToArray();

			if (includedFields.HasFlag(ArtistOptionalFields.Tags))
				Tags = artist.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).ToArray();

			if (thumbPersister != null && includedFields.HasFlag(ArtistOptionalFields.MainPicture) && !string.IsNullOrEmpty(artist.PictureMime)) {
				
				MainPicture = new EntryThumbForApiContract(new EntryThumb(artist, artist.PictureMime), thumbPersister, ssl);

			}

			if (includedFields.HasFlag(ArtistOptionalFields.WebLinks))
				WebLinks = artist.WebLinks.Select(w => new WebLinkForApiContract(w)).ToArray();

		}

		/// <summary>
		/// Comma-separated list of all other names that aren't the display name.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set;}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistType ArtistType { get; set; }

		/// <summary>
		/// Base voicebank, if applicable and specified.
		/// </summary>
		[DataMember]
		public ArtistContract BaseVoicebank { get; set; }

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
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		/// <summary>
		/// Description. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		/// <summary>
		/// List of groups this artist belongs in. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public ArtistContract[] Groups { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		/// <summary>
		/// If this artist is a group, list of members in this group.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public ArtistContract[] Members { get; set; }

		/// <summary>
		/// Id of the entry this entry was merged to, if any.
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
		/// MIME type for the main picture.
		/// </summary>
		[DataMember]
		public string PictureMime { get; set;}

		/// <summary>
		/// Artist relations. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public ArtistRelationsForApi Relations { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryStatus Status { get; set; }

		/// <summary>
		/// List of tags. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public TagUsageForApiContract[] Tags { get; set; }

		[DataMember]
		public int Version { get; set; }

		/// <summary>
		/// List of external links. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public WebLinkForApiContract[] WebLinks { get; set; }

	}

	[Flags]
	public enum ArtistOptionalFields {

		None = 0,
		AdditionalNames = 1,
		Description = 2,
		Groups = 4,
		MainPicture = 8,
		Members = 16,
		Names = 32,
		Tags = 64,
		WebLinks = 128

	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistRelationsForApi {

		[DataMember(EmitDefaultValue = false)]
		public AlbumContract[] LatestAlbums { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public SongContract[] LatestSongs { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public AlbumContract[] PopularAlbums { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public SongContract[] PopularSongs { get; set; }

	}

	[Flags]
	public enum ArtistRelationsFields {
		
		None = 0,
		LatestAlbums = 1,
		LatestSongs = 2,
		PopularAlbums = 4,
		PopularSongs = 8,
		All = (LatestAlbums | LatestSongs | PopularAlbums | PopularSongs)

	}

}
