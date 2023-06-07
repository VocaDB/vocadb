import type { LyricsForSongContract } from '@/DataContracts/Song/LyricsForSongContract';
import { SongRepository } from '@/Repositories/SongRepository';
import {
	includesAny,
	StateChangeEvent,
	LocationStateStore,
} from '@/route-sphere';
import Ajv from 'ajv';
import { computed, makeObservable, observable, runInAction } from 'mobx';

import schema from './SongLyricsRouteParams.schema.json';

interface SongLyricsRouteParams {
	albumId?: number;
	lyricsId?: number;
}

const clearResultsByQueryKeys: (keyof SongLyricsRouteParams)[] = [];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<SongLyricsRouteParams>(schema);

export class SongLyricsStore
	implements LocationStateStore<SongLyricsRouteParams> {
	@observable albumId?: number;
	@observable selectedLyrics?: LyricsForSongContract;
	@observable selectedLyricsId: number;

	constructor(
		private readonly songRepo: SongRepository,
		private readonly lyricsId: number,
		private readonly songVersion: number,
	) {
		makeObservable(this);

		this.selectedLyricsId = lyricsId;
	}

	@computed.struct get locationState(): SongLyricsRouteParams {
		return {
			albumId: this.albumId,
			lyricsId: this.selectedLyricsId,
		};
	}
	set locationState(value: SongLyricsRouteParams) {
		this.albumId = value.albumId;
		this.selectedLyricsId = value.lyricsId || this.lyricsId;
	}

	validateLocationState = (data: any): data is SongLyricsRouteParams => {
		return validate(data);
	};

	private pauseNotifications = false;

	updateResults = async (clearResults: boolean): Promise<void> => {
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		const lyrics = await this.songRepo.getLyrics({
			lyricsId: this.selectedLyricsId,
			songVersion: this.songVersion,
		});

		this.pauseNotifications = false;

		runInAction(() => {
			this.selectedLyrics = lyrics;
		});
	};

	onLocationStateChange = (
		event: StateChangeEvent<SongLyricsRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		this.updateResults(clearResults);
	};
}
