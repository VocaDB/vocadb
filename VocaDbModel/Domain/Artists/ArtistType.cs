using System;

namespace VocaDb.Model.Domain.Artists {

	/// <summary>
	/// Artist type (category).
	/// </summary>
	/// <remarks>
	/// Do not persist the numeric values anywhere - they may change.
	/// Names must match with <see cref="ArtistTypes"/>.
	/// </remarks>
	public enum ArtistType {

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

		Band

	}

	/// <summary>
	/// Bitarray of artist types. The numeric values shouldn't be saved anywhere because they can change.
	/// Prefer saving the individual values from <see cref="ArtistType"/>.
	/// </summary>
	[Flags]
	public enum ArtistTypes {
	
		Unknown = 0,

		Circle = 1,

		Label = 2,

		Producer = 4,

		Animator = 8,

		Illustrator = 16,

		Lyricist = 32,

		Vocaloid = 64,

		UTAU = 128,

		CeVIO = 256,

		OtherVoiceSynthesizer = 512,

		OtherVocalist = 1024,

		OtherGroup = 2048,

		OtherIndividual = 4096,

		Utaite = 8192,

		Band = 16384

	}

}
