namespace VocaDb.Model.Domain.Images {

	/// <summary>
	/// Persister for album/artist additional images and tag/songlist images.
	/// Can be migrated to <see cref="IEntryThumbPersister"/> when images are moved to static domain.
	/// </summary>
	public interface IEntryImagePersisterOld : IEntryImagePersister {}

}
