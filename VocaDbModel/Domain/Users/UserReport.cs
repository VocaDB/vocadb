namespace VocaDb.Model.Domain.Users {

	public class UserReport : GenericEntryReport<User, UserReportType> {

		public UserReport() { }

		public UserReport(User reportedUser, UserReportType reportType, User user, string hostname, string notes)
			: base(reportedUser, reportType, user, hostname, notes, null) {}

	}

	public enum UserReportType {

		/// <summary>
		/// Found a match on StopForumSpam, identifying the user as malicious.
		/// </summary>
		MaliciousIP = 1,

		Spamming	= 2,

		Other		= 4

	}
}
