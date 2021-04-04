#nullable disable

using System.Linq;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues
{
	public class ArchivedVenueVersionDetailsContract
	{
#nullable enable
		public ArchivedVenueVersionDetailsContract(ArchivedVenueVersion archived, ArchivedVenueVersion? comparedVersion, IUserPermissionContext permissionContext)
		{
			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ArchivedVenueVersionContract(archived);
			ComparedVersion = comparedVersion != null ? new ArchivedVenueVersionContract(comparedVersion) : null;
			Venue = new VenueContract(archived.Entry, permissionContext.LanguagePreference);
			Name = Venue.Name;

			ComparableVersions = archived.Entry.ArchivedVersionsManager
				.GetPreviousVersions(archived, permissionContext)
				.Select(a => ArchivedObjectVersionWithFieldsContract.Create(a, a.Diff.ChangedFields.Value, a.CommonEditEvent))
				.ToArray();

			Versions = ComparedVenueContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;
		}
#nullable disable

		public ArchivedVenueVersionContract ArchivedVersion { get; init; }

		public ArchivedObjectVersionContract[] ComparableVersions { get; init; }

		public ArchivedObjectVersionContract ComparedVersion { get; init; }

		public int ComparedVersionId { get; init; }

		public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);

		public string Name { get; init; }

		public VenueContract Venue { get; init; }

		public ComparedVenueContract Versions { get; init; }
	}
}
