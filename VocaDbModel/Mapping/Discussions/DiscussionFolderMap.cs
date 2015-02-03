using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.Mapping.Discussions {

	public class DiscussionFolderMap : ClassMap<DiscussionFolder> {

		public DiscussionFolderMap() {
			
			Schema("discussions");
			Table("DiscussionFolders");
			Cache.ReadWrite();

			Id(m => m.Id);

			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.Description).Not.Nullable().Length(int.MaxValue);
			Map(m => m.Name).Not.Nullable().Length(200);
			Map(m => m.SortIndex).Not.Nullable();

			HasMany(m => m.Topics).KeyColumn("[Folder]").Inverse().Cache.ReadWrite();

		}

	}
}
