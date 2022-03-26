using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongInListForApiContract
	{
#nullable disable
		public SongInListForApiContract() { }
#nullable enable

		public SongInListForApiContract(SongInList songInList, ContentLanguagePreference languagePreference, SongOptionalFields fields)
		{
			Notes = songInList.Notes;
			Order = songInList.Order;
			Song = new SongForApiContract(songInList.Song, null, languagePreference, fields);
		}

		[DataMember]
		public string Notes { get; init; }

		[DataMember]
		public int Order { get; init; }

		[DataMember]
		public SongForApiContract Song { get; init; }
	}
}
