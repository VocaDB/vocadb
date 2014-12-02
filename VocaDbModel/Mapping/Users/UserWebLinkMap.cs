using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users {

	public class UserWebLinkMap : ClassMap<UserWebLink> {

		public UserWebLinkMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Description).Not.Nullable();
			Map(m => m.Url).Not.Nullable();

			References(m => m.User).Not.Nullable();

		}

	}
}
