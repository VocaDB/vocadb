declare module SC {
	export var Widget: {
		(iframeElementId: string | HTMLIFrameElement): SoundCloudWidget;
		Events: SoundCloudEvents;
	};

	interface SoundCloudWidget {
		bind(eventName: string, listener: (e: any) => void);

		load(url: string, options: SoundCloudLoadOptions);

		pause();

		play();

		seekTo(milliseconds: number);

		unbind(eventName: string);
	}

	interface SoundCloudLoadOptions {
		auto_play?: boolean;

		callback?: () => void;
	}

	interface SoundCloudEvents {
		ERROR: string;
		FINISH: string;
		PAUSE: string;
		PLAY: string;
		READY: string;
	}
}
