#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongInListEditContract : SongInListForApiContract
	{
		public SongInListEditContract() { }

		public SongInListEditContract(SongInList songInList, ContentLanguagePreference languagePreference)
			: base(songInList, languagePreference, SongOptionalFields.AdditionalNames)
		{
			ParamIs.NotNull(() => songInList);

			SongInListId = songInList.Id;
		}

		[DataMember]
		public int SongInListId { get; init; }
	}
}
