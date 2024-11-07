using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Albums;

public enum AlbumReportType
{
	InvalidInfo = 1,
	Duplicate = 2,
	Inappropriate = 3,
	Other = 4,
	InvalidTag = 5,
}

public class AlbumReport : GenericEntryReport<Album, AlbumReportType>
{
	public static readonly HashSet<AlbumReportType> ReportTypesWithRequiredNotes = new()
	{
		AlbumReportType.InvalidInfo,
		AlbumReportType.Other,
		AlbumReportType.InvalidTag,
	};

	public AlbumReport() { }

	public AlbumReport(Album album, AlbumReportType reportType, User user, string hostname, string notes, int? versionNumber)
		: base(album, reportType, user, hostname, notes, versionNumber) { }

	public virtual ArchivedAlbumVersion? Version =>
		VersionNumber.HasValue
			? Entry.ArchivedVersionsManager.GetVersion(VersionNumber.Value)
			: null;

	public override ArchivedObjectVersion? VersionBase => Version;
}
