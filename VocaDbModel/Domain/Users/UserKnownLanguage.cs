namespace VocaDb.Model.Domain.Users {

	public class UserKnownLanguage : IEntryWithIntId {

		public UserKnownLanguage() { }

		public UserKnownLanguage(User user, string cultureCode, UserLanguageProficiency proficiency) {
			User = user;
			CultureCode = cultureCode;
			Proficiency = proficiency;
		}

		public virtual string CultureCode { get; set; }

		public virtual int Id { get; set; }

		public virtual UserLanguageProficiency Proficiency { get; set; }

		public virtual User User { get; set; }

	}

	public enum UserLanguageProficiency {
		Nothing = 0,
		Basics = 1,
		Intermediate = 2,
		Advanced = 3,
		Native = 4
	}

}
