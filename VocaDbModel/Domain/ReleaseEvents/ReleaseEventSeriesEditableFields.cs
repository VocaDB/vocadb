using System;

namespace VocaDb.Model.Domain.ReleaseEvents {

	[Flags]
	public enum ReleaseEventSeriesEditableFields {

		Nothing = 0,

		Category = 1,

		Description = 2,

		Name = 4,

		Names = 8,

		Picture = 16,

		Status = 32,

		WebLinks = 64

	}

}
