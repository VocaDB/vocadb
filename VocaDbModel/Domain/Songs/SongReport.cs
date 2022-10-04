using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Songs;

public enum SongReportType
{
	BrokenPV = 1,
	InvalidInfo = 2,
	Duplicate = 3,
	Inappropriate = 4,
	Other = 5,
}

public class SongReport : GenericEntryReport<Song, SongReportType>
{
	public static readonly HashSet<SongReportType> ReportTypesWithRequiredNotes = new()
	{
		SongReportType.BrokenPV,
		SongReportType.InvalidInfo,
		SongReportType.Other,
	};

	public SongReport() { }

	public SongReport(Song song, SongReportType reportType, User user, string hostname, string notes, int? versionNumber)
		: base(song, reportType, user, hostname, notes, versionNumber) { }

	public virtual ArchivedSongVersion? Version =>
		VersionNumber.HasValue
			? Entry.ArchivedVersionsManager.GetVersion(VersionNumber.Value)
			: null;

	public override ArchivedObjectVersion? VersionBase => Version;
}
