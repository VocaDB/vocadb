using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongForEditContract : SongContract {

		public SongForEditContract() {}

		public SongForEditContract(Song song, ContentLanguagePreference languagePreference)
			: base(song, languagePreference) {
			
			ParamIs.NotNull(() => song);

			Artists = song.Artists.Select(a => new ArtistForSongContract(a, languagePreference)).OrderBy(a => a.Name).ToArray();
			DefaultNameLanguage = song.TranslatedName.DefaultLanguage;
			Lyrics = song.Lyrics.Select(l => new LyricsForSongContract(l)).ToArray();
			Names = song.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			Notes = song.Notes;
			OriginalVersion = (song.OriginalVersion != null && !song.OriginalVersion.Deleted ? new SongContract(song.OriginalVersion, languagePreference) : null);
			PVs = song.PVs.Select(p => new PVContract(p)).ToArray();
			Tags = song.Tags.TagNames.ToArray();
			UpdateNotes = string.Empty;
			WebLinks = song.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public ArtistForSongContract[] Artists { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember]
		public LyricsForSongContract[] Lyrics { get; set; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public string Notes { get; set; }

		[DataMember]
		public SongContract OriginalVersion { get; set; }

		[DataMember(Name = "pvs")]
		public PVContract[] PVs { get; set; }

		// Required here for validation
		[DataMember]
		public string[] Tags { get; set; }

		[DataMember]
		public string UpdateNotes { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
