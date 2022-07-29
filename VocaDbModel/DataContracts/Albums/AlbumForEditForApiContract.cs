using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Albums;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record AlbumForEditForApiContract
{
	[DataMember]
	public ArtistForAlbumContract[] ArtistLinks { get; set; }

	[DataMember]
	public bool CanDelete { get; init; }

	[DataMember]
	public string? CoverPictureMime { get; init; }

	[DataMember]
	public ContentLanguageSelection DefaultNameLanguage { get; init; }

	[DataMember]
	public bool Deleted { get; init; }

	[DataMember]
	public EnglishTranslatedStringContract Description { get; init; }

	[DataMember]
	public AlbumDiscPropertiesContract[] Discs { get; set; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public DiscType DiscType { get; init; }

	[DataMember]
	public int Id { get; set; }

	[DataMember]
	public string[] Identifiers { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public LocalizedStringWithIdContract[] Names { get; init; }

	[DataMember]
	public AlbumReleaseContract OriginalRelease { get; init; }

	[DataMember]
	public IList<EntryPictureFileContract> Pictures { get; init; }

	[DataMember(Name = "pvs")]
	public PVContract[] PVs { get; init; }

	[DataMember]
	public SongInAlbumEditContract[] Songs { get; set; }

	[DataMember]
	public EntryStatus Status { get; init; }

	[DataMember]
	public string UpdateNotes { get; set; }

	[DataMember]
	public WebLinkForApiContract[] WebLinks { get; init; }

	public AlbumForEditForApiContract()
	{
		ArtistLinks = Array.Empty<ArtistForAlbumContract>();
		Description = new EnglishTranslatedStringContract();
		Discs = Array.Empty<AlbumDiscPropertiesContract>();
		Identifiers = Array.Empty<string>();
		Name = string.Empty;
		Names = Array.Empty<LocalizedStringWithIdContract>();
		OriginalRelease = new();
		Pictures = Array.Empty<EntryPictureFileContract>();
		PVs = Array.Empty<PVContract>();
		Songs = Array.Empty<SongInAlbumEditContract>();
		UpdateNotes = string.Empty;
		WebLinks = Array.Empty<WebLinkForApiContract>();
	}

	public AlbumForEditForApiContract(
		Album album,
		ContentLanguagePreference languagePreference,
		IAggregatedEntryImageUrlFactory imageStore,
		IUserPermissionContext permissionContext
	)
	{
		ArtistLinks = album.Artists
			.Select(a => new ArtistForAlbumContract(a, languagePreference))
			.OrderBy(a => a.Name)
			.ToArray();
		CanDelete = EntryPermissionManager.CanDelete(permissionContext, album);
		CoverPictureMime = album.CoverPictureMime;
		DefaultNameLanguage = album.TranslatedName.DefaultLanguage;
		Deleted = album.Deleted;
		Description = new EnglishTranslatedStringContract(album.Description);
		Discs = album.Discs
			.Select(d => new AlbumDiscPropertiesContract(d))
			.ToArray();
		DiscType = album.DiscType;
		Id = album.Id;
		Identifiers = album.Identifiers
			.Select(i => i.Value)
			.ToArray();
		Name = album.TranslatedName[languagePreference];
		Names = album.Names
			.Select(n => new LocalizedStringWithIdContract(n))
			.ToArray();
		OriginalRelease = album.OriginalRelease is not null
			? new(album.OriginalRelease, languagePreference)
			: new();
		Pictures = album.Pictures
			.Select(p => new EntryPictureFileContract(p, imageStore))
			.ToList();
		PVs = album.PVs
			.Select(p => new PVContract(p))
			.ToArray();
		Songs = album.Songs
			.OrderBy(s => s.DiscNumber)
			.ThenBy(s => s.TrackNumber)
			.Select(s => new SongInAlbumEditContract(s, languagePreference))
			.ToArray();
		Status = album.Status;
		UpdateNotes = string.Empty;
		WebLinks = album.WebLinks
			.Select(w => new WebLinkForApiContract(w))
			.OrderBy(w => w.DescriptionOrUrl)
			.ToArray();
	}
}
