using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Mapping {

	public class TrashedEntryMap : ClassMap<TrashedEntry> {

		public TrashedEntryMap() {

			Cache.ReadWrite();
			Table("TrashedEntries");
			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.EntryId).Not.Nullable();
			Map(m => m.EntryType).Not.Nullable();
			Map(m => m.Name).Not.Nullable().Length(255);
			Map(m => m.Notes).Not.Nullable().Length(200);

			References(m => m.User).Not.Nullable();

		}

	}

}
