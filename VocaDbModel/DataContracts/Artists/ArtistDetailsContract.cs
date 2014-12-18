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
			ChildVoicebanks = artist.CanHaveChildVoicebanks ? artist.ChildVoicebanks.Select(c => new ArtistContract(c, languagePreference)).ToArray() : new ArtistContract[0];
			CreateDate = artist.CreateDate;
			Description = artist.Description;
			Draft = artist.Status == EntryStatus.Draft;
			Groups = artist.Groups.Select(g => new GroupForArtistContract(g, languagePreference)).OrderBy(g => g.Group.Name).ToArray();
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			LatestAlbums = new AlbumContract[] {};
			LatestSongs = new SongContract[] {};
			Members = artist.Members.Select(m => new GroupForArtistContract(m, languagePreference)).OrderBy(a => a.Member.Name).ToArray();
			OwnerUsers = artist.OwnerUsers.Select(u => new UserContract(u.User)).ToArray();
			Pictures = artist.Pictures.Select(p => new EntryPictureFileContract(p)).ToArray();
			Tags = artist.Tags.Usages.Select(u => new TagUsageContract(u)).OrderByDescending(t => t.Count).ToArray();
			TopAlbums = new AlbumContract[] {};
			TopSongs = new SongContract[] {};
			WebLinks = artist.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public string AllNames { get; set; }

		[DataMember]
		public ArtistContract BaseVoicebank { get; set; }

		[DataMember]
		public ArtistContract[] ChildVoicebanks { get; set; }

		[DataMember]
		public int CommentCount { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool Draft { get; set; }

		/// <summary>
		/// Logged in user has subscribed to email notifications.
		/// </summary>
		[DataMember]
		public bool EmailNotifications { get; set; }

		[DataMember]
		public GroupForArtistContract[] Groups { get; set; }

		/// <summary>
		/// Logged in user is following this artist.
		/// </summary>
		[DataMember]
		public bool IsAdded { get; set; }

		[DataMember]
		public CommentContract[] LatestComments { get; set; }

		[DataMember]
		public GroupForArtistContract[] Members { get; set; }

		[DataMember]
		public ArtistContract MergedTo { get; set; }

		[DataMember]
		public AlbumContract[] LatestAlbums { get; set; }

		[DataMember]
		public SongContract[] LatestSongs { get; set; }

		[DataMember]
		public UserContract[] OwnerUsers { get; set; }

		[DataMember]
		public PersonalArtistStatsContract PersonalStats { get; set; }

		[DataMember]
		public EntryPictureFileContract[] Pictures { get; set; }

		[DataMember]
		public SharedArtistStatsContract SharedStats { get; set; }

		[DataMember]
		public bool SiteNotifications { get; set; }

		[DataMember]
		public TagUsageContract[] Tags { get; set; }

		[DataMember]
		public AlbumContract[] TopAlbums { get; set; }

		[DataMember]
		public SongContract[] TopSongs { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

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
