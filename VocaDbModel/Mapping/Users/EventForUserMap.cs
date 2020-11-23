using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users
{

	public class EventForUserMap : ClassMap<EventForUser>
	{
		public EventForUserMap()
		{

			Table("EventsForUsers");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.RelationshipType).Not.Nullable();

			References(m => m.ReleaseEvent).Not.Nullable();
			References(m => m.User).Not.Nullable();

		}
	}
}
