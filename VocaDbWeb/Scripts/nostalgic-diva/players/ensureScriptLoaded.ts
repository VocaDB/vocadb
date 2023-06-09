import { ILogger, LogLevel } from './ILogger';
import { getScript } from './getScript';

const urls: string[] = [];

export async function ensureScriptLoaded(
	url: string,
	logger: ILogger,
): Promise<boolean> {
	if (urls.includes(url)) {
		logger.log(LogLevel.Debug, url, 'script is already loaded');
		return false;
	}

	try {
		logger.log(LogLevel.Debug, url, 'Loading script...');

		await getScript(url);

		if (urls.includes(url)) {
			logger.log(LogLevel.Debug, url, 'script is already loaded');
			return false;
		} else {
			urls.push(url);
			logger.log(LogLevel.Debug, url, 'script loaded');
			return true;
		}
	} catch (error) {
		logger.log(LogLevel.Error, url, 'Failed to load script');
		throw error;
	}
}
