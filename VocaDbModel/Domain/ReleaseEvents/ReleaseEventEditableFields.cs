#nullable disable

using System;

namespace VocaDb.Model.Domain.ReleaseEvents
{
	[Flags]
	public enum ReleaseEventEditableFields
	{
		Nothing = 0,

		Artists = 1 << 0,

		Category = 1 << 1,

		Date = 1 << 2,

		Description = 1 << 3,

		MainPicture = 1 << 4,

		[Obsolete]
		Name = 1 << 5,

		Names = 1 << 6,

		OriginalName = 1 << 7,

		PVs = 1 << 8,

		Series = 1 << 9,

		SeriesNumber = 1 << 10,

		SeriesSuffix = 1 << 11,

		SongList = 1 << 12,

		Status = 1 << 13,

		Venue = 1 << 14,

		VenueName = 1 << 15,

		WebLinks = 1 << 16,
	}
}
