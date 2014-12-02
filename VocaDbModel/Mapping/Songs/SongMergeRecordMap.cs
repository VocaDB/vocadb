using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class SongMergeRecordMap : ClassMap<SongMergeRecord> {

		public SongMergeRecordMap() {

			Id(m => m.Id);

			Map(m => m.Source).Not.Nullable();
			References(m => m.Target).Not.Nullable();

		}

	}

}
