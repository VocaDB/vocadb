using System;

namespace VocaDb.Model.Domain.Tags
{

	/// <summary>
	/// Possible tag targets. This is a subset of combinations of <see cref="EntryType"/>.
	/// </summary>
	/// <remarks>
	/// Note: hybrid values such as AlbumArtist are no longer used, but they must not be marked with the ObsoleteAttribute
	/// because that makes them being ignored by XmlSerializer (see https://stackoverflow.com/questions/331013/obsolete-attribute-causes-property-to-be-ignored-by-xmlserialization).
	/// </remarks>
	[Flags]
	public enum TagTargetTypes
	{
		/// <summary>
		/// Cannot be used for any entries (only for grouping other tags)
		/// </summary>
		Nothing = EntryType.Undefined,
		Album = EntryType.Album,
		Artist = EntryType.Artist,
		Song = EntryType.Song,
		/// <summary>
		/// Obsolete
		/// </summary>
		AlbumArtist = Album | Artist,
		/// <summary>
		/// Obsolete
		/// </summary>
		AlbumSong = Album | Song,
		/// <summary>
		/// Obsolete
		/// </summary>
		ArtistSong = Artist | Song,
		Event = EntryType.ReleaseEvent,
		/// <summary>
		/// Valid for all entry types (default)
		/// </summary>
		All = 1073741823
	}

}
