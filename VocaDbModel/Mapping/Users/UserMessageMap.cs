#nullable disable

using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users
{
	public class UserMessageMap : ClassMap<UserMessage>
	{
		public UserMessageMap()
		{
			Id(m => m.Id);

			Map(m => m.CreatedUtc).Not.Nullable();
			Map(m => m.Inbox).Not.Nullable();
			Map(m => m.Subject).Length(200).Not.Nullable();
			Map(m => m.Message).Length(10000).Not.Nullable();
			Map(m => m.Read).Not.Nullable();

			References(m => m.Sender).Nullable();
			References(m => m.Receiver).Not.Nullable();
			References(m => m.User).Not.Nullable();
		}
	}
}
