using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Artists;

[DataContract(Namespace = Schemas.VocaDb)]
public class ArtistForApiContract
{
#nullable disable
	public ArtistForApiContract() { }
#nullable enable

	public ArtistForApiContract(
		Artist artist,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		IAggregatedEntryImageUrlFactory? thumbPersister,
		ArtistOptionalFields includedFields
	)
	{
		ArtistType = artist.ArtistType;
		CreateDate = artist.CreateDate;
		DefaultName = artist.DefaultName;
		DefaultNameLanguage = artist.Names.SortNames.DefaultLanguage;
		Deleted = artist.Deleted;
		Id = artist.Id;
		Name = artist.Names.SortNames[languagePreference];
		PictureMime = artist.PictureMime;
		ReleaseDate = artist.ReleaseDate.DateTime;
		Status = artist.Status;
		Version = artist.Version;

		if (includedFields.HasFlag(ArtistOptionalFields.AdditionalNames))
		{
			AdditionalNames = artist.Names.GetAdditionalNamesStringForLanguage(languagePreference);
		}

		if (includedFields.HasFlag(ArtistOptionalFields.ArtistLinks))
			ArtistLinks = artist.Groups.Select(g => new ArtistForArtistForApiContract(g, LinkDirection.ManyToOne, languagePreference)).ToArray();

		if (includedFields.HasFlag(ArtistOptionalFields.ArtistLinksReverse))
			ArtistLinksReverse = artist.Members.Select(m => new ArtistForArtistForApiContract(m, LinkDirection.OneToMany, languagePreference)).ToArray();

		if (includedFields.HasFlag(ArtistOptionalFields.BaseVoicebank))
			BaseVoicebank = artist.BaseVoicebank != null ? new ArtistContract(artist.BaseVoicebank, languagePreference) : null;

		if (includedFields.HasFlag(ArtistOptionalFields.Description))
			Description = artist.Description[languagePreference];

		if (includedFields.HasFlag(ArtistOptionalFields.Names))
			Names = artist.Names.Select(n => new LocalizedStringContract(n)).ToArray();

		if (includedFields.HasFlag(ArtistOptionalFields.Tags))
			Tags = artist.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).ToArray();

		if (thumbPersister != null && includedFields.HasFlag(ArtistOptionalFields.MainPicture) && artist.Thumb != null)
		{
			MainPicture = new EntryThumbForApiContract(artist.Thumb, thumbPersister);
		}

		if (includedFields.HasFlag(ArtistOptionalFields.WebLinks))
			WebLinks = artist.WebLinks.Select(w => new WebLinkForApiContract(w)).ToArray();
	}

	/// <summary>
	/// Comma-separated list of all other names that aren't the display name.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public string? AdditionalNames { get; init; }

	/// <summary>
	/// List of artists linked to this artist, from child to parent. Optional field.
	/// This includes groups, illustrators and voice providers.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public ArtistForArtistForApiContract[]? ArtistLinks { get; init; }

	/// <summary>
	/// List of artists linked to this artist, from parent to child. Optional field.
	/// This includes group members, illustrations and voicebanks provided by this artist.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public ArtistForArtistForApiContract[]? ArtistLinksReverse { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public ArtistType ArtistType { get; init; }

	/// <summary>
	/// Base voicebank, if applicable and specified.
	/// </summary>
	/// <remarks>
	/// The field is optional to avoid loading the whole hierarchy when not needed.
	/// </remarks>
	[DataMember(EmitDefaultValue = false)]
	public ArtistContract? BaseVoicebank { get; init; }

	/// <summary>
	/// Date this entry was created.
	/// </summary>
	[DataMember]
	public DateTime CreateDate { get; init; }

	/// <summary>
	/// Name in default language.
	/// </summary>
	[DataMember]
	public string DefaultName { get; init; }

	/// <summary>
	/// Language selection of the original name.
	/// </summary>
	[DataMember]
	public ContentLanguageSelection DefaultNameLanguage { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public bool Deleted { get; init; }

	/// <summary>
	/// Description. Optional field.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public string? Description { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public EntryThumbForApiContract? MainPicture { get; init; }

	/// <summary>
	/// Id of the entry this entry was merged to, if any.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public int MergedTo { get; init; }

	/// <summary>
	/// Display name (primary name in selected language, or default language).
	/// </summary>
	[DataMember]
	public string Name { get; init; }

	/// <summary>
	/// List of all names for this entry. Optional field.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public LocalizedStringContract[]? Names { get; init; }

	/// <summary>
	/// MIME type for the main picture.
	/// </summary>
	[DataMember]
	public string? PictureMime { get; init; }

	/// <summary>
	/// Artist relations. Optional field.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public ArtistRelationsForApi? Relations { get; set; }

	[DataMember]
	public DateTime? ReleaseDate { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public EntryStatus Status { get; init; }

	/// <summary>
	/// List of tags. Optional field.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public TagUsageForApiContract[]? Tags { get; init; }

	[DataMember]
	public int Version { get; init; }

	/// <summary>
	/// List of external links. Optional field.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public WebLinkForApiContract[]? WebLinks { get; init; }
}

[Flags]
public enum ArtistOptionalFields
{
	None = 0,
	AdditionalNames = 1 << 0,
	ArtistLinks = 1 << 1,
	ArtistLinksReverse = 1 << 2,
	BaseVoicebank = 1 << 3,
	Description = 1 << 4,
	MainPicture = 1 << 5,
	Names = 1 << 6,
	Tags = 1 << 7,
	WebLinks = 1 << 8,
}

[DataContract(Namespace = Schemas.VocaDb)]
public class ArtistRelationsForApi
{
	[DataMember(EmitDefaultValue = false)]
	public AlbumForApiContract[]? LatestAlbums { get; set; }

	[DataMember(EmitDefaultValue = false)]
	public ReleaseEventForApiContract[]? LatestEvents { get; set; }

	[DataMember(EmitDefaultValue = false)]
	public SongForApiContract[]? LatestSongs { get; set; }

	[DataMember(EmitDefaultValue = false)]
	public AlbumForApiContract[]? PopularAlbums { get; set; }

	[DataMember(EmitDefaultValue = false)]
	public SongForApiContract[]? PopularSongs { get; set; }
}

[Flags]
public enum ArtistRelationsFields
{
	None = 0,
	LatestAlbums = 1 << 0,
	LatestEvents = 1 << 1,
	LatestSongs = 1 << 2,
	PopularAlbums = 1 << 3,
	PopularSongs = 1 << 4,
	All = LatestAlbums | LatestEvents | LatestSongs | PopularAlbums | PopularSongs,
}
