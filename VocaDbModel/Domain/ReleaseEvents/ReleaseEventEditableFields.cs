using System;

namespace VocaDb.Model.Domain.ReleaseEvents
{

	[Flags]
	public enum ReleaseEventEditableFields
	{

		Nothing = 0,

		Artists = 1,

		Category = 2,

		Date = 4,

		Description = 8,

		MainPicture = 16,

		[Obsolete]
		Name = 32,

		Names = 64,

		OriginalName = 128,

		PVs = 256,

		Series = 512,

		SeriesNumber = 1024,

		SeriesSuffix = 2048,

		SongList = 4096,

		Status = 8192,

		Venue = 16384,

		VenueName = 32768,

		WebLinks = 65536

	}

}
