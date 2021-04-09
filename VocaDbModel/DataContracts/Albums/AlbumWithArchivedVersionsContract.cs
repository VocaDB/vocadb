#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums
{
	public class AlbumWithArchivedVersionsContract : AlbumContract
	{
#nullable enable
		public AlbumWithArchivedVersionsContract(Album album, ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory)
			: base(album, languagePreference)
		{
			ParamIs.NotNull(() => album);

			ArchivedVersions = album.ArchivedVersionsManager.Versions.Select(a => new ArchivedAlbumVersionContract(a, userIconFactory)).ToArray();
		}
#nullable disable

		public ArchivedAlbumVersionContract[] ArchivedVersions { get; init; }
	}
}
