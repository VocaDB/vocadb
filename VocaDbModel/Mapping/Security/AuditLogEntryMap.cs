using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Mapping.Security
{
	public class AuditLogEntryMap : ClassMap<AuditLogEntry>
	{
		public AuditLogEntryMap()
		{
			Table("AuditLogEntries");
			Id(m => m.Id);

			Map(m => m.Action).Length(400).Not.Nullable();
			Map(m => m.AgentName).Length(100).Not.Nullable();
			Map(m => m.Time).Not.Nullable();
			References(m => m.User).Nullable();

			Component(m => m.EntryId, c =>
			{
				c.Map(m => m.EntryType, "EntryType");
				c.Map(m => m.Id, "EntryId");
			});
		}
	}
}
