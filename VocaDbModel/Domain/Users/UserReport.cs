namespace VocaDb.Model.Domain.Users {

	public class UserReport : EntryReport {

		private User user;

		public UserReport() { }

		public UserReport(User reportedUser, UserReportType reportType, User user, string hostname, string notes)
			: base(user, hostname, notes) {

			ReportedUser = reportedUser;
			ReportType = reportType;

		}

		public override IEntryWithNames EntryBase {
			get { return ReportedUser; }
		}

		public virtual UserReportType ReportType { get; set; }

		public virtual User ReportedUser {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value;
			}
		}

		public override string ToString() {
			return string.Format("Entry report '{0}' for {1} [{2}]", ReportType, EntryBase, Id);
		}

	}

	public enum UserReportType {

		/// <summary>
		/// Found a match on StopForumSpam, identifying the user as malicious.
		/// </summary>
		MaliciousIP = 1,

		Other = 2

	}
}
