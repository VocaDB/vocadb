#nullable disable

using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists
{
	public class ComparedArtistsContract : ComparedVersionsContract<ArchivedArtistContract>
	{
		public ComparedArtistsContract(ComparedVersionsContract<ArchivedArtistContract> comparedVersions)
			: base(comparedVersions) { }

		public static ComparedArtistsContract Create(ArchivedArtistVersion firstData, ArchivedArtistVersion secondData)
		{
			return new ComparedArtistsContract(Create(firstData, secondData, ArchivedArtistContract.GetAllProperties, d => d.Id));
		}
	}
}
