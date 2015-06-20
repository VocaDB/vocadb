using System;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.ExtSites {

	public class AlbumDescriptionGenerator {

		public string GenerateDescription(AlbumContract album, Func<DiscType, string> albumTypeNames) {
					
			var typeName = albumTypeNames(album.DiscType);

			if (album.ReleaseDate.IsEmpty)
				return typeName;
			else
				return string.Format("{0}, released {1}", typeName, album.ReleaseDate.Formatted);

		}

	}

}
