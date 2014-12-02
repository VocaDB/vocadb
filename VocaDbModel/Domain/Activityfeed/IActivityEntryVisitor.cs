namespace VocaDb.Model.Domain.Activityfeed {

	public interface IActivityEntryVisitor {

		void Visit(AlbumActivityEntry entry);

		void Visit(ArtistActivityEntry entry);

		void Visit(SongActivityEntry entry);

	}
}
