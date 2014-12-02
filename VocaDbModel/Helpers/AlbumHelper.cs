using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Helpers {

	public static class AlbumHelper {

		public static bool IsAnimation(DiscType t) {
			return (t == DiscType.Video);
		}

	}

}
