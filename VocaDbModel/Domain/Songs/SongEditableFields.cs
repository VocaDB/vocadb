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

		Albums = 1,

		Artists = 2,

		Length = 4,

		Lyrics = 8,

		Names = 16,

		Notes = 32,

		OriginalName = 64,

		OriginalVersion = 128,

		PublishDate = 256,

		PVs = 512,

		ReleaseEvent = 1024,

		SongType = 2048,

		Status = 4096,

		WebLinks = 8192
	}
}
