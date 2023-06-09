import React from 'react';

import { IPlayerApi } from '../players';
import { ILogger, LogLevel } from '../players/ILogger';

interface NostalgicDivaContextProps extends IPlayerApi {
	logger: ILogger;
	playerApiRef: React.MutableRefObject<IPlayerApi | undefined>;
}

const NostalgicDivaContext = React.createContext<NostalgicDivaContextProps>(
	// eslint-disable-next-line @typescript-eslint/no-non-null-assertion
	undefined!,
);

interface NostalgicDivaProviderProps {
	logger?: ILogger;
	children?: React.ReactNode;
}

const defaultLogger = new (class implements ILogger {
	private readonly title = 'nostalgic-diva';

	private createMessage(message: any): string {
		return `[${this.title}] ${message}`;
	}

	private debug(message?: any, ...optionalParams: any): void {
		console.debug(this.createMessage(message), ...optionalParams);
	}

	private error(message?: any, ...optionalParams: any): void {
		console.error(this.createMessage(message), ...optionalParams);
	}

	private warn(message?: any, ...optionalParams: any): void {
		console.warn(this.createMessage(message), ...optionalParams);
	}

	isEnabled(): boolean {
		return true;
	}

	log(logLevel: LogLevel, message?: any, ...optionalParams: any[]): void {
		switch (logLevel) {
			case LogLevel.Debug:
				this.debug(message, ...optionalParams);
				break;
			case LogLevel.Warning:
				this.warn(message, ...optionalParams);
				break;
			case LogLevel.Error:
				this.error(message, ...optionalParams);
				break;
		}
	}
})();

export const NostalgicDivaProvider = ({
	logger = defaultLogger,
	children,
}: NostalgicDivaProviderProps): React.ReactElement => {
	const playerApiRef = React.useRef<IPlayerApi>();

	const loadVideo = React.useCallback(async (id: string) => {
		await playerApiRef.current?.loadVideo(id);
	}, []);

	const play = React.useCallback(async () => {
		await playerApiRef.current?.play();
	}, []);

	const pause = React.useCallback(async () => {
		await playerApiRef.current?.pause();
	}, []);

	const setCurrentTime = React.useCallback(async (seconds: number) => {
		const playerApi = playerApiRef.current;
		if (!playerApi) return;

		await playerApi.setCurrentTime(seconds);
		await playerApi.play();
	}, []);

	const setVolume = React.useCallback(async (volume: number) => {
		await playerApiRef.current?.setVolume(volume);
	}, []);

	const setMuted = React.useCallback(async (muted: boolean) => {
		await playerApiRef.current?.setMuted(muted);
	}, []);

	const getDuration = React.useCallback(async () => {
		return await playerApiRef.current?.getDuration();
	}, []);

	const getCurrentTime = React.useCallback(async () => {
		return await playerApiRef.current?.getCurrentTime();
	}, []);

	const getVolume = React.useCallback(async () => {
		return await playerApiRef.current?.getVolume();
	}, []);

	const value = React.useMemo(
		(): NostalgicDivaContextProps => ({
			logger,
			playerApiRef,
			loadVideo,
			play,
			pause,
			setCurrentTime,
			setVolume,
			setMuted,
			getDuration,
			getCurrentTime,
			getVolume,
		}),
		[
			logger,
			loadVideo,
			play,
			pause,
			setCurrentTime,
			setVolume,
			setMuted,
			getDuration,
			getCurrentTime,
			getVolume,
		],
	);

	return (
		<NostalgicDivaContext.Provider value={value}>
			{children}
		</NostalgicDivaContext.Provider>
	);
};

export const useNostalgicDiva = (): NostalgicDivaContextProps => {
	return React.useContext(NostalgicDivaContext);
};
