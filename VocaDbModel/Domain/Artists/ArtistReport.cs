using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Artists;

public enum ArtistReportType
{
	InvalidInfo = 1,
	Duplicate = 2,
	Inappropriate = 3,
	OwnershipClaim = 4,
	Other = 5,
	InvalidTag = 6,
}

public class ArtistReport : GenericEntryReport<Artist, ArtistReportType>
{
	public static readonly HashSet<ArtistReportType> ReportTypesWithRequiredNotes = new()
	{
		ArtistReportType.InvalidInfo,
		ArtistReportType.Other,
		ArtistReportType.InvalidTag,
	};

	public ArtistReport() { }

	public ArtistReport(Artist artist, ArtistReportType reportType, User user, string hostname, string notes, int? versionNumber)
		: base(artist, reportType, user, hostname, notes, versionNumber) { }

	public virtual ArchivedArtistVersion? Version =>
		VersionNumber.HasValue
			? Entry.ArchivedVersionsManager.GetVersion(VersionNumber.Value)
			: null;

	public override ArchivedObjectVersion? VersionBase => Version;
}
