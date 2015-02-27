using System;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.ExtSites {

	public class AlbumDescriptionGenerator {

		public string GenerateDescription(AlbumContract album, Func<DiscType, string> albumTypeNames) {
					
			if (album.ReleaseDate.IsEmpty)
				return albumTypeNames(album.DiscType);
			else
				return string.Format("{0}, released {1}", albumTypeNames(album.DiscType), album.ReleaseDate.Formatted);

		}

	}

}
