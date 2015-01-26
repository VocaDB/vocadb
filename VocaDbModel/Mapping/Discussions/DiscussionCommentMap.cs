using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.Mapping.Discussions {

	public class DiscussionCommentMap : ClassMap<DiscussionComment> {

		public DiscussionCommentMap() {
			
			Schema("discussions");
			Table("DiscussionComments");
			Cache.ReadWrite();

			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Message).Length(800).Not.Nullable();

			References(m => m.Author).Not.Nullable();
			References(m => m.Topic).Not.Nullable();

		}

	}
}
