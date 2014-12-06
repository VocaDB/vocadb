using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Search.Tags {

	public class TagSearchTextQuery : SearchTextQuery {

		public static new TagSearchTextQuery Create(string query, 
			NameMatchMode selectedMode = NameMatchMode.Auto,
			NameMatchMode defaultMode = NameMatchMode.Words) {
			
			query = FindHelpers.GetMatchModeAndQueryForSearch(query, ref selectedMode, defaultMode);
			var tagNameQuery = !string.IsNullOrEmpty(query) ? query.Replace(' ', '_') : query;
			return new TagSearchTextQuery(tagNameQuery, selectedMode);

		}

		public TagSearchTextQuery(string query, NameMatchMode matchMode, string[] words = null) 
			: base(query, matchMode, words) {}

	}

}
