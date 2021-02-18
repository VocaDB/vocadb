#nullable disable

using System;

namespace VocaDb.Model.Domain.ReleaseEvents
{
	[Flags]
	public enum ReleaseEventSeriesEditableFields
	{
		Nothing = 0,

		Category = 1 << 0,

		Description = 1 << 1,

		[Obsolete]
		Name = 1 << 2,

		Names = 1 << 3,

		OriginalName = 1 << 4,

		Picture = 1 << 5,

		Status = 1 << 6,

		WebLinks = 1 << 7,
	}
}
