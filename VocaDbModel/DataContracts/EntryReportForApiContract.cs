using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record EntryReportForApiContract
{
	[DataMember]
	public UserForApiContract? ClosedBy { get; init; }

	[DataMember]
	public DateTime? ClosedAt { get; init; }

	[DataMember]
	public DateTime Created { get; init; }

	[DataMember]
	public EntryForApiContract Entry { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember]
	public string Notes { get; init; }

	[DataMember]
	public string ReportTypeName { get; init; }

	[DataMember]
	public UserForApiContract? User { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract? Version { get; init; }

	[DataMember]
	public string? Hostname { get; init; }

	public EntryReportForApiContract(EntryReport report, EntryForApiContract entry, IUserIconFactory userIconFactory, IUserPermissionContext permissionContext)
	{
		ParamIs.NotNull(() => report);
		ClosedAt = report.ClosedAt;
		ClosedBy = report.ClosedBy != null ? new UserForApiContract(report.ClosedBy, userIconFactory, UserOptionalFields.MainPicture) : null;
		Created = report.Created;
		Entry = entry;
		Id = report.Id;
		Notes = report.Notes;
		ReportTypeName = report.ReportTypeName();
		User = (report.User != null ? new UserForApiContract(report.User, userIconFactory, UserOptionalFields.MainPicture) : null);
		Version = (report.VersionBase != null) ? new ArchivedObjectVersionForApiContract(report.VersionBase, false, "", userIconFactory) : null;
		Hostname = permissionContext.HasPermission(PermissionToken.ManageIPRules) ? report.Hostname : null;
	}
}