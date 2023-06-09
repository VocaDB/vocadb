export enum ArtistRoles {
	Default = 0,

	/// <summary>
	/// Mostly PVs
	/// </summary>
	Animator = 1 << 0,

	/// <summary>
	/// Usually associated with remixes/covers
	/// </summary>
	Arranger = 1 << 1,

	Composer = 1 << 2,

	/// <summary>
	/// Usually circle/label
	/// </summary>
	Distributor = 1 << 3,

	/// <summary>
	/// PVs, cover art, booklet
	/// </summary>
	Illustrator = 1 << 4,

	/// <summary>
	/// Plays instruments
	/// </summary>
	Instrumentalist = 1 << 5,

	Lyricist = 1 << 6,

	Mastering = 1 << 7,

	/// <summary>
	/// Usually circle/label
	/// </summary>
	Publisher = 1 << 8,

	Vocalist = 1 << 9,

	/// <summary>
	/// Vocaloid voice manipulator
	/// </summary>
	VoiceManipulator = 1 << 10,

	Other = 1 << 11,

	Mixer = 1 << 12,

	/// <summary>
	/// For UtaiteDB.
	/// "Utaites sometimes like to provide backing vocals for other utaites, and it happens frequently enough that it should be defined as a role."
	/// </summary>
	Chorus = 1 << 13,

	/// <summary>
	/// For UtaiteDB.
	/// </summary>
	Encoder = 1 << 14,

	VocalDataProvider = 1 << 15,
}
