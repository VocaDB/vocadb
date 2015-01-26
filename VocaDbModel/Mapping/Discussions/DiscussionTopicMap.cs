using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.Mapping.Discussions {

	public class DiscussionTopicMap : ClassMap<DiscussionTopic> {

		public DiscussionTopicMap() {
			
			Schema("discussions");
			Table("DiscussionTopics");
			Cache.ReadWrite();

			Id(m => m.Id);

			Map(m => m.Content).Not.Nullable().Length(int.MaxValue);
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.Locked).Not.Nullable();
			Map(m => m.Pinned).Not.Nullable();
			Map(m => m.Title).Not.Nullable().Length(200);

			References(m => m.Author).Nullable();
			References(m => m.Folder).Not.Nullable();

			HasMany(m => m.Comments).Inverse().Cascade.AllDeleteOrphan();

		}

	}

}
