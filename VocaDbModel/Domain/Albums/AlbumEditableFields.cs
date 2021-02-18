#nullable disable

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

		Artists = 1 << 0,

		/// <summary>
		/// Identifiers list
		/// </summary>
		[Obsolete("Replaced by Identifiers")]
		Barcode = 1 << 1,

		Cover = 1 << 2,

		Description = 1 << 3,

		Discs = 1 << 4,

		DiscType = 1 << 5,

		Identifiers = 1 << 6,

		Names = 1 << 7,

		OriginalName = 1 << 8,

		OriginalRelease = 1 << 9,

		Pictures = 1 << 10,

		PVs = 1 << 11,

		Status = 1 << 12,

		Tracks = 1 << 13,

		WebLinks = 1 << 14,
	}
}
