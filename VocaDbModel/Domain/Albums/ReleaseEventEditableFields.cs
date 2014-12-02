using System;

namespace VocaDb.Model.Domain.Albums {

	[Flags]
	public enum ReleaseEventEditableFields {

		Nothing			= 0,

		Date			= 1,

		Description		= 2,

		Name			= 4,

		SeriesNumber	= 8,

		SeriesSuffix	= 16,

	}

}
