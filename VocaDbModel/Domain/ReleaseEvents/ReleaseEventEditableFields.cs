using System;

namespace VocaDb.Model.Domain.ReleaseEvents {

	[Flags]
	public enum ReleaseEventEditableFields {

		Nothing			= 0,

		Date			= 1,

		Description		= 2,

		MainPicture		= 4,

		Name			= 8,

		Series			= 16,

		SeriesNumber	= 32,

		SeriesSuffix	= 64,

		SongList		= 128,

		Venue			= 256,

		WebLinks		= 512

	}

}
