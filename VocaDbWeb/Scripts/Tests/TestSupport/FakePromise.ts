export default class FakePromise<T> {
	public constructor(private readonly value: T) {}

	public then = (onfulfilled: (value: T) => T): T => {
		return onfulfilled(this.value);
	};

	public static resolve = <T>(value: T): Promise<T> =>
		(new FakePromise<T>(value) as unknown) as Promise<T>;
}
