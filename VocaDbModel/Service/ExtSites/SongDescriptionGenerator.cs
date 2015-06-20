using System;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.ExtSites {

	public class SongDescriptionGenerator {

		public string GenerateDescription(SongContract song, Func<SongType, string> songTypeNames) {
					
			var typeName = songTypeNames(song.SongType);

			if (!song.PublishDate.HasValue)
				return typeName;
			else
				return string.Format("{0}, published {1}", typeName, song.PublishDate.Value.ToShortDateString());

		}

	}

}
