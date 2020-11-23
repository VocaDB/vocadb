using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Mapping.Tags
{
	public class TagMergeRecordMap : ClassMap<TagMergeRecord>
	{
		public TagMergeRecordMap()
		{
			Id(m => m.Id);

			Map(m => m.Source).Not.Nullable();
			References(m => m.Target).Not.Nullable();
		}
	}
}
