using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Tags;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumDetailsContract : AlbumContract {

		public AlbumDetailsContract() { }

		public AlbumDetailsContract(Album album, ContentLanguagePreference languagePreference)
			: base(album, languagePreference) {

			ArtistLinks = album.Artists.Select(a => new ArtistForAlbumContract(a, languagePreference)).OrderBy(a => a.Name).ToArray();
			Description = album.Description;
			OriginalRelease = (album.OriginalRelease != null ? new AlbumReleaseContract(album.OriginalRelease) : null);
			Pictures = album.Pictures.Select(p => new EntryPictureFileContract(p)).ToArray();
			PVs = album.PVs.Select(p => new PVContract(p)).ToArray();
			Songs = album.Songs
				.OrderBy(s => s.DiscNumber).ThenBy(s => s.TrackNumber)
				.Select(s => new SongInAlbumContract(s, languagePreference, false)).ToArray();
			Tags = album.Tags.Usages.Select(u => new TagUsageContract(u)).OrderByDescending(t => t.Count).ToArray();
			WebLinks = album.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public AlbumForUserContract AlbumForUser { get; set; }

		[DataMember]
		public ArtistForAlbumContract[] ArtistLinks { get; set; }

		[DataMember]
		public int CommentCount { get; set; }

		[DataMember]
		public EnglishTranslatedString Description { get; set; }

		[DataMember]
		public int Hits { get; set; }

		[DataMember]
		public CommentContract[] LatestComments { get; set; }

		[DataMember]
		public AlbumContract MergedTo { get; set; }

		[DataMember]
		public AlbumReleaseContract OriginalRelease { get; set; }

		[DataMember]
		public int OwnedCount { get; set; }

		[DataMember]
		public EntryPictureFileContract[] Pictures { get; set; }

		[DataMember]
		public PVContract[] PVs { get; set; }

		[DataMember]
		public SongInAlbumContract[] Songs { get; set; }

		[DataMember]
		public TagUsageContract[] Tags { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

		[DataMember]
		public int WishlistCount { get; set; }

	}

}
