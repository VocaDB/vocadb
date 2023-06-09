type Iteratee<T> = (value: T) => any;

export function groupBy<T>(
	collection: T[],
	iteratee: Iteratee<T> = (value) => value
): { [key: string]: T[] } {
	const result: { [key: string]: T[] } = {};

	for (const item of collection) {
		const key = String(iteratee(item));

		if (key in result) {
			result[key].push(item);
		} else {
			result[key] = [item];
		}
	}

	return result;
}

