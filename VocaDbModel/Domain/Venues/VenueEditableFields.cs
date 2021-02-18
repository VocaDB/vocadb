#nullable disable

using System;

namespace VocaDb.Model.Domain.Venues
{
	[Flags]
	public enum VenueEditableFields
	{
		Nothing = 0,

		Address = 1 << 0,

		AddressCountryCode = 1 << 1,

		Coordinates = 1 << 2,

		Description = 1 << 3,

		Names = 1 << 4,

		OriginalName = 1 << 5,

		Status = 1 << 6,

		WebLinks = 1 << 7,
	}
}
