#nullable disable

using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	public class SongInListContract
	{
#nullable enable
		public SongInListContract(SongInList songInList, ContentLanguagePreference languagePreference)
		{
			ParamIs.NotNull(() => songInList);

			Order = songInList.Order;
			Notes = songInList.Notes;
			Song = new SongContract(songInList.Song, languagePreference);
		}
#nullable disable

		public int Order { get; init; }

		public string Notes { get; init; }

		public SongContract Song { get; init; }
	}
}
