#nullable disable

using System.Collections.Generic;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Tags
{
	public class TagReport : GenericEntryReport<Tag, TagReportType>
	{
		public static readonly HashSet<TagReportType> ReportTypesWithRequiredNotes = new() { TagReportType.InvalidInfo, TagReportType.Other };

		public TagReport() { }

		public TagReport(Tag tag, TagReportType reportType, User user, string hostname, string notes, int? versionNumber)
			: base(tag, reportType, user, hostname, notes, versionNumber) { }

		public virtual ArchivedTagVersion Version => VersionNumber.HasValue ? Entry.ArchivedVersionsManager.GetVersion(VersionNumber.Value) : null;

		public override ArchivedObjectVersion VersionBase => Version;
	}

	public enum TagReportType
	{
		InvalidInfo = 1,

		Duplicate = 2,

		Inappropriate = 3,

		Other = 4
	}
}
