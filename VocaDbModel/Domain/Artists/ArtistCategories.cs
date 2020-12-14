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

		Vocalist = 1,

		Producer = 2,

		Animator = 4,

		Label = 8,

		Circle = 16,

		/// <summary>
		/// Instrumentalist, lyricist, etc.
		/// </summary>
		Other = 32,

		Band = 64,

		Illustrator = 128,

		Subject = 256
	}
}
