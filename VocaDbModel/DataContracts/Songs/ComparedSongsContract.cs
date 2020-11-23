using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	public class ComparedSongsContract : ComparedVersionsContract<ArchivedSongContract>
	{
		public ComparedSongsContract(ComparedVersionsContract<ArchivedSongContract> comparedVersions)
			: base(comparedVersions) { }

		public static ComparedSongsContract Create(ArchivedSongVersion firstData, ArchivedSongVersion secondData)
		{
			return new ComparedSongsContract(Create(firstData, secondData, ArchivedSongContract.GetAllProperties, d => d.Id));
		}
	}
}
