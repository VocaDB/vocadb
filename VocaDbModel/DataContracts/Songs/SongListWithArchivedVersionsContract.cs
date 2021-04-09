#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	public class SongListWithArchivedVersionsContract : SongListBaseContract
	{
		public SongListWithArchivedVersionsContract(SongList songList, IUserPermissionContext permissionContext, IUserIconFactory userIconFactory)
			: base(songList)
		{
			ArchivedVersions = songList.ArchivedVersionsManager.Versions.Select(
				a => new ArchivedSongListVersionContract(a, userIconFactory)).ToArray();

			Version = songList.Version;
		}

		public ArchivedSongListVersionContract[] ArchivedVersions { get; init; }

		public int Version { get; init; }
	}
}
