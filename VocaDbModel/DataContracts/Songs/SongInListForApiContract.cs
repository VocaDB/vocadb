using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongInListForApiContract
	{
		public SongInListForApiContract() { }

		public SongInListForApiContract(SongInList songInList, ContentLanguagePreference languagePreference, SongOptionalFields fields)
		{
			this.Notes = songInList.Notes;
			this.Order = songInList.Order;
			this.Song = new SongForApiContract(songInList.Song, null, languagePreference, fields);
		}

		[DataMember]
		public string Notes { get; set; }

		[DataMember]
		public int Order { get; set; }

		[DataMember]
		public SongForApiContract Song { get; set; }
	}
}
