using System;

namespace VocaDb.Model.Domain.Albums
{
	/// <summary>
	/// Saved in the database as strings - numbers can be changed, but not the names.
	/// </summary>
	[Flags]
	public enum AlbumEditableFields
	{
		Nothing = 0,

		Artists = 1,

		/// <summary>
		/// Identifiers list
		/// </summary>
		[Obsolete("Replaced by Identifiers")]
		Barcode = 2,

		Cover = 4,

		Description = 8,

		Discs = 16,

		DiscType = 32,

		Identifiers = 64,

		Names = 128,

		OriginalName = 256,

		OriginalRelease = 512,

		Pictures = 1024,

		PVs = 2048,

		Status = 4096,

		Tracks = 8192,

		WebLinks = 16384
	}
}
