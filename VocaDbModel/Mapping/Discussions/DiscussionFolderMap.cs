using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.Mapping.Discussions {

	public class DiscussionFolderMap : ClassMap<DiscussionFolder> {

		public DiscussionFolderMap() {
			
			Schema("discussions");
			Table("DiscussionFolders");
			Cache.ReadWrite();

			Id(m => m.Id);

			Map(m => m.Description).Not.Nullable().Length(int.MaxValue);
			Map(m => m.Title).Not.Nullable().Length(200);

			HasMany(m => m.Topics).KeyColumn("[Folder]").Inverse();

		}

	}
}
