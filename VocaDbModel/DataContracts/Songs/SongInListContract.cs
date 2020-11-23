using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	public class SongInListContract
	{
		public SongInListContract(SongInList songInList, ContentLanguagePreference languagePreference)
		{
			ParamIs.NotNull(() => songInList);

			Order = songInList.Order;
			Notes = songInList.Notes;
			Song = new SongContract(songInList.Song, languagePreference);
		}

		public int Order { get; set; }

		public string Notes { get; set; }

		public SongContract Song { get; set; }
	}
}
