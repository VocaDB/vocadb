#nullable disable

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.UseCases
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumForEditContract : AlbumContract
	{
		public AlbumForEditContract() { }

		public AlbumForEditContract(Album album, ContentLanguagePreference languagePreference, IAggregatedEntryImageUrlFactory imageStore)
			: base(album, languagePreference)
		{
			ArtistLinks = album.Artists.Select(a => new ArtistForAlbumContract(a, languagePreference)).OrderBy(a => a.Name).ToArray();
			DefaultNameLanguage = album.TranslatedName.DefaultLanguage;
			Description = new EnglishTranslatedStringContract(album.Description);
			Discs = album.Discs.Select(d => new AlbumDiscPropertiesContract(d)).ToArray();
			Identifiers = album.Identifiers.Select(i => i.Value).ToArray();
			Names = album.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			OriginalRelease = (album.OriginalRelease != null ? new AlbumReleaseContract(album.OriginalRelease, languagePreference) : null);
			Pictures = album.Pictures.Select(p => new EntryPictureFileContract(p, imageStore)).ToList();
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
		public ContentLanguageSelection DefaultNameLanguage { get; init; }

		[DataMember]
		public EnglishTranslatedStringContract Description { get; init; }

		[DataMember]
		public AlbumDiscPropertiesContract[] Discs { get; set; }

		[DataMember]
		public string[] Identifiers { get; init; }

		[DataMember]
		public AlbumReleaseContract OriginalRelease { get; init; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; init; }

		[DataMember]
		public IList<EntryPictureFileContract> Pictures { get; init; }

		[DataMember(Name = "pvs")]
		public PVContract[] PVs { get; init; }

		[DataMember]
		public SongInAlbumEditContract[] Songs { get; set; }

		[DataMember]
		public string UpdateNotes { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; init; }
	}
}
