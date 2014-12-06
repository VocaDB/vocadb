using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Search.Artists {

	public class ArtistSearchTextQuery : SearchTextQuery {

		public static new ArtistSearchTextQuery Create(string query, 
			NameMatchMode selectedMode = NameMatchMode.Auto, 
			NameMatchMode defaultMode = NameMatchMode.Words) {
			
			query = FindHelpers.GetMatchModeAndQueryForSearch(query, ref selectedMode, defaultMode);
			var canonizedName = ArtistHelper.GetCanonizedName(query);
			return new ArtistSearchTextQuery(query, selectedMode, canonizedName);

		}

		public ArtistSearchTextQuery(string query, NameMatchMode matchMode, string canonizedName) 
			: base(canonizedName, matchMode) {
			
			OriginalQuery = query;

		}

		public string OriginalQuery { get; private set; }

	}

}
