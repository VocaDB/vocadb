#nullable disable

using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums
{
	public class ComparedAlbumsContract : ComparedVersionsContract<ArchivedAlbumContract>
	{
		public ComparedAlbumsContract(ComparedVersionsContract<ArchivedAlbumContract> comparedVersions)
			: base(comparedVersions) { }

		public static ComparedAlbumsContract Create(ArchivedAlbumVersion firstData, ArchivedAlbumVersion secondData)
		{
			return new ComparedAlbumsContract(Create(firstData, secondData, ArchivedAlbumContract.GetAllProperties, d => d.Id));
		}
	}
}
