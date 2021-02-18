#nullable disable

using System;

namespace VocaDb.Model.Domain.Artists
{
	/// <summary>
	/// Artist categories are shown as rows on song/album details page,
	/// with their own title.
	/// </summary>
	[Flags]
	public enum ArtistCategories
	{
		Nothing = 0,

		Vocalist = 1 << 0,

		Producer = 1 << 1,

		Animator = 1 << 2,

		Label = 1 << 3,

		Circle = 1 << 4,

		/// <summary>
		/// Instrumentalist, lyricist, etc.
		/// </summary>
		Other = 1 << 5,

		Band = 1 << 6,

		Illustrator = 1 << 7,

		Subject = 1 << 8,
	}
}
