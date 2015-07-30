using System;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Search {

	/// <summary>
	/// Represents a textual filter for entry name.
	/// </summary>
	/// <remarks>
	/// Name filter is usually combined with <see cref="NameMatchMode"/> which can be specified by the client 
	/// or determined automatically based on the text query.
	/// This class is intended to encapsulate the relationship between those two values.
	/// </remarks>
	public class SearchTextQuery {

		protected string[] words;
		private readonly NameMatchMode matchMode;
		private readonly string originalQuery;
		private readonly string query;

		public static SearchTextQuery Empty => new SearchTextQuery();

		/// <summary>
		/// Creates search text query.
		/// Determines the actual name match mode.
		/// Parses and caches the words list for words query.
		/// If using the 'Auto' name match mode, the query will also be trimmed and cleaned for wildcards (SQL wildcards, asterisks and quotes).
		/// </summary>
		/// <param name="query">Text query. Can include wildcards. Can be null or empty.</param>
		/// <param name="selectedMode">Selected name match mode. If 'Auto', the name match mode will be selected automatically.</param>
		/// <param name="defaultMode">Default name match mode to be used for normal queries, if no special rules apply and no name match mode is specified.</param>
		/// <returns>Search text query. Cannot be null.</returns>
		public static SearchTextQuery Create(string query, 
			NameMatchMode selectedMode = NameMatchMode.Auto, 
			NameMatchMode defaultMode = NameMatchMode.Words) {
			
			var parsedQuery = FindHelpers.GetMatchModeAndQueryForSearch(query, ref selectedMode, defaultMode);
			return new SearchTextQuery(parsedQuery, selectedMode, query);

		}

		public SearchTextQuery() {
			query = string.Empty;
		}

		/// <summary>
		/// Initializes a new search text query.
		/// In most cases the <see cref="Create"/> factory method should be used.
		/// </summary>
		/// <param name="query">Text query. Can be null or empty.</param>
		/// <param name="matchMode">Name match mode. Cannot be 'Auto'. Use the factory method to determine the match mode.</param>
		/// <param name="originalQuery">Original query without any processing. Can be null or empty.</param>
		/// <param name="words">
		/// List of query words, if any. Can be null, in which case the words list will be parsed from <paramref name="query"/>.
		/// </param>
		public SearchTextQuery(string query, NameMatchMode matchMode, 
			string originalQuery,
			string[] words = null) {

			if (!string.IsNullOrEmpty(query) && matchMode == NameMatchMode.Auto)
				throw new ArgumentException("'Auto' is not allowed here; specific name match mode is required", "matchMode");

			this.query = query;
			this.matchMode = matchMode;
			this.originalQuery = originalQuery;
			this.words = words;

		}

		/// <summary>
		/// Whether no name filter is specified.
		/// Usually this means no filtering is done.
		/// </summary>
		public bool IsEmpty => string.IsNullOrEmpty(Query);

		/// <summary>
		/// Whether the query represents exact match.
		/// </summary>
		public bool IsExact => MatchMode == NameMatchMode.Exact;

		/// <summary>
		/// Selected name match mode. This cannot be Auto.
		/// </summary>
		public NameMatchMode MatchMode => matchMode;

		/// <summary>
		/// Original query without any processing.
		/// Can be null or empty.
		/// </summary>
		public string OriginalQuery => originalQuery;

		/// <summary>
		/// Textual filter for entry name.
		/// Can be null or empty.
		/// </summary>
		/// <remarks>
		/// Usually this field is processed by trimming any whitespace, 
		/// and by removing any SQL or query wildcards (such as asterisks and quotes).
		/// </remarks>
		public string Query => query;

		/// <summary>
		/// List of search query words.
		/// This list will be cached.
		/// </summary>
		public string[] Words => words ?? (words = FindHelpers.GetQueryWords(Query));

		public override string ToString() {
			return string.Format("Text filter by '{0}' ({1})", Query, MatchMode);
		}

	}

}
