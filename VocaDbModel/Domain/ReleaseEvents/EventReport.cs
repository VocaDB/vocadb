using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.ReleaseEvents;

public enum EventReportType
{
	InvalidInfo = 1,
	Duplicate = 2,
	Inappropriate = 3,
	Other = 4,
	InvalidTag = 5,
}

public class EventReport : GenericEntryReport<ReleaseEvent, EventReportType>
{
	public static readonly HashSet<EventReportType> ReportTypesWithRequiredNotes = new()
	{
		EventReportType.InvalidInfo,
		EventReportType.Other,
		EventReportType.InvalidTag,
	};

	public EventReport() { }

	public EventReport(ReleaseEvent tag, EventReportType reportType, User user, string hostname, string notes, int? versionNumber)
		: base(tag, reportType, user, hostname, notes, versionNumber) { }

	public virtual ArchivedReleaseEventVersion? Version =>
		VersionNumber.HasValue
			? Entry.ArchivedVersionsManager.GetVersion(VersionNumber.Value)
			: null;

	public override ArchivedObjectVersion? VersionBase => Version;
}
