using System;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts {

	public class EntryReportContract {

		public EntryReportContract(EntryReport report, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => report);

			Created = report.Created;
			Entry = new EntryRefWithNameContract(report.EntryBase, languagePreference);
			Hostname = report.Hostname;
			Id = report.Id;
			Notes = report.Notes;
			User = (report.User != null ? new UserWithEmailContract(report.User) : null);

		}

		public EntryReportContract(AlbumReport report, ContentLanguagePreference languagePreference)
			: this((EntryReport)report, languagePreference) {

			AlbumReportType = report.ReportType;

		}

		public EntryReportContract(ArtistReport report, ContentLanguagePreference languagePreference)
			: this((EntryReport)report, languagePreference) {

			ArtistReportType = report.ReportType;

		}

		public EntryReportContract(SongReport songReport, ContentLanguagePreference languagePreference)
			: this((EntryReport)songReport, languagePreference) {

			SongReportType = songReport.ReportType;

		}

		public EntryReportContract(UserReport userReport, ContentLanguagePreference languagePreference)
			: this((EntryReport)userReport, languagePreference) {

			UserReportType = userReport.ReportType;

		}

		public AlbumReportType AlbumReportType { get; set; }

		public ArtistReportType ArtistReportType { get; set; }

		public DateTime Created { get; set; }

		public EntryRefWithNameContract Entry { get; set; }

		public string Hostname { get; set; }

		public int Id { get; set; }

		public string Notes { get; set; }

		public SongReportType SongReportType { get; set; }

		public UserWithEmailContract User { get; set; }

		public UserReportType UserReportType { get; set; }

	}

	public class EntryReportContractFactory {

		public EntryReportContract Create(EntryReport report, ContentLanguagePreference languagePreference) {

			if (report is AlbumReport)
				return new EntryReportContract((AlbumReport)report, languagePreference);

			if (report is ArtistReport)
				return new EntryReportContract((ArtistReport)report, languagePreference);

			if (report is SongReport)
				return new EntryReportContract((SongReport)report, languagePreference);

			if (report is UserReport)
				return new EntryReportContract((UserReport)report, languagePreference);

			return new EntryReportContract(report, languagePreference);

		}

	}

}
