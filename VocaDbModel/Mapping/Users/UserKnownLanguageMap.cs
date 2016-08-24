using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users {

	public class UserKnownLanguageMap : ClassMap<UserKnownLanguage> {

		public UserKnownLanguageMap() {

			Id(m => m.Id);

			Map(m => m.CultureCode).Not.Nullable();
			Map(m => m.Proficiency).Not.Nullable();
			References(m => m.User).Not.Nullable();

		}

	}

}
