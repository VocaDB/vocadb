using System;

namespace VocaDb.Model.Domain.Venues {

	[Flags]
	public enum VenueEditableFields {

		Nothing = 0,

		Address = 1,

		Coordinates = 2,

		Description = 4,

		Names = 8,

		OriginalName = 16,

		RegionCode = 32,

		Status = 64,

		WebLinks = 128

	}

}
