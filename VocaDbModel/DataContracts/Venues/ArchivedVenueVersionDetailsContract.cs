#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues
{
	public class ArchivedVenueVersionDetailsContract
	{
#nullable enable
		public ArchivedVenueVersionDetailsContract(ArchivedVenueVersion archived, ArchivedVenueVersion? comparedVersion, IUserPermissionContext permissionContext, IUserIconFactory userIconFactory)
		{
			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ArchivedVenueVersionContract(archived, userIconFactory);
			ComparedVersion = comparedVersion != null ? new ArchivedVenueVersionContract(comparedVersion, userIconFactory) : null;
			Venue = new VenueContract(archived.Entry, permissionContext.LanguagePreference);
			Name = Venue.Name;

			ComparableVersions = archived.Entry.ArchivedVersionsManager
				.GetPreviousVersions(archived, permissionContext)
				.Select(a => ArchivedObjectVersionWithFieldsContract.Create(a, userIconFactory, a.Diff.ChangedFields.Value, a.CommonEditEvent))
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
