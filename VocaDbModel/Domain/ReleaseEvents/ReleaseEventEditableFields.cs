using System;

namespace VocaDb.Model.Domain.ReleaseEvents {

	[Flags]
	public enum ReleaseEventEditableFields {

		Nothing			= 0,

		Category		= 1,

		Date			= 2,

		Description		= 4,

		MainPicture		= 8,

		Name			= 16,

		Series			= 32,

		SeriesNumber	= 64,

		SeriesSuffix	= 128,

		SongList		= 256,

		Status			= 512,

		Venue			= 1024,

		WebLinks		= 2048

	}

}
