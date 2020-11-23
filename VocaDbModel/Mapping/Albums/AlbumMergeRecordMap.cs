using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums
{

	public class AlbumMergeRecordMap : ClassMap<AlbumMergeRecord>
	{

		public AlbumMergeRecordMap()
		{

			Id(m => m.Id);

			Map(m => m.Source).Not.Nullable();
			References(m => m.Target).Not.Nullable();

		}

	}

}
