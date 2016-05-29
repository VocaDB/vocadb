using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users {

	public class OldUsernameMap : ClassMap<OldUsername> {

		public OldUsernameMap() {

			Id(m => m.Id);

			Map(m => m.Date).Not.Nullable();
			Map(m => m.OldName).Not.Nullable();
			References(m => m.User).Not.Nullable();

		}

	}

}
