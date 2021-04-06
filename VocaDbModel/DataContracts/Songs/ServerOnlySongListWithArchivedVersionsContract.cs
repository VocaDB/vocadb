#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	public class ServerOnlySongListWithArchivedVersionsContract : SongListBaseContract
	{
		public ServerOnlySongListWithArchivedVersionsContract(SongList songList, IUserPermissionContext permissionContext)
			: base(songList)
		{
			ArchivedVersions = songList.ArchivedVersionsManager.Versions.Select(
				a => new ServerOnlyArchivedSongListVersionContract(a)).ToArray();

			Version = songList.Version;
		}

		public ServerOnlyArchivedSongListVersionContract[] ArchivedVersions { get; init; }

		public int Version { get; init; }
	}
}
