using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Artists;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArtistDetailsForApiContract
{
	[DataMember]
	public string AdditionalNames { get; init; }

	[DataMember]
	public AdvancedArtistStatsContract? AdvancedStats { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public ArtistType ArtistType { get; init; }

	[DataMember]
	public TagBaseContract? ArtistTypeTag { get; init; }

	[DataMember]
	public ArtistForApiContract? BaseVoicebank { get; init; }

	[DataMember]
	public bool CanRemoveTagUsages { get; init; }

	[DataMember]
	public ArtistForApiContract? CharacterDesigner { get; init; }

	[DataMember]
	public ArtistForApiContract[] CharacterDesignerOf { get; init; }

	[DataMember]
	public ArtistForApiContract[] ChildVoicebanks { get; init; }

	[DataMember]
	public int CommentCount { get; init; }

	[DataMember]
	public DateTime CreateDate { get; init; }

	[DataMember]
	public bool Deleted { get; init; }

	[DataMember]
	public EnglishTranslatedStringContract Description { get; init; }

	[DataMember]
	public bool Draft { get; init; }

	/// <summary>
	/// Logged in user has subscribed to email notifications.
	/// </summary>
	[DataMember]
	public bool EmailNotifications { get; set; }

	[DataMember]
	public ArtistForApiContract[] Groups { get; init; }

	[DataMember]
	public int Id { get; set; }

	[DataMember]
	public ArtistForApiContract[] IllustratorOf { get; init; }

	[DataMember]
	public ArtistForApiContract[] Illustrators { get; init; }

	/// <summary>
	/// Logged in user is following this artist.
	/// </summary>
	[DataMember]
	public bool IsAdded { get; set; }

	[DataMember]
	public AlbumForApiContract[] LatestAlbums { get; set; } = Array.Empty<AlbumForApiContract>();

	[DataMember]
	public CommentForApiContract[] LatestComments { get; set; } = default!;

	[DataMember]
	public ReleaseEventForApiContract[] LatestEvents { get; set; } = Array.Empty<ReleaseEventForApiContract>();

	[DataMember]
	public SongForApiContract[] LatestSongs { get; set; } = Array.Empty<SongForApiContract>();

	[DataMember(EmitDefaultValue = false)]
	public EntryThumbForApiContract? MainPicture { get; init; }

	[DataMember]
	public ArtistForApiContract[] ManagerOf { get; init; }

	[DataMember]
	public ArtistForApiContract[] Managers { get; init; }

	[DataMember]
	public ArtistForApiContract[] Members { get; init; }

	[DataMember]
	public ArtistForApiContract? MergedTo { get; set; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public UserForApiContract[] OwnerUsers { get; init; }

	/// <summary>
	/// Personal stats for the logged in user.
	/// Can be null if the user isn't logged in.
	/// </summary>
	[DataMember]
	public PersonalArtistStatsContract? PersonalStats { get; init; }

	[DataMember]
	public EntryThumbForApiContract[] Pictures { get; init; }

	[DataMember]
	public DateTime? ReleaseDate { get; init; }

	[DataMember]
	public SharedArtistStatsContract SharedStats { get; init; } = default!;

	[DataMember]
	public bool SiteNotifications { get; set; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public EntryStatus Status { get; init; }

	[DataMember]
	public TagUsageForApiContract[] Tags { get; init; }

	[DataMember]
	public AlbumForApiContract[] TopAlbums { get; set; } = Array.Empty<AlbumForApiContract>();

	[DataMember]
	public SongForApiContract[] TopSongs { get; set; } = Array.Empty<SongForApiContract>();

	[DataMember]
	public int Version { get; init; }

	[DataMember]
	public ArtistForApiContract[] Voicebanks { get; init; }

	[DataMember]
	public ArtistForApiContract[] VoiceProviders { get; init; }

	[DataMember]
	public WebLinkForApiContract[] WebLinks { get; init; }

	public ArtistDetailsForApiContract(
		Artist artist,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext userContext,
		IAggregatedEntryImageUrlFactory imageStore,
		IUserIconFactory userIconFactory,
		Tag? artistTypeTag = null
	)
	{
		AdditionalNames = artist.Names.GetAdditionalNamesStringForLanguage(languagePreference);
		ArtistType = artist.ArtistType;

		ArtistTypeTag = artistTypeTag is not null
			? new TagBaseContract(artistTypeTag, languagePreference)
			: null;

		BaseVoicebank = artist.BaseVoicebank is not null
			? new ArtistForApiContract(
				artist: artist.BaseVoicebank,
				languagePreference: languagePreference,
				thumbPersister: null,
				includedFields: ArtistOptionalFields.None
			)
			: null;

		CanRemoveTagUsages = EntryPermissionManager.CanRemoveTagUsages(userContext, artist);
		CreateDate = artist.CreateDate;
		Deleted = artist.Deleted;
		Description = new EnglishTranslatedStringContract(artist.Description);
		Draft = artist.Status == EntryStatus.Draft;
		Id = artist.Id;

		MainPicture = artist.Thumb is not null
			? new EntryThumbForApiContract(image: artist.Thumb, thumbPersister: imageStore)
			: null;

		Name = artist.TranslatedName[languagePreference];

		OwnerUsers = artist.OwnerUsers
			.Select(u => new UserForApiContract(user: u.User, iconFactory: userIconFactory, optionalFields: UserOptionalFields.MainPicture))
			.ToArray();

		Pictures = artist.Pictures.Select(p => new EntryThumbForApiContract(image: p, thumbPersister: imageStore, name: p.Name)).ToArray();
		ReleaseDate = artist.ReleaseDate.DateTime;
		Status = artist.Status;
		Version = artist.Version;

		WebLinks = artist.WebLinks
			.OrderBy(w => w.DescriptionOrUrl)
			.Select(w => new WebLinkForApiContract(webLink: w))
			.ToArray();

		CharacterDesigner = artist
			.ArtistLinksOfType(ArtistLinkType.CharacterDesigner, LinkDirection.ManyToOne, allowInheritance: true)
			.Select(g => new ArtistForApiContract(
				artist: g,
				languagePreference: languagePreference,
				thumbPersister: null,
				includedFields: ArtistOptionalFields.None
			))
			.FirstOrDefault();

		CharacterDesignerOf = artist
			.ArtistLinksOfType(ArtistLinkType.CharacterDesigner, LinkDirection.OneToMany)
			.Select(g => new ArtistForApiContract(
				artist: g,
				languagePreference: languagePreference,
				thumbPersister: null,
				includedFields: ArtistOptionalFields.None
			))
			.OrderBy(a => a.Name)
			.ToArray();

		if (artist.CanHaveChildVoicebanks)
		{
			var children = artist.ChildVoicebanks
				.Where(c => !c.Deleted)
				.Select(c => new ArtistForApiContract(
					artist: c,
					languagePreference: languagePreference,
					thumbPersister: null,
					includedFields: ArtistOptionalFields.None
				))
				.ToArray();

			// Show child voicebanks with release date first
			ChildVoicebanks = children
				.Where(c => c.ReleaseDate.HasValue)
				.OrderBy(c => c.ReleaseDate)
				.Concat(children.Where(c => !c.ReleaseDate.HasValue))
				.ToArray();
		}
		else
			ChildVoicebanks = Array.Empty<ArtistForApiContract>();

		Groups = artist
			.ArtistLinksOfType(ArtistLinkType.Group, LinkDirection.ManyToOne)
			.Select(g => new ArtistForApiContract(
				artist: g,
				languagePreference: languagePreference,
				thumbPersister: null,
				includedFields: ArtistOptionalFields.None
			))
			.OrderBy(g => g.Name)
			.ToArray();

		IllustratorOf = artist
			.ArtistLinksOfType(ArtistLinkType.Illustrator, LinkDirection.OneToMany)
			.Select(g => new ArtistForApiContract(
				artist: g,
				languagePreference: languagePreference,
				thumbPersister: null,
				includedFields: ArtistOptionalFields.None
			))
			.OrderBy(a => a.Name)
			.ToArray();

		Illustrators = artist
			.ArtistLinksOfType(ArtistLinkType.Illustrator, LinkDirection.ManyToOne, allowInheritance: true)
			.Select(g => new ArtistForApiContract(
				artist: g,
				languagePreference: languagePreference,
				thumbPersister: null,
				includedFields: ArtistOptionalFields.None
			))
			.ToArray();

		ManagerOf = artist
			.ArtistLinksOfType(ArtistLinkType.Manager, LinkDirection.OneToMany)
			.Select(g => new ArtistForApiContract(
				artist: g,
				languagePreference: languagePreference,
				thumbPersister: null,
				includedFields: ArtistOptionalFields.None
			))
			.OrderBy(a => a.Name)
			.ToArray();

		Managers = artist
			.ArtistLinksOfType(ArtistLinkType.Manager, LinkDirection.ManyToOne, allowInheritance: true)
			.Select(g => new ArtistForApiContract(
				artist: g,
				languagePreference: languagePreference,
				thumbPersister: null,
				includedFields: ArtistOptionalFields.None
			))
			.OrderBy(a => a.Name)
			.ToArray();

		Members = artist
			.ArtistLinksOfType(ArtistLinkType.Group, LinkDirection.OneToMany)
			.Select(g => new ArtistForApiContract(
				artist: g,
				languagePreference: languagePreference,
				thumbPersister: imageStore,
				includedFields: ArtistOptionalFields.MainPicture
			))
			.OrderBy(g => g.Name)
			.ToArray();

		Tags = artist.Tags.ActiveUsages
			.Select(u => new TagUsageForApiContract(tagUsage: u, languagePreference: languagePreference))
			.OrderByDescending(t => t.Count)
			.ToArray();

		Voicebanks = artist
			.ArtistLinksOfType(ArtistLinkType.VoiceProvider, LinkDirection.OneToMany)
			.Select(g => new ArtistForApiContract(
				artist: g,
				languagePreference: languagePreference,
				thumbPersister: null,
				includedFields: ArtistOptionalFields.None
			))
			.OrderBy(a => a.Name)
			.ToArray();

		VoiceProviders = artist
			.ArtistLinksOfType(ArtistLinkType.VoiceProvider, LinkDirection.ManyToOne, allowInheritance: true)
			.Select(g => new ArtistForApiContract(
				artist: g,
				languagePreference: languagePreference,
				thumbPersister: null,
				includedFields: ArtistOptionalFields.None
			))
			.OrderBy(a => a.Name)
			.ToArray();
	}
}
