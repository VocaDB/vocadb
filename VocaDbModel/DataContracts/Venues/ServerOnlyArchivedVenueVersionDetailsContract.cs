#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues
{
	public class ServerOnlyArchivedVenueVersionDetailsContract
	{
#nullable enable
		public ServerOnlyArchivedVenueVersionDetailsContract(ArchivedVenueVersion archived, ArchivedVenueVersion? comparedVersion, IUserPermissionContext permissionContext, IUserIconFactory userIconFactory)
		{
			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ServerOnlyArchivedVenueVersionContract(archived, userIconFactory);
			ComparedVersion = comparedVersion != null ? new ServerOnlyArchivedVenueVersionContract(comparedVersion, userIconFactory) : null;
			Venue = new VenueContract(archived.Entry, permissionContext.LanguagePreference);
			Name = Venue.Name;

			ComparableVersions = archived.Entry.ArchivedVersionsManager
				.GetPreviousVersions(archived, permissionContext)
				.Select(a => ServerOnlyArchivedObjectVersionWithFieldsContract.Create(a, userIconFactory, a.Diff.ChangedFields.Value, a.CommonEditEvent))
				.ToArray();

			Versions = ComparedVenueContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;
		}
#nullable disable

		public ServerOnlyArchivedVenueVersionContract ArchivedVersion { get; init; }

		public ServerOnlyArchivedObjectVersionContract[] ComparableVersions { get; init; }

		public ServerOnlyArchivedObjectVersionContract ComparedVersion { get; init; }

		public int ComparedVersionId { get; init; }

		public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);

		public string Name { get; init; }

		public VenueContract Venue { get; init; }

		public ComparedVenueContract Versions { get; init; }
	}
}
