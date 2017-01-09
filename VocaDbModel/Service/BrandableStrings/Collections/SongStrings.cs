using System.Resources;
using VocaDb.Model.Resources.Views;

namespace VocaDb.Model.Service.BrandableStrings.Collections {

	public class SongStrings : ResStringCollection {

		public SongStrings(ResourceManager resourceMan) 
			: base(resourceMan) {}

		public string NewSongInfo => GetString(nameof(SongRes.NewSongInfo));
		public string RankingsTitle => GetString(nameof(SongRes.RankingsTitle));

	}

}
