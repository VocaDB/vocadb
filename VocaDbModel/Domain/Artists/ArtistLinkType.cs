#nullable disable

namespace VocaDb.Model.Domain.Artists
{
	/// <summary>
	/// Type of (bidirectional) link between artists.
	/// </summary>
	public enum ArtistLinkType
	{
		/// <summary>
		/// Character designer / designer of.
		/// By default the same as Illustrator.
		/// </summary>
		CharacterDesigner,

		/// <summary>
		/// Group/member
		/// </summary>
		Group,

		/// <summary>
		/// Illustrator/illustration.
		/// </summary>
		Illustrator,

		/// <summary>
		/// Manager / manager of.
		/// By default the same as VoiceProvider
		/// </summary>
		Manager,

		/// <summary>
		/// Voice provider / voice provider of.
		/// </summary>
		VoiceProvider
	}
}
