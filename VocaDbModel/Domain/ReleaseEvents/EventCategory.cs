namespace VocaDb.Model.Domain.ReleaseEvents
{

	public enum EventCategory
	{
		Unspecified = 0,
		/// <summary>
		/// Comiket, VoMas
		/// </summary>
		AlbumRelease = 1,
		Anniversary = 2,
		/// <summary>
		/// Vocaloid Freak
		/// </summary>
		Club = 4,
		/// <summary>
		/// Magical Mirai
		/// </summary>
		Concert = 8,
		Contest = 16,
		/// <summary>
		/// Nico Chokaigi
		/// </summary>
		Convention = 32,
		Other = 64
	}

}
