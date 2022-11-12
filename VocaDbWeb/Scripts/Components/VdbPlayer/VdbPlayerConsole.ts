export class VdbPlayerConsole {
	private static title = 'VdbPlayer';

	static assert = (
		condition?: boolean | undefined,
		message?: any,
		...optionalParams: any
	): void =>
		console.assert(
			condition,
			`[${VdbPlayerConsole.title}] ${message}`,
			...optionalParams,
		);

	static debug = (message?: any, ...optionalParams: any): void =>
		console.debug(`[${VdbPlayerConsole.title}] ${message}`, ...optionalParams);

	static error = (message?: any, ...optionalParams: any): void =>
		console.error(`[${VdbPlayerConsole.title}] ${message}`, ...optionalParams);

	static warn = (message?: any, ...optionalParams: any): void =>
		console.warn(`[${VdbPlayerConsole.title}] ${message}`, ...optionalParams);
}
