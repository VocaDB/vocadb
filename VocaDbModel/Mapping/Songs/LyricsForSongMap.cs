using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class LyricsForSongMap : ClassMap<LyricsForSong> {

		public LyricsForSongMap() {

			Table("LyricsForSongs");

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Language).Not.Nullable();
			Map(m => m.Source).Length(255).Not.Nullable();
			Map(m => m.Value).Column("Text").Length(int.MaxValue).Not.Nullable();
			References(m => m.Song).Not.Nullable();

		}


	}

}
