
	enum NameMatchMode {
		
		// Automatically choose match mode based on query term length.
		Auto,

		// Always partial matching.
		// Wildcards are allowed.
		Partial,

		// Starts with.
		StartsWith,

		// Always exact matching (usually still case-insensitive).
		// Wildcards are not allowed.
		Exact,

		// Allow breaking the search string into words separated by whitespace.
		Words

	}

	export default NameMatchMode;