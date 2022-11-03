export class FakePromise<T> {
	constructor(private readonly value: T) {}

	then = (onfulfilled: (value: T) => T): T => {
		return onfulfilled(this.value);
	};

	static resolve = <T>(value: T): Promise<T> =>
		(new FakePromise<T>(value) as unknown) as Promise<T>;
}
