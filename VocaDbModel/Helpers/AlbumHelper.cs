using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Helpers {

	public static class AlbumHelper {

		public static ContentFocus GetContentFocus(DiscType t) {
			return (t == DiscType.Video ? ContentFocus.Video : ContentFocus.Music);
		}

	}

}
