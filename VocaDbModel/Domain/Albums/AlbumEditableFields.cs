using System;

namespace VocaDb.Model.Domain.Albums {

	/// <summary>
	/// Saved in the database as strings - numbers can be changed, but not the names.
	/// </summary>
	[Flags]
	public enum AlbumEditableFields {

		Nothing			= 0,

		Artists			= 1,

		/// <summary>
		/// Identifiers list
		/// </summary>
		Barcode			= 2,

		Cover			= 4,

		Description		= 8,

		DiscType		= 16,

		Identifiers		= 32,

		Names			= 64,

		OriginalName	= 128,

		OriginalRelease	= 256,

		Pictures		= 512,

		PVs				= 1024,

		Status			= 2048,

		Tracks			= 4096,

		WebLinks		= 8192

	}

}
