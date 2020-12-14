#nullable disable


namespace VocaDb.Model.Domain.Songs
{
	public class SongHit : GenericEntryHit<Song>
	{
		public SongHit() { }

		public SongHit(Song song, int agent)
			: base(song, agent) { }
	}
}
