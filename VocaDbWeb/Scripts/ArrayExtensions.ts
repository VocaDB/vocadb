import {
	chunk,
	Dictionary,
	first,
	groupBy,
	head,
	List,
	ListIteratee,
	ListIterator,
	Many,
	NotVoid,
	orderBy,
	PartialShallow,
	PropertyName,
	sortBy,
	sum,
	take,
	unionBy,
	ValueIteratee,
} from 'lodash';

declare global {
	interface Array<T> {
		chunk(size?: number): T[][];
		first(): T | undefined;
		groupBy(iteratee?: ValueIteratee<T>): Dictionary<T[]>;
		head(): T | undefined;
		orderBy(
			iteratees?: Many<
				ListIterator<T, NotVoid> | PropertyName | PartialShallow<T>
			>,
			orders?: Many<boolean | 'asc' | 'desc'>,
		): T[];
		sortBy(...iteratees: Array<Many<ListIteratee<T>>>): T[];
		sum(): number;
		take(n?: number): T[];
		unionBy(
			arrays2: List<T> | null | undefined,
			iteratee?: ValueIteratee<T>,
		): T[];
	}
}

// eslint-disable-next-line no-extend-native
Array.prototype.chunk = function <T>(this: T[], size?: number): T[][] {
	return chunk(this, size);
};

// eslint-disable-next-line no-extend-native
Array.prototype.first = function <T>(this: T[]): T | undefined {
	return first(this);
};

// eslint-disable-next-line no-extend-native
Array.prototype.groupBy = function <T>(
	this: T[],
	iteratee?: ValueIteratee<T>,
): Dictionary<T[]> {
	return groupBy(this, iteratee);
};

// eslint-disable-next-line no-extend-native
Array.prototype.head = function <T>(this: T[]): T | undefined {
	return head(this);
};

// eslint-disable-next-line no-extend-native
Array.prototype.orderBy = function <T>(
	this: T[],
	iteratees?: Many<ListIterator<T, NotVoid> | PropertyName | PartialShallow<T>>,
	orders?: Many<boolean | 'asc' | 'desc'>,
): T[] {
	return orderBy(this, iteratees, orders);
};

// eslint-disable-next-line no-extend-native
Array.prototype.sortBy = function <T>(
	this: T[],
	...iteratees: Array<Many<ListIteratee<T>>>
): T[] {
	return sortBy(this, ...iteratees);
};

// eslint-disable-next-line no-extend-native
Array.prototype.sum = function <T>(this: T[]): number {
	return sum(this);
};

// eslint-disable-next-line no-extend-native
Array.prototype.take = function <T>(this: T[], n?: number): T[] {
	return take(this, n);
};

// eslint-disable-next-line no-extend-native
Array.prototype.unionBy = function <T>(
	this: T[],
	arrays2: List<T> | null | undefined,
	iteratee?: ValueIteratee<T>,
): T[] {
	return unionBy(this, arrays2, iteratee);
};
