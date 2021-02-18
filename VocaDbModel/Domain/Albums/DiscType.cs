#nullable disable

namespace VocaDb.Model.Domain.Albums
{
	/// <summary>
	/// Album disc type.
	/// </summary>
	public enum DiscType
	{
		Unknown = 0,

		/// <summary>
		/// Original album (default)
		/// </summary>
		Album = 1 << 0,

		Single = 1 << 1,

		EP = 1 << 2,

		SplitAlbum = 1 << 3,

		Compilation = 1 << 4,

		Video = 1 << 5,

		Artbook = 1 << 6,

		/// <summary>
		/// For TouhouDB
		/// </summary>
		Game = 1 << 7,

		/// <summary>
		/// For TouhouDB
		/// </summary>
		Fanmade = 1 << 8,

		Instrumental = 1 << 9,

		Other = 1 << 10,
	}
}
