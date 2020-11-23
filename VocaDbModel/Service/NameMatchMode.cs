namespace VocaDb.Model.Service
{
	/// <summary>
	/// Match mode for name queries.
	/// </summary>
	public enum NameMatchMode
	{
		/// <summary>
		/// Automatically choose match mode based on query term length.
		/// For longer queries this is generally Words, for shorter StartsWith.
		/// </summary>
		Auto,

		/// <summary>
		/// Partial matching (match anywhere in the name).
		/// Wildcards are allowed.
		/// </summary>
		Partial,

		/// <summary>
		/// Prefix search.
		/// </summary>
		StartsWith,

		/// <summary>
		/// Exact (strict) match (usually still case-insensitive).
		/// Wildcards are not allowed.
		/// </summary>
		Exact,

		/// <summary>
		/// Allow breaking the search string into words separated by whitespace.
		/// The order of words does not matter.
		/// </summary>
		Words
	}
}