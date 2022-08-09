import { LyricsForSongContract } from '@/DataContracts/Song/LyricsForSongContract';
import { SongRepository } from '@/Repositories/SongRepository';
import { StoreWithUpdateResults } from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import { computed, makeObservable, observable, runInAction } from 'mobx';

interface SongLyricsRouteParams {
	albumId?: number;
	lyricsId?: number;
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<SongLyricsRouteParams> = require('./SongLyricsRouteParams.schema');
const validate = ajv.compile(schema);

export class SongLyricsStore
	implements StoreWithUpdateResults<SongLyricsRouteParams> {
	@observable public albumId?: number;
	@observable public selectedLyrics?: LyricsForSongContract;
	@observable public selectedLyricsId: number;

	public constructor(
		private readonly songRepo: SongRepository,
		private readonly lyricsId: number,
		private readonly songVersion: number,
	) {
		makeObservable(this);

		this.selectedLyricsId = lyricsId;
	}

	public popState = false;

	@computed.struct public get routeParams(): SongLyricsRouteParams {
		return {
			albumId: this.albumId,
			lyricsId: this.selectedLyricsId,
		};
	}
	public set routeParams(value: SongLyricsRouteParams) {
		this.albumId = value.albumId;
		this.selectedLyricsId = value.lyricsId || this.lyricsId;
	}

	public validateRouteParams = (data: any): data is SongLyricsRouteParams => {
		return validate(data);
	};

	public clearResultsByQueryKeys: (keyof SongLyricsRouteParams)[] = [];

	private pauseNotifications = false;

	public updateResults = async (clearResults: boolean): Promise<void> => {
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
}
