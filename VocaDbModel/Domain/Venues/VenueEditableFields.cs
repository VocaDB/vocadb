using System;

namespace VocaDb.Model.Domain.Venues {

	[Flags]
	public enum VenueEditableFields {

		Nothing = 0,

		Coordinates = 1,

		Description = 2,

		Names = 4,

		OriginalName = 8,

		Status = 16,

		WebLinks = 32

	}

}
