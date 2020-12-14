#nullable disable

using System.Collections.Generic;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Venues
{
	public class VenueReport : GenericEntryReport<Venue, VenueReportType>
	{
		public static readonly HashSet<VenueReportType> ReportTypesWithRequiredNotes = new HashSet<VenueReportType> { VenueReportType.InvalidInfo, VenueReportType.Other };

		public VenueReport() { }

		public VenueReport(Venue venue, VenueReportType reportType, User user, string hostname, string notes, int? versionNumber) : base(venue, reportType, user, hostname, notes, versionNumber) { }

		public virtual ArchivedVenueVersion Version => VersionNumber.HasValue ? Entry.ArchivedVersionsManager.GetVersion(VersionNumber.Value) : null;

		public override ArchivedObjectVersion VersionBase => Version;
	}

	public enum VenueReportType
	{
		InvalidInfo = 1,

		Duplicate = 2,

		Inappropriate = 3,

		Other = 4
	}
}
