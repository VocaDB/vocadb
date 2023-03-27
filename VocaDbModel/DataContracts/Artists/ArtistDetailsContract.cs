#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
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

[Obsolete]
[DataContract(Namespace = Schemas.VocaDb)]
public class ArtistDetailsContract : ArtistContract
{
	public ArtistDetailsContract() { }

	public ArtistDetailsContract(
		Artist artist,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext userContext,
		IAggregatedEntryImageUrlFactory imageStore,
		IUserIconFactory userIconFactory,
		Tag artistTypeTag = null
	)
		: base(artist, languagePreference)
	{
		AllNames = string.Join(", ", artist.AllNames.Where(n => n != Name));
		ArtistTypeTag = artistTypeTag != null ? new TagBaseContract(artistTypeTag, languagePreference) : null;
		BaseVoicebank = artist.BaseVoicebank != null ? new ArtistContract(artist.BaseVoicebank, languagePreference) : null;
		CanRemoveTagUsages = EntryPermissionManager.CanRemoveTagUsages(userContext, artist);
		CreateDate = artist.CreateDate;
		Description = artist.Description;
		Draft = artist.Status == EntryStatus.Draft;
		TranslatedName = new TranslatedStringContract(artist.TranslatedName);
		LatestAlbums = Array.Empty<AlbumForApiContract>();
		LatestSongs = Array.Empty<SongForApiContract>();
		OwnerUsers = artist.OwnerUsers.Select(u => new UserForApiContract(u.User, userIconFactory, UserOptionalFields.MainPicture)).ToArray();
		Pictures = artist.Pictures.Select(p => new EntryPictureFileContract(p, imageStore)).ToArray();
		TopAlbums = Array.Empty<AlbumForApiContract>();
		TopSongs = Array.Empty<SongForApiContract>();
		WebLinks = artist.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		CharacterDesigner = artist.ArtistLinksOfType(ArtistLinkType.CharacterDesigner, LinkDirection.ManyToOne, allowInheritance: true)
			.Select(g => new ArtistContract(g, languagePreference)).FirstOrDefault();

		CharacterDesignerOf = artist.ArtistLinksOfType(ArtistLinkType.CharacterDesigner, LinkDirection.OneToMany)
			.Select(g => new ArtistContract(g, languagePreference)).OrderBy(a => a.Name).ToArray();

		if (artist.CanHaveChildVoicebanks)
		{
			var children = artist.ChildVoicebanks
				.Where(c => !c.Deleted)
				.Select(c => new ArtistContract(c, languagePreference))
				.ToArray();

			// Show child voicebanks with release date first
			ChildVoicebanks = children
				.Where(c => c.ReleaseDate.HasValue)
				.OrderBy(c => c.ReleaseDate)
				.Concat(children.Where(c => !c.ReleaseDate.HasValue))
				.ToArray();
		}
		else
		{
			ChildVoicebanks = Array.Empty<ArtistContract>();
		}

		Groups = artist.ArtistLinksOfType(ArtistLinkType.Group, LinkDirection.ManyToOne)
			.Select(g => new ArtistContract(g, languagePreference)).OrderBy(g => g.Name).ToArray();

		Illustrators = artist.ArtistLinksOfType(ArtistLinkType.Illustrator, LinkDirection.ManyToOne, allowInheritance: true)
			.Select(g => new ArtistContract(g, languagePreference)).ToArray();

		IllustratorOf = artist.ArtistLinksOfType(ArtistLinkType.Illustrator, LinkDirection.OneToMany)
			.Select(g => new ArtistContract(g, languagePreference)).OrderBy(a => a.Name).ToArray();

		Managers = artist.ArtistLinksOfType(ArtistLinkType.Manager, LinkDirection.ManyToOne, allowInheritance: true)
			.Select(g => new ArtistContract(g, languagePreference)).OrderBy(a => a.Name).ToArray();

		ManagerOf = artist.ArtistLinksOfType(ArtistLinkType.Manager, LinkDirection.OneToMany)
			.Select(g => new ArtistContract(g, languagePreference)).OrderBy(a => a.Name).ToArray();

		Members = artist.ArtistLinksOfType(ArtistLinkType.Group, LinkDirection.OneToMany)
			.Select(g => new ArtistContract(g, languagePreference)).OrderBy(g => g.Name).ToArray();

		Tags = artist.Tags.ActiveUsages
			.Select(u => new TagUsageForApiContract(u, languagePreference))
			.OrderByDescending(t => t.Count).ToArray();

		VoiceProviders = artist.ArtistLinksOfType(ArtistLinkType.VoiceProvider, LinkDirection.ManyToOne, allowInheritance: true)
			.Select(g => new ArtistContract(g, languagePreference)).OrderBy(a => a.Name).ToArray();

		Voicebanks = artist.ArtistLinksOfType(ArtistLinkType.VoiceProvider, LinkDirection.OneToMany)
			.Select(g => new ArtistContract(g, languagePreference)).OrderBy(a => a.Name).ToArray();
	}

