using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Mapping.Tags
{

	public class EntryTypeToTagMappingMap : ClassMap<EntryTypeToTagMapping>
	{
		public EntryTypeToTagMappingMap()
		{
			Id(m => m.Id);
			Map(m => m.EntryType).Not.Nullable();
			Map(m => m.SubType).Not.Nullable();
			References(m => m.Tag).Not.Nullable();
		}
	}
}
