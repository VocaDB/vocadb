#nullable disable

using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users
{
	public class OwnedArtistForUserMap : ClassMap<OwnedArtistForUser>
	{
		public OwnedArtistForUserMap()
		{
			Table("OwnedArtistsForUsers");
			Cache.ReadWrite();
			Id(m => m.Id);

			References(m => m.Artist).Not.Nullable();
			References(m => m.User).Not.Nullable();
		}
	}
}
