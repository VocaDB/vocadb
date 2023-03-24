#nullable disable

using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Albums;

[Obsolete]
public class AlbumWithArchivedVersionsContract : AlbumContract
{
#nullable enable
	public AlbumWithArchivedVersionsContract(
		Album album,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		IUserIconFactory userIconFactory
	)
		: base(album, languagePreference, permissionContext)
	{
		ParamIs.NotNull(() => album);

		ArchivedVersions = album.ArchivedVersionsManager.Versions.Select(a => new ArchivedAlbumVersionContract(a, userIconFactory)).ToArray();
	}
#nullable disable

	public ArchivedAlbumVersionContract[] ArchivedVersions { get; init; }
}
