#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Search
{
	/// <summary>
	/// Represents a textual filter for entry name.
	/// </summary>
	/// <remarks>
	/// Name filter is usually combined with <see cref="NameMatchMode"/> which can be specified by the client 
	/// or determined automatically based on the text query.
	/// This class is intended to encapsulate the relationship between those two values.
	/// </remarks>
	public class SearchTextQuery
	{
		protected string[] _words;
		private readonly NameMatchMode _matchMode;
		private readonly string _originalQuery;
		private readonly string _query;

		public static SearchTextQuery Empty => new SearchTextQuery();

		public static bool IsNullOrEmpty(SearchTextQuery query)
		{
			return query == null || query.IsEmpty;
		}

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
			NameMatchMode defaultMode = NameMatchMode.Words)
		{
			var parsedQuery = FindHelpers.GetMatchModeAndQueryForSearch(query, ref selectedMode, defaultMode);
			return new SearchTextQuery(parsedQuery, selectedMode, query);
		}

		public static IEnumerable<SearchTextQuery> Create(IEnumerable<string> names, NameMatchMode selectedMode = NameMatchMode.Auto, NameMatchMode defaultMode = NameMatchMode.Words)
		{
			return names.Select(n => Create(n, selectedMode, defaultMode));
		}

		public SearchTextQuery()
		{
			_query = string.Empty;
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
			string[] words = null)
		{
			if (!string.IsNullOrEmpty(query) && matchMode == NameMatchMode.Auto)
				throw new ArgumentException("'Auto' is not allowed here; specific name match mode is required", nameof(MatchMode));

			this._query = query;
			this._matchMode = matchMode;
			this._originalQuery = originalQuery;
			this._words = words;
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
		/// Selected name match mode. 
		/// This can never be "Auto" (checked in the constructor).
		/// </summary>
		public NameMatchMode MatchMode => _matchMode;

		/// <summary>
		/// Original query without any processing.
		/// Can be null or empty.
		/// </summary>
		public string OriginalQuery => _originalQuery;

		/// <summary>
		/// Textual filter for entry name.
		/// Can be null or empty.
		/// </summary>
		/// <remarks>
		/// Usually this field is processed by trimming any whitespace, 
		/// and by removing any SQL or query wildcards (such as asterisks and quotes).
		/// </remarks>
		public string Query => _query;

		/// <summary>
		/// List of search query words.
		/// This list will be cached.
		/// </summary>
		public string[] Words => _words ?? (_words = FindHelpers.GetQueryWords(Query));

		public override string ToString()
		{
			return $"Text filter by '{Query}' ({MatchMode})";
		}
	}
}
