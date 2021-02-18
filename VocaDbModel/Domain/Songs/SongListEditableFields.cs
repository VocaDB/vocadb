#nullable disable

using System;

namespace VocaDb.Model.Domain.Songs
{
	[Flags]
	public enum SongListEditableFields
	{
		Nothing = 0,

		Description = 1 << 0,

		EventDate = 1 << 1,

		FeaturedCategory = 1 << 2,

		Name = 1 << 3,

		Songs = 1 << 4,

		Status = 1 << 5,

		Thumbnail = 1 << 6,
	}
}
