namespace VocaDb.Model.Domain.Artists {

	public class ArtistMergeRecord : MergeRecord<Artist> {

		public ArtistMergeRecord() {}

		public ArtistMergeRecord(Artist source, Artist target)
			: base(source, target) {
		}

	}

}
