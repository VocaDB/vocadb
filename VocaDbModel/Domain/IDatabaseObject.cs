namespace VocaDb.Model.Domain {

	/// <summary>
	/// Base interface for all objects saved in database.
	/// Applies to both root entities and child entities.
	/// The ID is usually Int32, but may also be Int64 or Guid.
	/// </summary>
	/// <remarks>
	/// Root entities are entities that don't depend on anything else,
	/// such as Album, Artist, Song.
	/// 
	/// Child entities depend on one or more root entities, for example
	/// AlbumName, TagUsage.
	/// </remarks>
	public interface IDatabaseObject {

	}

}
