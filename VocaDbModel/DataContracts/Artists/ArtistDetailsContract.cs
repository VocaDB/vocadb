#nullable disable

using System;
using System.Linq;
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

namespace VocaDb.Model.DataContracts.Artists
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistDetailsContract : ArtistContract
	{
		public ArtistDetailsContract() { }

		public ArtistDetailsContract(Artist artist, ContentLanguagePreference languagePreference, IUserPermissionContext userContext,
			IAggregatedEntryImageUrlFactory imageStore, Tag artistTypeTag = null)
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
			LatestAlbums = new AlbumForApiContract[] { };
			LatestSongs = new SongForApiContract[] { };
			OwnerUsers = artist.OwnerUsers.Select(u => new UserContract(u.User)).ToArray();
			Pictures = artist.Pictures.Select(p => new EntryPictureFileContract(p, imageStore)).ToArray();
			TopAlbums = new AlbumForApiContract[] { };
			TopSongs = new SongForApiContract[] { };
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
				ChildVoicebanks = new ArtistContract[0];
			}

			Groups = artist.ArtistLinksOfType(ArtistLinkType.Group, LinkDirection.ManyToOne)
				.Select(g => new ArtistContract(g, languagePreference)).OrderBy(g => g.Name).ToArray();

			Illustrators = artist.ArtistLinksOfType(ArtistLinkType.Illustrator, LinkDirection.ManyToOne, allowInheritance: true)
				.Select(g => new ArtistContract(g, languagePreference)).ToArray();

			IllustratorOf = artist.ArtistLinksOfType(ArtistLinkType.Illustrator, LinkDirection.OneToMany)
				.Select(g => new ArtistContract(g, languagePreference)).OrderBy(a => a.Name).ToArray();

			Manager = artist.ArtistLinksOfType(ArtistLinkType.Manager, LinkDirection.ManyToOne, allowInheritance: true)
				.Select(g => new ArtistContract(g, languagePreference)).FirstOrDefault();

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
		public AdvancedArtistStatsContract AdvancedStats { get; set; }

		[DataMember]
		public string AllNames { get; set; }

		[DataMember]
		public TagBaseContract ArtistTypeTag { get; set; }

		[DataMember]
		public ArtistContract BaseVoicebank { get; set; }

		public bool CanRemoveTagUsages { get; set; }

		[DataMember]
		public ArtistContract CharacterDesigner { get; set; }

		[DataMember]
		public ArtistContract[] CharacterDesignerOf { get; set; }

		[DataMember]
		public ArtistContract[] ChildVoicebanks { get; set; }

		[DataMember]
		public int CommentCount { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public EnglishTranslatedString Description { get; set; }

		[DataMember]
		public bool Draft { get; set; }

		/// <summary>
		/// Logged in user has subscribed to email notifications.
		/// </summary>
		[DataMember]
		public bool EmailNotifications { get; set; }

		[DataMember]
		public ArtistContract[] Groups { get; set; }

		/// <summary>
		/// Logged in user is following this artist.
		/// </summary>
		[DataMember]
		public bool IsAdded { get; set; }

		[DataMember]
		public ArtistContract[] Illustrators { get; set; }

		[DataMember]
		public ArtistContract[] IllustratorOf { get; set; }

		[DataMember]
		public CommentForApiContract[] LatestComments { get; set; }

		[DataMember]
		public ArtistContract Manager { get; set; }

		[DataMember]
		public ArtistContract[] Members { get; set; }

		[DataMember]
		public ArtistContract MergedTo { get; set; }

		[DataMember]
		public AlbumForApiContract[] LatestAlbums { get; set; }

		[DataMember]
		public ReleaseEventForApiContract[] LatestEvents { get; set; } = new ReleaseEventForApiContract[0];

		[DataMember]
		public SongForApiContract[] LatestSongs { get; set; }

		[DataMember]
		public UserContract[] OwnerUsers { get; set; }

		/// <summary>
		/// Personal stats for the logged in user. 
		/// Can be null if the user isn't logged in.
		/// </summary>
		[DataMember]
		public PersonalArtistStatsContract PersonalStats { get; set; }

		[DataMember]
		public EntryPictureFileContract[] Pictures { get; set; }

		[DataMember]
		public SharedArtistStatsContract SharedStats { get; set; }

		[DataMember]
		public bool SiteNotifications { get; set; }

		[DataMember]
		public TagUsageForApiContract[] Tags { get; set; }

		[DataMember]
		public AlbumForApiContract[] TopAlbums { get; set; }

		[DataMember]
		public SongForApiContract[] TopSongs { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArtistContract[] Voicebanks { get; set; }

		[DataMember]
		public ArtistContract[] VoiceProviders { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }
	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PersonalArtistStatsContract
	{
		/// <summary>
		/// Number of times logged user has rated songs by this artist.
		/// </summary>
		[DataMember]
		public int SongRatingCount { get; set; }
	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SharedArtistStatsContract
	{
		[DataMember]
		public int AlbumCount { get; set; }

		[DataMember]
		public double AlbumRatingAverage { get; set; }

		[DataMember]
		public int EventCount { get; set; }

		[DataMember]
		public int FollowerCount { get; set; }

		[DataMember]
		public int RatedAlbumCount { get; set; }

		[DataMember]
		public int SongCount { get; set; }

		/// <summary>
		/// Total rating score for this artist.
		/// </summary>
		[DataMember]
		public int RatedSongCount { get; set; }
	}
}
