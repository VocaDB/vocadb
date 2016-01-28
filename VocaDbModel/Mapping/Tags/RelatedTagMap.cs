using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Mapping.Tags {

	public class RelatedTagMap : ClassMap<RelatedTag> {

		public RelatedTagMap() {

			Cache.ReadOnly(); // Immutable
			Id(m => m.Id);

			References(m => m.Tag1).Not.Nullable().UniqueKey("IX_RelatedTags_Tag1_Tag2");
			References(m => m.Tag2).Not.Nullable().UniqueKey("IX_RelatedTags_Tag1_Tag2");

		}

	}

}
