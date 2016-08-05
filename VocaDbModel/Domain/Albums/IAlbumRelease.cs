namespace VocaDb.Model.Domain.Albums {

	public interface IAlbumRelease {

		string CatNum { get; }

		int EventId { get; }

		string EventName { get; }

		IOptionalDateTime ReleaseDate { get; }

	}

}
