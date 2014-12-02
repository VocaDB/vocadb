namespace VocaDb.Model.Domain.Albums {

	public interface IAlbumRelease {

		string CatNum { get; }

		string EventName { get; }

		IOptionalDateTime ReleaseDate { get; }

	}

}
