#nullable disable

using System;

namespace VocaDb.Model.Domain.Songs
{
	/// <summary>
	/// Saved in the database as strings - numbers can be changed, but not the names.
	/// </summary>
	[Flags]
	public enum SongEditableFields
	{
		Nothing = 0,

		Albums = 1 << 0,

		Artists = 1 << 1,

		Length = 1 << 2,

		Lyrics = 1 << 3,

		Names = 1 << 4,

		Notes = 1 << 5,

		OriginalName = 1 << 6,

		OriginalVersion = 1 << 7,

		PublishDate = 1 << 8,

		PVs = 1 << 9,

		ReleaseEvent = 1 << 10,

		SongType = 1 << 11,

		Status = 1 << 12,

		WebLinks = 1 << 13,

		Bpm = 1 << 14,
	}
}
