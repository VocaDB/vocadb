using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Mapping.Tags {

	public class RelatedTagMap : ClassMap<RelatedTag> {

		public RelatedTagMap() {

			Cache.ReadOnly(); // Immutable
			Id(m => m.Id);

			References(m => m.OwnerTag).Not.Nullable().UniqueKey("IX_RelatedTags_Tag1_Tag2");
			References(m => m.LinkedTag).Not.Nullable().UniqueKey("IX_RelatedTags_Tag1_Tag2");

		}

	}

}
