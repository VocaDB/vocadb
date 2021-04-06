#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Artists
{
	public class ServerOnlyArchivedArtistVersionDetailsContract
	{
		public ServerOnlyArchivedArtistVersionDetailsContract() { }

		public ServerOnlyArchivedArtistVersionDetailsContract(ArchivedArtistVersion archived, ArchivedArtistVersion comparedVersion,
			IUserPermissionContext permissionContext)
		{
			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ServerOnlyArchivedArtistVersionContract(archived);
			Artist = new ArtistContract(archived.Artist, permissionContext.LanguagePreference);
			ComparedVersion = comparedVersion != null ? new ServerOnlyArchivedArtistVersionContract(comparedVersion) : null;
			ComparedVersionId = comparedVersion != null ? comparedVersion.Id : 0;
			Name = Artist.Name;

			ComparableVersions = archived.Artist.ArchivedVersionsManager
				.GetPreviousVersions(archived, permissionContext)
				.Select(a => ServerOnlyArchivedObjectVersionWithFieldsContract.Create(a, a.Diff.ChangedFields.Value, a.Reason))
				.ToArray();

			Versions = ComparedArtistsContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;
		}

		public ServerOnlyArchivedArtistVersionContract ArchivedVersion { get; init; }

		public ArtistContract Artist { get; init; }

		public bool CanBeReverted => ArchivedVersion.Version < Artist.Version - 1;

		public ServerOnlyArchivedObjectVersionWithFieldsContract<ArtistEditableFields, ArtistArchiveReason>[] ComparableVersions { get; init; }

		public ServerOnlyArchivedArtistVersionContract ComparedVersion { get; init; }

		public int ComparedVersionId { get; init; }

		public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);

		public string Name { get; init; }

		public ComparedArtistsContract Versions { get; init; }
	}
}
