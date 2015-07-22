using System.Resources;

namespace VocaDb.Model.Service.BrandableStrings.Collections {

	public class SongStrings : ResStringCollection {

		public SongStrings(ResourceManager resourceMan) 
			: base(resourceMan) {}

		public string NewSongInfo => GetString("NewSongInfo");

		public string RankingsTitle => GetString("RankingsTitle");

	}

}
