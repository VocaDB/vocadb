#nullable disable

namespace VocaDb.Model.Domain.ReleaseEvents
{
	public enum EventCategory
	{
		Unspecified = 0,
		/// <summary>
		/// Comiket, VoMas
		/// </summary>
		AlbumRelease = 1 << 0,
		Anniversary = 1 << 1,
		/// <summary>
		/// Vocaloid Freak
		/// </summary>
		Club = 1 << 2,
		/// <summary>
		/// Magical Mirai
		/// </summary>
		Concert = 1 << 3,
		Contest = 1 << 4,
		/// <summary>
		/// Nico Chokaigi
		/// </summary>
		Convention = 1 << 5,
		Other = 1 << 6,
	}
}
