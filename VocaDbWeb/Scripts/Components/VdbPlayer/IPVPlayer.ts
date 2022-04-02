import PVContract from '@DataContracts/PVs/PVContract';

export interface IPVPlayerOptions {
	onError?: (e: any) => void;
	onPlay?: () => void;
	onPause?: () => void;
	onEnded?: () => void;
}

export default interface IPVPlayer {
	load: (pv: PVContract) => Promise<void>;
	play: () => void;
	pause: () => void;
	seekTo: (seconds: number) => void;
	mute: () => void;
	unmute: () => void;
}
