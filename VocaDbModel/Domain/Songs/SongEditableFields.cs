using System;

namespace VocaDb.Model.Domain.Songs {

	[Flags]
	public enum SongEditableFields {

		Nothing			= 0,

		Artists			= 1,

		Length			= 2,

		Lyrics			= 4,

		Names			= 8,

		Notes			= 16,

		OriginalName	= 32,

		OriginalVersion	= 64,

		PVs				= 128,

		SongType		= 256,

		Status			= 512,

		WebLinks		= 1024

	}

}
