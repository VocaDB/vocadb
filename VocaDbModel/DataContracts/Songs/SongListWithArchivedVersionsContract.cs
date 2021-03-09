#nullable disable

using System.Linq;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	public class SongListWithArchivedVersionsContract : SongListBaseContract
	{
		public SongListWithArchivedVersionsContract(SongList songList, IUserPermissionContext permissionContext)
			: base(songList)
		{
			ArchivedVersions = songList.ArchivedVersionsManager.Versions.Select(
				a => new ArchivedSongListVersionContract(a)).ToArray();

			Version = songList.Version;
		}

		public ArchivedSongListVersionContract[] ArchivedVersions { get; init; }

		public int Version { get; init; }
	}
}
