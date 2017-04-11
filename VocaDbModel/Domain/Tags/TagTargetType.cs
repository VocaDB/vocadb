using System;

namespace VocaDb.Model.Domain.Tags {

	/// <summary>
	/// Possible tag targets. This is a subset of combinations of <see cref="EntryType"/>.
	/// </summary>
	[Flags]
	public enum TagTargetTypes {
		/// <summary>
		/// Cannot be used for any entries (only for grouping other tags)
		/// </summary>
		Nothing = EntryType.Undefined,
		Album = EntryType.Album,
		Artist = EntryType.Artist,
		Song = EntryType.Song,
		AlbumArtist = Album | Artist,
		AlbumSong = Album | Song,
		ArtistSong = Artist | Song,
		/// <summary>
		/// Valid for all entry types (default)
		/// </summary>
		All = 1073741823
	}

}
