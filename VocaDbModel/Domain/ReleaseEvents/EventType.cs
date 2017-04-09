namespace VocaDb.Model.Domain.ReleaseEvents {

	public enum EventType {
		Unspecified = 0,
		/// <summary>
		/// Comiket, VoMas
		/// </summary>
		AlbumRelease = 1,
		CharacterAnniversary = 2,
		/// <summary>
		/// Vocaloid Freak
		/// </summary>
		Club = 4,
		/// <summary>
		/// Magical Mirai
		/// </summary>
		Concert = 8,
		/// <summary>
		/// Nico Chokaigi
		/// </summary>
		Convention = 16,
		Other = 32
	}

}
