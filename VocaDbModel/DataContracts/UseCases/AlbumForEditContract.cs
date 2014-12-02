using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.UseCases {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumForEditContract : AlbumContract {

		public AlbumForEditContract() {}

		public AlbumForEditContract(Album album, ContentLanguagePreference languagePreference)
			: base(album, languagePreference) {

			ArtistLinks = album.Artists.Select(a => new ArtistForAlbumContract(a, languagePreference)).OrderBy(a => a.Name).ToArray();
			DefaultNameLanguage = album.TranslatedName.DefaultLanguage;
			Description = album.Description;
			Identifiers = album.Identifiers.Select(i => i.Value).ToArray();
			Names = album.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			OriginalRelease = (album.OriginalRelease != null ? new AlbumReleaseContract(album.OriginalRelease) : null);
			Pictures = album.Pictures.Select(p => new EntryPictureFileContract(p)).ToArray();
			PVs = album.PVs.Select(p => new PVContract(p)).ToArray();
			Songs = album.Songs
				.OrderBy(s => s.DiscNumber).ThenBy(s => s.TrackNumber)
				.Select(s => new SongInAlbumEditContract(s, languagePreference)).ToArray();
			UpdateNotes = string.Empty;
			WebLinks = album.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public ArtistForAlbumContract[] ArtistLinks { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string[] Identifiers { get; set; }

		[DataMember]
		public AlbumReleaseContract OriginalRelease { get; set; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public EntryPictureFileContract[] Pictures { get; set; }

		[DataMember(Name = "pvs")]
		public PVContract[] PVs { get; set; }

		[DataMember]
		public SongInAlbumEditContract[] Songs { get; set; }

		[DataMember]
		public string UpdateNotes { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
