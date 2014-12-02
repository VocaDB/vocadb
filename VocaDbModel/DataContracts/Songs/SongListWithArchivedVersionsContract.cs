using System.Linq;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongListWithArchivedVersionsContract : SongListContract {

		public SongListWithArchivedVersionsContract(SongList songList, IUserPermissionContext permissionContext)
			: base(songList, permissionContext) {
			
			ArchivedVersions = songList.ArchivedVersionsManager.Versions.Select(
				a => new ArchivedSongListVersionContract(a)).ToArray();

		}

		public ArchivedSongListVersionContract[] ArchivedVersions { get; set; }

	}

}
