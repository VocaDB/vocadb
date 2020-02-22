using System;

namespace VocaDb.Model.Domain.Venues {

	[Flags]
	public enum VenueEditableFields {

		Nothing = 0,

		Description = 1,

		Names = 2,

		OriginalName = 4,

		Status = 8,

		WebLinks = 16

	}

}
