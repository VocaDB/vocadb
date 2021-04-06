#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums
{
	public class ServerOnlyAlbumWithArchivedVersionsContract : AlbumContract
	{
#nullable enable
		public ServerOnlyAlbumWithArchivedVersionsContract(Album album, ContentLanguagePreference languagePreference)
			: base(album, languagePreference)
		{
			ParamIs.NotNull(() => album);

			ArchivedVersions = album.ArchivedVersionsManager.Versions.Select(a => new ServerOnlyArchivedAlbumVersionContract(a)).ToArray();
		}
#nullable disable

		public ServerOnlyArchivedAlbumVersionContract[] ArchivedVersions { get; init; }
	}
}
