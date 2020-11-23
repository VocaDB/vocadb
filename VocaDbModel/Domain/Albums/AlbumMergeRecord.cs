namespace VocaDb.Model.Domain.Albums
{
	public class AlbumMergeRecord : MergeRecord<Album>
	{
		public AlbumMergeRecord() { }

		public AlbumMergeRecord(Album source, Album target)
			: base(source, target)
		{
		}
	}
}
