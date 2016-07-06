using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistDetailsContract : ArtistContract {

		public ArtistDetailsContract() {}

		public ArtistDetailsContract(Artist artist, ContentLanguagePreference languagePreference)
			: base(artist, languagePreference) {

			AllNames = string.Join(", ", artist.AllNames.Where(n => n != Name));
			BaseVoicebank = artist.BaseVoicebank != null ? new ArtistContract(artist.BaseVoicebank, languagePreference) : null;
			CharacterDesigns = artist.ArtistLinksOfType(ArtistLinkType.Illustrator, LinkDirection.OneToMany).Select(g => new ArtistContract(g, languagePreference)).OrderBy(a => a.Name).ToArray();
			ChildVoicebanks = artist.CanHaveChildVoicebanks ? artist.ChildVoicebanks.Where(c => !c.Deleted).Select(c => new ArtistContract(c, languagePreference)).ToArray() : new ArtistContract[0];
			CreateDate = artist.CreateDate;
			Description =  artist.Description;
			Draft = artist.Status == EntryStatus.Draft;
			Groups = artist.ArtistLinksOfType(ArtistLinkType.Group, LinkDirection.ManyToOne).Select(g => new ArtistContract(g, languagePreference)).OrderBy(g => g.Name).ToArray();
			Illustrator = artist.ArtistLinksOfType(ArtistLinkType.Illustrator, LinkDirection.ManyToOne).Select(g => new ArtistContract(g, languagePreference)).FirstOrDefault();
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			LatestAlbums = new AlbumContract[] {};
			LatestSongs = new SongForApiContract[] {};
			Members = artist.ArtistLinksOfType(ArtistLinkType.Group, LinkDirection.OneToMany).Select(g => new ArtistContract(g, languagePreference)).OrderBy(g => g.Name).ToArray();
			OwnerUsers = artist.OwnerUsers.Select(u => new UserContract(u.User)).ToArray();
			Pictures = artist.Pictures.Select(p => new EntryPictureFileContract(p)).ToArray();
			Tags = artist.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).OrderByDescending(t => t.Count).ToArray();
			TopAlbums = new AlbumContract[] {};
			TopSongs = new SongForApiContract[] {};
			Voicebanks = artist.ArtistLinksOfType(ArtistLinkType.VoiceProvider, LinkDirection.OneToMany).Select(g => new ArtistContract(g, languagePreference)).OrderBy(a => a.Name).ToArray();
			VoiceProvider = artist.ArtistLinksOfType(ArtistLinkType.VoiceProvider, LinkDirection.ManyToOne).Select(g => new ArtistContract(g, languagePreference)).FirstOrDefault();
			WebLinks = artist.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public AdvancedArtistStatsContract AdvancedStats { get; set; }

		[DataMember]
		public string AllNames { get; set; }

		[DataMember]
		public ArtistContract BaseVoicebank { get; set; }

		[DataMember]
		public ArtistContract[] CharacterDesigns { get; set; }

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
		public ArtistContract Illustrator { get; set; }

		[DataMember]
		public CommentForApiContract[] LatestComments { get; set; }

		[DataMember]
		public ArtistContract[] Members { get; set; }

		[DataMember]
		public ArtistContract MergedTo { get; set; }

		[DataMember]
		public AlbumContract[] LatestAlbums { get; set; }

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
		public AlbumContract[] TopAlbums { get; set; }

		[DataMember]
		public SongForApiContract[] TopSongs { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArtistContract[] Voicebanks { get; set; }

		[DataMember]
		public ArtistContract VoiceProvider { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PersonalArtistStatsContract {
		
		/// <summary>
		/// Number of times logged user has rated songs by this artist.
		/// </summary>
		[DataMember]
		public int SongRatingCount { get; set; }

	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SharedArtistStatsContract {

		[DataMember]
		public int AlbumCount { get; set; }

		[DataMember]
		public double AlbumRatingAverage { get; set; }

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
