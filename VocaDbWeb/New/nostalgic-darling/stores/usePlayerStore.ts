import { SongContract } from '@/types/DataContracts/Song/SongContract';
import React from 'react';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

export interface IPlayerApi {
	loadVideo(id: string): void;
	play(): void;
	pause(): void;
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
	playerApi?: React.MutableRefObject<IPlayerApi | undefined>;
	active: boolean;
	playerBounds: Rectangle | undefined;
	// TODO: Remoev this and convert to queue ops
	loadSong(song: SongContract): void;
	setPlayerBounds: (bounds: Rectangle | undefined) => void;
	setActive: (active: boolean) => void;
	setPlayerApi: (api: React.MutableRefObject<IPlayerApi | undefined> | undefined) => void;
	unload(): void;
	// playPause(): void;
	// setCurrentTime(): Promise<void>;
	// setMuted(): Promise<void>;
	// getDuration(): Promise<void>;
	// getCurrentTime(): Promise<void>;
	// getVolume(): Promise<void>;
}

const DEFAULT_BOUNDS = { x: 0, y: 0, width: 200, height: 200 };

export const usePlayerStore = create<PlayerState>()(
	persist(
		(set, get) => ({
			queue: [],
			active: false,
			playerBounds: undefined,
			setPlayerBounds(newBounds) {
				set({ playerBounds: newBounds });
			},
			loadSong(song) {
				set({ song });
			},
			unload() {
				set({ active: false, song: undefined });
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
		}
	)
);

