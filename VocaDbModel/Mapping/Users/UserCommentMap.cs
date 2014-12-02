using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users {

	public class UserCommentMap : ClassMap<UserComment> {

		public UserCommentMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Message).Length(800).Not.Nullable();

			References(m => m.Author).Not.Nullable();
			References(m => m.User).Not.Nullable();

		}

	}
}
