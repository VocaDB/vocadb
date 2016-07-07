using System;

namespace VocaDb.Model.Domain.ReleaseEvents {

	[Flags]
	public enum ReleaseEventEditableFields {

		Nothing			= 0,

		Date			= 1,

		Description		= 2,

		Name			= 4,

		Series			= 8,

		SeriesNumber	= 16,

		SeriesSuffix	= 32,

	}

}
