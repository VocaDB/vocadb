using System;

namespace VocaDb.Model.Domain.Venues {

	[Flags]
	public enum VenueEditableFields {

		Nothing = 0,

		Address = 1,

		AddressCountryCode = 2,

		Coordinates = 4,

		Description = 8,

		Names = 16,

		OriginalName = 32,

		Status = 64,

		WebLinks = 128

	}

}
