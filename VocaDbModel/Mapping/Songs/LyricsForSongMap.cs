using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs
{
	public class LyricsForSongMap : ClassMap<LyricsForSong>
	{
		public LyricsForSongMap()
		{
			Table("LyricsForSongs");

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Source).Length(255).Not.Nullable();
			Map(m => m.TranslationType).Not.Nullable();
			Map(m => m.URL).Length(500).Not.Nullable();
			Map(m => m.Value).Column("Text").Length(int.MaxValue).Not.Nullable();
			References(m => m.Song).Not.Nullable();

			Component(m => m.CultureCode, c =>
			{
				c.Map(m => m.CultureCode).Column("[CultureCode]").Length(20).Not.Nullable();
			});
		}
	}
}
