namespace VocaDb.Model.Domain.Albums {

	public interface IAlbumRelease {

		string CatNum { get; }

		IOptionalDateTime ReleaseDate { get; }

	}

}
