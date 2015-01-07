
declare module SC {

	export var Widget: {
		(iframeElementId: string): SoundCloudWidget;
		Events: SoundCloudEvents
	};

	interface SoundCloudWidget {

		bind(eventName: string, listener: () => void);

		load(url: string, options: SoundCloudLoadOptions);

		play();

		unbind(eventName: string);

	}

	interface SoundCloudLoadOptions {

		auto_play?: boolean;

		callback?: () => void;

	}

	interface SoundCloudEvents {
		ERROR: string;
		FINISH: string;
		READY: string;
	}

}
