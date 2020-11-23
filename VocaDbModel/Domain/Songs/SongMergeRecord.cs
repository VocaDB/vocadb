namespace VocaDb.Model.Domain.Songs
{
	public class SongMergeRecord : MergeRecord<Song>
	{
		public SongMergeRecord() { }

		public SongMergeRecord(Song source, Song target)
			: base(source, target) { }
	}
}
