#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	public class ServerOnlyArchivedSongVersionDetailsContract
	{
		public ServerOnlyArchivedSongVersionDetailsContract() { }

#nullable enable
		public ServerOnlyArchivedSongVersionDetailsContract(ArchivedSongVersion archived, ArchivedSongVersion? comparedVersion, IUserPermissionContext permissionContext, IUserIconFactory userIconFactory)
		{
			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ServerOnlyArchivedSongVersionContract(archived, userIconFactory);
			ComparedVersion = comparedVersion != null ? new ServerOnlyArchivedSongVersionContract(comparedVersion, userIconFactory) : null;
			ComparedVersionId = comparedVersion != null ? comparedVersion.Id : 0;
			Song = new SongContract(archived.Song, permissionContext.LanguagePreference);
			Name = Song.Name;

			ComparableVersions = archived.Song.ArchivedVersionsManager
				.GetPreviousVersions(archived, permissionContext)
				.Select(a => ServerOnlyArchivedObjectVersionWithFieldsContract.Create(a, userIconFactory, a.Diff.ChangedFields.Value, a.Reason))
				.ToArray();

			Versions = ComparedSongsContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;
		}
#nullable disable

		public ServerOnlyArchivedSongVersionContract ArchivedVersion { get; init; }

		public bool CanBeReverted => ArchivedVersion.Version < Song.Version - 1;

		public ServerOnlyArchivedObjectVersionWithFieldsContract<SongEditableFields, SongArchiveReason>[] ComparableVersions { get; init; }

		public ServerOnlyArchivedSongVersionContract ComparedVersion { get; init; }

		public int ComparedVersionId { get; init; }

		public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);

		public string Name { get; init; }

		public SongContract Song { get; init; }

		public ComparedSongsContract Versions { get; init; }
	}
}
