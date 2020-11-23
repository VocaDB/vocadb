using System;

namespace VocaDb.Model.Domain.ReleaseEvents
{

	[Flags]
	public enum ReleaseEventSeriesEditableFields
	{

		Nothing = 0,

		Category = 1,

		Description = 2,

		[Obsolete]
		Name = 4,

		Names = 8,

		OriginalName = 16,

		Picture = 32,

		Status = 64,

		WebLinks = 128

	}

}
