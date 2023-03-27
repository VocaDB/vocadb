using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedVenueVersionDetailsForApiContract
{
	[DataMember]
	public VenueForApiContract Venue { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract ArchivedVersion { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract[] ComparableVersions { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract? ComparedVersion { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public ComparedVersionsForApiContract<ArchivedVenueContract> Versions { get; init; }

	public ArchivedVenueVersionDetailsForApiContract(
		ArchivedVenueVersion archived,
		ArchivedVenueVersion? comparedVersion,
		IUserPermissionContext permissionContext,
		IUserIconFactory userIconFactory
	)
	{
		Venue = new VenueForApiContract(
			venue: archived.Entry,
			languagePreference: permissionContext.LanguagePreference,
			fields: VenueOptionalFields.None
		);
		ArchivedVersion = ArchivedObjectVersionForApiContract.FromVenue(archived, userIconFactory);
		ComparedVersion = comparedVersion != null
			? ArchivedObjectVersionForApiContract.FromVenue(comparedVersion, userIconFactory)
			: null;
		Name = Venue.Name;

		ComparableVersions = archived.Entry.ArchivedVersionsManager
			.GetPreviousVersions(archived, permissionContext)
			.Select(a => ArchivedObjectVersionForApiContract.FromVenue(a, userIconFactory))
			.ToArray();

		Versions = ComparedVersionsForApiContract.FromVenue(archived, comparedVersion);
	}

	public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);
}
