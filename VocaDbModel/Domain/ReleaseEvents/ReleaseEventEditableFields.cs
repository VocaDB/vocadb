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

		Series          = 128,

		SeriesNumber	= 256,

		SeriesSuffix	= 512,

		SongList		= 1024,

		Status			= 2048,

		Venue			= 4096,

		WebLinks		= 8192

	}

}
