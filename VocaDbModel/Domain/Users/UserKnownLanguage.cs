#nullable disable

using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Users
{
	public class UserKnownLanguage : IEntryWithIntId
	{
		private OptionalCultureCode _cultureCode;

		public UserKnownLanguage() { }

		public UserKnownLanguage(User user, string cultureCode, UserLanguageProficiency proficiency)
		{
			User = user;
			CultureCode = new OptionalCultureCode(cultureCode);
			Proficiency = proficiency;
		}

		public virtual OptionalCultureCode CultureCode
		{
			get => _cultureCode ?? (_cultureCode = OptionalCultureCode.Empty);
			set => _cultureCode = value ?? OptionalCultureCode.Empty;
		}

		public virtual int Id { get; set; }

		public virtual UserLanguageProficiency Proficiency { get; set; }

		public virtual User User { get; set; }
	}

	public enum UserLanguageProficiency
	{
		Nothing = 0,
		Basics = 1,
		Intermediate = 2,
		Advanced = 3,
		Native = 4
	}
}
