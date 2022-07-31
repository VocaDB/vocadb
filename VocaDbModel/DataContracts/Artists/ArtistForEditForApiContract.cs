using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Artists;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArtistForEditForApiContract
{
	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public ArtistType ArtistType { get; init; }

	[DataMember]
	public ArtistForArtistContract[] AssociatedArtists { get; init; }

	[DataMember]
	public ArtistContract? BaseVoicebank { get; init; }

	[DataMember]
	public bool CanDelete { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public bool Deleted { get; init; }

	[DataMember]
	public EnglishTranslatedStringContract Description { get; init; }

	[DataMember]
	public ContentLanguageSelection DefaultNameLanguage { get; set /* TODO: init */; }

	[DataMember]
	public ArtistForArtistContract[] Groups { get; init; }

	[DataMember]
	public int Id { get; set /* TODO: init */; }

	[DataMember]
	public ArtistContract? Illustrator { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public LocalizedStringWithIdContract[] Names { get; init; }

	[DataMember]
	public string? PictureMime { get; set; }

	[DataMember]
	public IList<EntryPictureFileContract> Pictures { get; init; }

	[DataMember]
	public DateTime? ReleaseDate { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public EntryStatus Status { get; init; }

	[DataMember]
	public string UpdateNotes { get; set; }

	[DataMember]
	public int Version { get; init; }

	[DataMember]
	public ArtistContract? VoiceProvider { get; init; }

	[DataMember]
	public WebLinkForApiContract[] WebLinks { get; init; }

	public ArtistForEditForApiContract()
	{
		AssociatedArtists = Array.Empty<ArtistForArtistContract>();
		Description = new();
		Groups = Array.Empty<ArtistForArtistContract>();
		Name = string.Empty;
		Names = Array.Empty<LocalizedStringWithIdContract>();
		Pictures = Array.Empty<EntryPictureFileContract>();
		UpdateNotes = string.Empty;
		WebLinks = Array.Empty<WebLinkForApiContract>();
	}

	public ArtistForEditForApiContract(
		Artist artist,
		ContentLanguagePreference languagePreference,
		IAggregatedEntryImageUrlFactory imageStore,
		IUserPermissionContext permissionContext
	)
	{
		ArtistType = artist.ArtistType;
		BaseVoicebank = artist.BaseVoicebank is not null
			? new ArtistContract(artist.BaseVoicebank, languagePreference)
			: null;
		CanDelete = EntryPermissionManager.CanDelete(permissionContext, artist);
		DefaultNameLanguage = artist.TranslatedName.DefaultLanguage;
		Deleted = artist.Deleted;
		Description = new EnglishTranslatedStringContract(artist.Description);
		Groups = artist.Groups
			.Where(g => g.LinkType == ArtistLinkType.Group)
			.Select(g => new ArtistForArtistContract(g, languagePreference))
			.OrderBy(g => g.Parent.Name)
			.ToArray();
		Id = artist.Id;
		Illustrator = artist.ArtistLinksOfType(ArtistLinkType.Illustrator, LinkDirection.ManyToOne)
			.Select(g => new ArtistContract(g, languagePreference))
			.FirstOrDefault();
		Name = artist.Names.SortNames[languagePreference];
		Names = artist.Names
			.Select(n => new LocalizedStringWithIdContract(n))
			.ToArray();
		PictureMime = artist.PictureMime;
		Pictures = artist.Pictures
			.Select(p => new EntryPictureFileContract(p, imageStore))
			.ToList();
		ReleaseDate = artist.ReleaseDate.DateTime;
		Status = artist.Status;
		UpdateNotes = string.Empty;
		Version = artist.Version;
		VoiceProvider = artist.ArtistLinksOfType(ArtistLinkType.VoiceProvider, LinkDirection.ManyToOne)
			.Select(g => new ArtistContract(g, languagePreference))
			.FirstOrDefault();
		WebLinks = artist.WebLinks
			.Select(w => new WebLinkForApiContract(w))
			.OrderBy(w => w.DescriptionOrUrl)
			.ToArray();

		AssociatedArtists = artist.Groups
			.Where(
				a =>
					a.LinkType != ArtistLinkType.Group &&
					(a.Parent.Id != Illustrator?.Id || a.LinkType != ArtistLinkType.Illustrator) &&
					(a.Parent.Id != VoiceProvider?.Id || a.LinkType != ArtistLinkType.VoiceProvider)
			)
			.Select(g => new ArtistForArtistContract(g, languagePreference))
			.ToArray();
	}
}
