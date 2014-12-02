using VocaDb.Model.Domain.Users;
using FluentNHibernate.Mapping;

namespace VocaDb.Model.Mapping.Users {

	public class UserMessageMap : ClassMap<UserMessage> {

		public UserMessageMap() {

			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Subject).Length(200).Not.Nullable();
			Map(m => m.Message).Length(10000).Not.Nullable();
			Map(m => m.Read).Not.Nullable();

			References(m => m.Sender).Nullable();
			References(m => m.Receiver).Not.Nullable();

		}

	}

}
