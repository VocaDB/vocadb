using System;

namespace VocaDb.Model.Domain.ReleaseEvents {

	[Flags]
	public enum ReleaseEventSeriesEditableFields {

		Nothing = 0,

		Description = 1,

		Name = 2,

		Names = 4,

		Picture = 8,

		WebLinks = 16

	}

}
