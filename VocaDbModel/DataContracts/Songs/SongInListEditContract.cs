using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongInListEditContract {

		private string notes;

		public SongInListEditContract() { }

		public SongInListEditContract(SongInList songInList, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => songInList);

			SongInListId = songInList.Id;
			Order = songInList.Order;
			Notes = songInList.Notes;
			SongName = songInList.Song.TranslatedName[languagePreference];
			SongAdditionalNames = string.Join(", ", songInList.Song.AllNames.Where(n => n != SongName));
			SongArtistString = songInList.Song.ArtistString[languagePreference];
			SongId = songInList.Song.Id;

		}

		public SongInListEditContract(SongContract songContract) {

			ParamIs.NotNull(() => songContract);

			SongId = songContract.Id;
			SongName = songContract.Name;
			SongAdditionalNames = songContract.AdditionalNames;
			SongArtistString = songContract.ArtistString;

			Notes = string.Empty;

		}

		[DataMember]
		public int Order { get; set; }

		[DataMember]
		public string Notes {
			get { return notes; }
			set { notes = value ?? string.Empty; }
		}

		[DataMember]
		public string SongAdditionalNames { get; set; }

		[DataMember]
		public string SongArtistString { get; set; }

		[DataMember]
		public int SongId { get; set; }

		[DataMember]
		public int SongInListId { get; set; }

		[DataMember]
		public string SongName { get; set; }

	}
}
