using System;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.DataContracts {

	public class EntryReportContract {

		public EntryReportContract(EntryReport report, EntryForApiContract entry,
			IEnumTranslations enumTranslations) {

			ParamIs.NotNull(() => report);

			Created = report.Created;
			Entry = entry;
			Hostname = report.Hostname;
			Id = report.Id;
			Notes = report.Notes;
			ReportTypeName = enumTranslations != null ? report.TranslatedReportTypeName(enumTranslations) : null;
			User = (report.User != null ? new UserWithEmailContract(report.User) : null);
			Version = (report.VersionBase != null ? new ArchivedObjectVersionContract(report.VersionBase) : null);

		}

		public DateTime Created { get; set; }

		public EntryForApiContract Entry { get; set; }

		public string Hostname { get; set; }

		public int Id { get; set; }

		public string Notes { get; set; }

		public string ReportTypeName { get; set; }

		public UserWithEmailContract User { get; set; }

		public ArchivedObjectVersionContract Version { get; set; }

	}

}
