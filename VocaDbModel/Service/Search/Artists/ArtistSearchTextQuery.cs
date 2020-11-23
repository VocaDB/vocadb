using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Search.Artists
{

	public class ArtistSearchTextQuery : SearchTextQuery
	{

		public static new ArtistSearchTextQuery Empty => new ArtistSearchTextQuery();

		public static new ArtistSearchTextQuery Create(string query,
			NameMatchMode selectedMode = NameMatchMode.Auto,
			NameMatchMode defaultMode = NameMatchMode.Words)
		{

			var parsedQuery = FindHelpers.GetMatchModeAndQueryForSearch(query, ref selectedMode, defaultMode);
			var canonizedName = ArtistHelper.GetCanonizedName(parsedQuery);
			return new ArtistSearchTextQuery(canonizedName, selectedMode, query);

		}

		public static ArtistSearchTextQuery Create(SearchTextQuery textQuery)
		{

			var canonizedName = ArtistHelper.GetCanonizedName(textQuery.Query);

			// Can't use the existing words collection here as they are noncanonized
			return new ArtistSearchTextQuery(canonizedName, textQuery.MatchMode, textQuery.OriginalQuery);

		}

		public ArtistSearchTextQuery() { }

		public ArtistSearchTextQuery(string canonizedName, NameMatchMode matchMode, string originalQuery, string[] words = null)
			: base(canonizedName, matchMode, originalQuery, words)
		{

		}

		public ArtistSearchTextQuery OverrideMatchMode(NameMatchMode? matchMode)
		{
			return matchMode.HasValue ? new ArtistSearchTextQuery(Query, matchMode.Value, OriginalQuery, words) : this;
		}

	}

}
