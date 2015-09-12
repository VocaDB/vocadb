using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.Mapping.Discussions {

	public class DiscussionCommentMap : ClassMap<DiscussionComment> {

		public DiscussionCommentMap() {
			
			Schema("discussions");
			Table("DiscussionComments");
			Cache.ReadWrite();

			Id(m => m.Id);

			Map(m => m.AuthorName).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Message).Length(4000).Not.Nullable();

			References(m => m.Author).Not.Nullable();
			References(m => m.EntryForComment).Column("[Topic]").Not.Nullable();

		}

	}
}
