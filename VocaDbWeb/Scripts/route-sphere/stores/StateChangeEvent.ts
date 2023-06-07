export const includesAny = <T>(array: T[], values: T[]): boolean => {
	return values.some((value) => array.includes(value));
};

export interface StateChangeEvent<TState> {
	keys: (keyof TState)[];
	popState: boolean;
}
