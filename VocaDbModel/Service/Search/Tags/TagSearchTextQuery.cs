using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Search.Tags {

	public class TagSearchTextQuery : SearchTextQuery {

		public static new TagSearchTextQuery Empty => new TagSearchTextQuery();

		public static new TagSearchTextQuery Create(string query, 
			NameMatchMode selectedMode = NameMatchMode.Auto,
			NameMatchMode defaultMode = NameMatchMode.Words) {
			
			var parsedQuery = FindHelpers.GetMatchModeAndQueryForSearch(query, ref selectedMode, defaultMode);
			var tagNameQuery = !string.IsNullOrEmpty(parsedQuery) ? parsedQuery.Replace(' ', '_') : parsedQuery;
			return new TagSearchTextQuery(tagNameQuery, selectedMode, query);

		}

		public static TagSearchTextQuery Create(SearchTextQuery textQuery) {
			
			var tagNameQuery = !textQuery.IsEmpty ? textQuery.Query.Replace(' ', '_') : textQuery.Query;
			return new TagSearchTextQuery(tagNameQuery, textQuery.MatchMode, textQuery.OriginalQuery);

		}

		public TagSearchTextQuery() { }

		public TagSearchTextQuery(string query, NameMatchMode matchMode, string originalQuery, string[] words = null) 
			: base(query, matchMode, originalQuery, words) {}

	}

}
