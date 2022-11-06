import {
	Dictionary,
	ListIteratee,
	ListIterator,
	Many,
	NotVoid,
	PartialShallow,
	PropertyName,
	ValueIteratee,
} from 'lodash';
import {
	chunk,
	first,
	groupBy,
	head,
	last,
	orderBy,
	sortBy,
	sum,
	sumBy,
	take,
} from 'lodash-es';

declare global {
	interface Array<T> {
		chunk(size?: number): T[][];
		first(): T | undefined;
		groupBy(iteratee?: ValueIteratee<T>): Dictionary<T[]>;
		head(): T | undefined;
		last(): T | undefined;
		orderBy(
			iteratees?: Many<
				ListIterator<T, NotVoid> | PropertyName | PartialShallow<T>
			>,
			orders?: Many<boolean | 'asc' | 'desc'>,
		): T[];
		sortBy(...iteratees: Array<Many<ListIteratee<T>>>): T[];
		sum(): number;
		sumBy(iteratee?: ((value: T) => number) | string): number;
		take(n?: number): T[];
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
Array.prototype.last = function <T>(this: T[]): T | undefined {
	return last(this);
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
Array.prototype.sumBy = function <T>(
	this: T[],
	iteratee?: ((value: T) => number) | string,
): number {
	return sumBy(this, iteratee);
};

// eslint-disable-next-line no-extend-native
Array.prototype.take = function <T>(this: T[], n?: number): T[] {
	return take(this, n);
};
