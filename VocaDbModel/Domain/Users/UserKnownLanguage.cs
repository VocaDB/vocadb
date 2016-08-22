namespace VocaDb.Model.Domain.Users {

	public class UserKnownLanguage {

		public string CultureCode { get; set; }

		public UserLanguageProficiency Proficiency { get; set; }

		public User User { get; set; }

	}

	public enum UserLanguageProficiency {
		Nothing = 0,
		Basics = 1,
		Intermediate = 2,
		Advanced = 3,
		Native = 4
	}

}
