using System;

namespace VocaDb.Model.Domain.ReleaseEvents {

	[Flags]
	public enum ReleaseEventEditableFields {

		Nothing			= 0,

		Category		= 1,

		Date			= 2,

		Description		= 4,

		MainPicture		= 8,

		[Obsolete]
		Name			= 16,

		Names           = 32,

		OriginalName    = 64,

		PVs				= 128,

		Series          = 256,

		SeriesNumber	= 512,

		SeriesSuffix	= 1024,

		SongList		= 2048,

		Status			= 4096,

		Venue			= 8192,

		WebLinks		= 16384

	}

}