	[DataMember]
	public AdvancedArtistStatsContract AdvancedStats { get; init; }

	[DataMember]
	public string AllNames { get; init; }

	[DataMember]
	public TagBaseContract ArtistTypeTag { get; init; }

	[DataMember]
	public ArtistContract BaseVoicebank { get; init; }

	public bool CanRemoveTagUsages { get; init; }

	[DataMember]
	public ArtistContract CharacterDesigner { get; init; }

	[DataMember]
	public ArtistContract[] CharacterDesignerOf { get; init; }

	[DataMember]
	public ArtistContract[] ChildVoicebanks { get; init; }

	[DataMember]
	public int CommentCount { get; init; }

	[DataMember]
	public DateTime CreateDate { get; init; }

	[DataMember]
	public EnglishTranslatedString Description { get; init; }

	[DataMember]
	public bool Draft { get; init; }

	/// <summary>
	/// Logged in user has subscribed to email notifications.
	/// </summary>
	[DataMember]
	public bool EmailNotifications { get; set; }

	[DataMember]
	public ArtistContract[] Groups { get; init; }

	/// <summary>
	/// Logged in user is following this artist.
	/// </summary>
	[DataMember]
	public bool IsAdded { get; set; }

	[DataMember]
	public ArtistContract[] Illustrators { get; init; }

	[DataMember]
	public ArtistContract[] IllustratorOf { get; init; }

	[DataMember]
	public CommentForApiContract[] LatestComments { get; set; }

	[DataMember]
	public ArtistContract[] Managers { get; init; }

	[DataMember]
	public ArtistContract[] ManagerOf { get; init; }

	[DataMember]
	public ArtistContract[] Members { get; init; }

	[DataMember]
	public ArtistContract MergedTo { get; set; }

	[DataMember]
	public AlbumForApiContract[] LatestAlbums { get; set; }

	[DataMember]
	public ReleaseEventForApiContract[] LatestEvents { get; set; } = global::System.Array.Empty<global::VocaDb.Model.DataContracts.ReleaseEvents.ReleaseEventForApiContract>();

	[DataMember]
	public SongForApiContract[] LatestSongs { get; set; }

	[DataMember]
	public UserForApiContract[] OwnerUsers { get; init; }

	/// <summary>
	/// Personal stats for the logged in user. 
	/// Can be null if the user isn't logged in.
	/// </summary>
	[DataMember]
	public PersonalArtistStatsContract PersonalStats { get; init; }

	[DataMember]
	public EntryPictureFileContract[] Pictures { get; init; }

	[DataMember]
	public SharedArtistStatsContract SharedStats { get; init; }

	[DataMember]
	public bool SiteNotifications { get; set; }

	[DataMember]
	public TagUsageForApiContract[] Tags { get; init; }

	[DataMember]
	public AlbumForApiContract[] TopAlbums { get; set; }

	[DataMember]
	public SongForApiContract[] TopSongs { get; set; }

	[DataMember]
	public TranslatedStringContract TranslatedName { get; init; }

	[DataMember]
	public ArtistContract[] Voicebanks { get; init; }

	[DataMember]
	public ArtistContract[] VoiceProviders { get; init; }

	[DataMember]
	public WebLinkContract[] WebLinks { get; init; }
}

[DataContract(Namespace = Schemas.VocaDb)]
public class PersonalArtistStatsContract
{
	/// <summary>
	/// Number of times logged user has rated songs by this artist.
	/// </summary>
	[DataMember]
	public int SongRatingCount { get; init; }
}

[DataContract(Namespace = Schemas.VocaDb)]
public class SharedArtistStatsContract
{
	[DataMember]
	public int AlbumCount { get; set; }

	[DataMember]
	public double AlbumRatingAverage { get; init; }

	[DataMember]
	public int EventCount { get; init; }

	[DataMember]
	public int FollowerCount { get; init; }

	[DataMember]
	public int RatedAlbumCount { get; init; }

	[DataMember]
	public int SongCount { get; set; }

	/// <summary>
	/// Total rating score for this artist.
	/// </summary>
	[DataMember]
	public int RatedSongCount { get; init; }
}
