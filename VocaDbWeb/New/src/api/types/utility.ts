export type Contains<Fields, Value, Result> =
	Fields extends Array<infer T>
		? T extends Value
			? Result
			: undefined
		: undefined;

export type FilterUndefined<T> = {
	[K in keyof T as T[K] extends undefined ? never : K]: T[K];
};

