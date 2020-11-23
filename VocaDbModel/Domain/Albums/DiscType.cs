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
		Album = 1,

		Single = 2,

		EP = 4,

		SplitAlbum = 8,

		Compilation = 16,

		Video = 32,

		Artbook = 64,

		/// <summary>
		/// For TouhouDB
		/// </summary>
		Game = 128,

		/// <summary>
		/// For TouhouDB
		/// </summary>
		Fanmade = 256,

		Instrumental = 512,

		Other = 1024
	}
}
