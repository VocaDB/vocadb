#nullable disable

using System;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.DataContracts
{
	public class ServerOnlyEntryReportContract
	{
		public ServerOnlyEntryReportContract(EntryReport report, EntryForApiContract entry,
			IEnumTranslations enumTranslations, IUserIconFactory userIconFactory)
		{
			ParamIs.NotNull(() => report);

			ClosedAt = report.ClosedAt;
			ClosedBy = report.ClosedBy != null ? new UserForApiContract(report.ClosedBy, userIconFactory, UserOptionalFields.MainPicture) : null;
			Created = report.Created;
			Entry = entry;
			Hostname = report.Hostname;
			Id = report.Id;
			Notes = report.Notes;
			ReportTypeName = enumTranslations != null ? report.TranslatedReportTypeName(enumTranslations) : null;
			User = (report.User != null ? new UserForApiContract(report.User, userIconFactory, UserOptionalFields.MainPicture) : null);
			Version = (report.VersionBase != null ? new ServerOnlyArchivedObjectVersionContract(report.VersionBase) : null);
		}

		public UserForApiContract ClosedBy { get; init; }

		public DateTime? ClosedAt { get; init; }

		public DateTime Created { get; init; }

		public EntryForApiContract Entry { get; init; }

		public string Hostname { get; init; }

		public int Id { get; init; }

		public string Notes { get; init; }

		public string ReportTypeName { get; init; }

		public UserForApiContract User { get; init; }

		public ServerOnlyArchivedObjectVersionContract Version { get; init; }
	}
}
