export default class VdbPlayerConsole {
	private static title = 'VdbPlayer';

	public static assert = (
		condition?: boolean | undefined,
		message?: any,
		...optionalParams: any
	): void =>
		console.assert(
			condition,
			`[${VdbPlayerConsole.title}] ${message}`,
			...optionalParams,
		);

	public static debug = (message?: any, ...optionalParams: any): void =>
		console.debug(`[${VdbPlayerConsole.title}] ${message}`, ...optionalParams);

	public static error = (message?: any, ...optionalParams: any): void =>
		console.error(`[${VdbPlayerConsole.title}] ${message}`, ...optionalParams);

	public static warn = (message?: any, ...optionalParams: any): void =>
		console.warn(`[${VdbPlayerConsole.title}] ${message}`, ...optionalParams);
}
