#nullable disable

using System;

namespace VocaDb.Model.Domain.Artists
{
	/// <summary>
	/// Artist type (category).
	/// </summary>
	/// <remarks>
	/// Do not persist the numeric values anywhere - they may change.
	/// Names must match with <see cref="ArtistTypes"/>.
	/// </remarks>
	public enum ArtistType
	{
		Unknown,

		/// <summary>
		/// Doujin circle. A group of doujin producers that also releases music (acts as a label).
		/// </summary>
		Circle,

		/// <summary>
		/// Commercial music label. Does not produce music by itself.
		/// </summary>
		Label,

		/// <summary>
		/// Producer is the maker or the song (usually an individual, for example doriko)
		/// </summary>
		Producer,

		Animator,

		Illustrator,

		Lyricist,

		Vocaloid,

		UTAU,

		CeVIO,

		OtherVoiceSynthesizer,

		OtherVocalist,

		OtherGroup,

		OtherIndividual,

		Utaite,

		Band,

		Vocalist,

		Character,
	}

	/// <summary>
	/// Bitarray of artist types. The numeric values shouldn't be saved anywhere because they can change.
	/// Prefer saving the individual values from <see cref="ArtistType"/>.
	/// </summary>
	[Flags]
	public enum ArtistTypes
	{
		Unknown = 0,

		Circle = 1 << 0,

		Label = 1 << 1,

		Producer = 1 << 2,

		Animator = 1 << 3,

		Illustrator = 1 << 4,

		Lyricist = 1 << 5,

		Vocaloid = 1 << 6,

		UTAU = 1 << 7,

		CeVIO = 1 << 8,

		OtherVoiceSynthesizer = 1 << 9,

		OtherVocalist = 1 << 10,

		OtherGroup = 1 << 11,

		OtherIndividual = 1 << 12,

		Utaite = 1 << 13,

		Band = 1 << 14,

		Vocalist = 1 << 15,

		Character = 1 << 16,
	}
}
