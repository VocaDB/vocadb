using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users
{
	public class TagForUserMap : ClassMap<TagForUser>
	{
		public TagForUserMap()
		{
			Table("TagsForUsers");
			Cache.ReadWrite();
			Id(m => m.Id);

			References(m => m.Tag).Not.Nullable();
			References(m => m.User).Not.Nullable();
		}
	}
}
