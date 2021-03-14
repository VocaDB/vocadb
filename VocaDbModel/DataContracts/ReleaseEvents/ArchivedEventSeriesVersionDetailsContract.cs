#nullable disable

using System.Linq;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ArchivedEventSeriesVersionDetailsContract
	{
#nullable enable
		public ArchivedEventSeriesVersionDetailsContract(ArchivedReleaseEventSeriesVersion archived, ArchivedReleaseEventSeriesVersion? comparedVersion, IUserPermissionContext permissionContext)
		{
			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ArchivedEventSeriesVersionContract(archived);
			ComparedVersion = comparedVersion != null ? new ArchivedEventSeriesVersionContract(comparedVersion) : null;
			ReleaseEventSeries = new ReleaseEventSeriesContract(archived.Entry, permissionContext.LanguagePreference);
			Name = ReleaseEventSeries.Name;

			ComparableVersions = archived.Entry.ArchivedVersionsManager
				.GetPreviousVersions(archived, permissionContext)
				.Select(a => ArchivedObjectVersionWithFieldsContract.Create(a, a.Diff.ChangedFields.Value, a.CommonEditEvent))
				.ToArray();

			Versions = ComparedEventSeriesContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;
		}
#nullable disable

		public ArchivedEventSeriesVersionContract ArchivedVersion { get; init; }

		public ArchivedObjectVersionContract[] ComparableVersions { get; init; }

		public ArchivedObjectVersionContract ComparedVersion { get; init; }

		public int ComparedVersionId { get; init; }

		public ReleaseEventSeriesContract ReleaseEventSeries { get; init; }

		public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);

		public string Name { get; init; }

		public ComparedEventSeriesContract Versions { get; init; }
	}
}
