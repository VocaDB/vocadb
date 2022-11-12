export class SearchTextQueryHelper {
	// Test whether a query text contains a wildcard - either quotes for exact match or asterisk for prefix.
	static isWildcardQuery = (queryText: string): boolean => {
		if (!queryText || queryText.length < 2) return false;

		return (
			queryText.charAt(queryText.length - 1) === '*' ||
			(queryText.charAt(0) === '"' &&
				queryText.charAt(queryText.length - 1) === '"')
		);
	};
}
