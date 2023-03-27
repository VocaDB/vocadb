using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.ReleaseEvents;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedEventSeriesVersionDetailsForApiContract
{
	[DataMember]
	public ReleaseEventSeriesForApiContract ReleaseEventSeries { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract ArchivedVersion { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract[] ComparableVersions { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract? ComparedVersion { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public ComparedVersionsForApiContract<ArchivedEventSeriesForApiContract> Versions { get; init; }

	public ArchivedEventSeriesVersionDetailsForApiContract(
		ArchivedReleaseEventSeriesVersion archived,
		ArchivedReleaseEventSeriesVersion? comparedVersion,
		IUserPermissionContext permissionContext,
		IUserIconFactory userIconFactory
	)
	{
		ReleaseEventSeries = new ReleaseEventSeriesForApiContract(
			series: archived.Entry,
			languagePreference: permissionContext.LanguagePreference,
			permissionContext,
			fields: ReleaseEventSeriesOptionalFields.None,
			thumbPersister: null
		);
		ArchivedVersion = ArchivedObjectVersionForApiContract.FromReleaseEventSeries(archived, userIconFactory);
		ComparedVersion = comparedVersion != null
			? ArchivedObjectVersionForApiContract.FromReleaseEventSeries(comparedVersion, userIconFactory)
			: null;
		Name = ReleaseEventSeries.Name;

		ComparableVersions = archived.Entry.ArchivedVersionsManager
			.GetPreviousVersions(archived, permissionContext)
			.Select(a => ArchivedObjectVersionForApiContract.FromReleaseEventSeries(a, userIconFactory))
			.ToArray();

		Versions = ComparedVersionsForApiContract.FromReleaseEventSeries(archived, comparedVersion, permissionContext);
	}

	public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);
}
