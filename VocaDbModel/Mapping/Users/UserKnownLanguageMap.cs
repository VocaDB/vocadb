using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users {

	public class UserKnownLanguageMap : ClassMap<UserKnownLanguage> {

		public UserKnownLanguageMap() {

			Id(m => m.Id);

			Map(m => m.Proficiency).Not.Nullable();
			References(m => m.User).Not.Nullable();

			Component(m => m.CultureCode, c => {
				c.Map(m => m.CultureCode).Column("[CultureCode]").Length(20).Not.Nullable();
			});

		}

	}

}
