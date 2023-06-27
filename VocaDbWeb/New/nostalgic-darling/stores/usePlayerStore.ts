import { SongContract } from '@/types/DataContracts/Song/SongContract';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

export interface IPlayerApi {
	play(): void;
	pause(): void;
	getCurrentTime(): number;
	getDuration(): number;
	setCurrentTime(newProgress: number): void;
	// setCurrentTime(seconds: number): Promise<void>;
	// setVolume(volume: number): Promise<void>;
	// setMuted(muted: boolean): Promise<void>;
	// getDuration(): Promise<number | undefined>;
	// getCurrentTime(): Promise<number | undefined>;
	// getVolume(): Promise<number | undefined>;
}

type Rectangle = { x: number; y: number; width: number; height: number };

export interface PlayerState {
	song?: SongContract;
	queue: SongContract[];
	playerApi?: IPlayerApi | undefined;
	active: boolean;
	playerBounds: Rectangle | undefined;
	// TODO: Remoev this and convert to queue ops
	loadSong(song: SongContract): void;
	setPlayerBounds: (bounds: Rectangle | undefined) => void;
	setActive: (active: boolean) => void;
	setPlayerApi: (api: IPlayerApi | undefined) => void;
	onEnd(): void;
	// playPause(): void;
	// setCurrentTime(): Promise<void>;
	// setMuted(): Promise<void>;
	// getDuration(): Promise<void>;
	// getCurrentTime(): Promise<void>;
	// getVolume(): Promise<void>;
}

export const usePlayerStore = create<PlayerState>()(
	persist(
		(set, get) => ({
			queue: [],
			active: false,
			playerBounds: undefined,
			currentTime: undefined,
			setPlayerBounds(newBounds) {
				set({ playerBounds: newBounds });
			},
			loadSong(song) {
				set({ song, active: false });
			},
			onEnd() {
				set({
					active: false,
					song: undefined,
					playerApi: undefined,
					playerBounds: undefined,
				});
			},
			setActive: (active) => set({ active }),
			setPlayerApi: (api) => set({ playerApi: api }),
			// playPause() {
			// 	if (get().active) {
			// 		get().playerApi?.current?.pause();
			// 		set({ active: false });
			// 	} else {
			// 		get().playerApi?.current?.play();
			// 		set({ active: true });
			// 	}
			// },
		}),
		{
			name: 'player-storage',
			partialize: (state) => ({ song: state.song, queue: state.queue }),
		}
	)
);

