#nullable disable

using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums
{
	public class AlbumWithArchivedVersionsContract : AlbumContract
	{
#nullable enable
		public AlbumWithArchivedVersionsContract(Album album, ContentLanguagePreference languagePreference)
			: base(album, languagePreference)
		{
			ParamIs.NotNull(() => album);

			ArchivedVersions = album.ArchivedVersionsManager.Versions.Select(a => new ArchivedAlbumVersionContract(a)).ToArray();
		}
#nullable disable

		public ArchivedAlbumVersionContract[] ArchivedVersions { get; init; }
	}
}
