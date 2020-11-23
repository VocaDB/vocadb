using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Mapping.Tags
{
	public class TagMappingMap : ClassMap<TagMapping>
	{
		public TagMappingMap()
		{
			Id(m => m.Id);
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.SourceTag).Not.Nullable().Length(200);
			References(m => m.Tag).Not.Nullable();
		}
	}
}
