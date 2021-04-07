#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	public class ServerOnlySongWithArchivedVersionsContract : SongContract
	{
#nullable enable
		public ServerOnlySongWithArchivedVersionsContract(Song song, ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory)
			: base(song, languagePreference)
		{
			ParamIs.NotNull(() => song);

			ArchivedVersions = song.ArchivedVersionsManager.Versions.Select(a => new ServerOnlyArchivedSongVersionContract(a, userIconFactory)).OrderByDescending(v => v.Version).ToArray();
			//Author = (ArchivedVersions.Any() && ArchivedVersions.Last().Author != null ? ArchivedVersions.Last().Author : null);
		}
#nullable disable

		public ServerOnlyArchivedSongVersionContract[] ArchivedVersions { get; init; }

		//public UserForApiContract Author { get; init; }
	}
}
