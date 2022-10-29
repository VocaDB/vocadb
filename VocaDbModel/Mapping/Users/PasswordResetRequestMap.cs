#nullable disable

using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users
{
	public class PasswordResetRequestMap : ClassMap<PasswordResetRequest>
	{
		public PasswordResetRequestMap()
		{
			Id(m => m.Id).GeneratedBy.GuidComb();

			Map(m => m.CreatedUtc).Not.Nullable();
			Map(m => m.Email).Not.Nullable().Length(50);

			References(m => m.User).Not.Nullable();
		}
	}
}
