using System;

namespace VocaDb.Model.Domain.Artists
{
	/// <summary>
	/// Possible artist roles for albums and songs.
	/// Saved into database as a bitarray - do not change the number values.
	/// Also remember to update the TypeScript file (ArtistRoles.ts).
	/// </summary>
	[Flags]
	public enum ArtistRoles
	{
		Default = 0,

		/// <summary>
		/// Mostly PVs
		/// </summary>
		Animator = 1,

		/// <summary>
		/// Usually associated with remixes/covers
		/// </summary>
		Arranger = 2,

		Composer = 4,

		/// <summary>
		/// Usually circle/label
		/// </summary>
		Distributor = 8,

		/// <summary>
		/// PVs, cover art, booklet
		/// </summary>
		Illustrator = 16,

		/// <summary>
		/// Plays instruments
		/// </summary>
		Instrumentalist = 32,

		Lyricist = 64,

		Mastering = 128,

		/// <summary>
		/// Usually circle/label
		/// </summary>
		Publisher = 256,

		Vocalist = 512,

		/// <summary>
		/// Vocaloid voice manipulator
		/// </summary>
		VoiceManipulator = 1024,

		Other = 2048,

		Mixer = 4096,

		/// <summary>
		/// For UtaiteDB.
		/// "Utaites sometimes like to provide backing vocals for other utaites, and it happens frequently enough that it should be defined as a role."
		/// </summary>
		Chorus = 8192,

		/// <summary>
		/// For UtaiteDB.
		/// </summary>
		Encoder = 16384,

		VocalDataProvider = 32768
	}
}
