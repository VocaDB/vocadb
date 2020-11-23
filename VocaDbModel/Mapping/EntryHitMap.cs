using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Mapping
{
	public class EntryHitMap<THit, TEntry> : ClassMap<THit> where THit : GenericEntryHit<TEntry> where TEntry : class
	{
		protected EntryHitMap()
		{
			Id(m => m.Id);
			ReadOnly();

			Map(m => m.Agent).Not.Nullable();
			Map(m => m.Date).Not.Nullable().Generated.Always(); ;
			References(m => m.Entry).Not.Nullable().Column(ClassConventions.EscapeColumn(typeof(TEntry).Name));
		}
	}
}
