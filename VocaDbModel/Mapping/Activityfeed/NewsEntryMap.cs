using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.Mapping.Activityfeed {

	public class NewsEntryMap : ClassMap<NewsEntry> {

		public NewsEntryMap() {

			Table("NewsEntries");
			Id(m => m.Id);
			Cache.ReadWrite();

			Map(m => m.Anonymous).Not.Nullable();
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Important).Not.Nullable();
			Map(m => m.Stickied).Not.Nullable();
			Map(m => m.Text).Length(2000).Not.Nullable();

			References(m => m.Author).Not.Nullable();

		}

	}
}
