import LyricsForSongContract from '@DataContracts/Song/LyricsForSongContract';
import SongRepository from '@Repositories/SongRepository';
import Ajv, { JSONSchemaType } from 'ajv';
import { computed, makeObservable, observable, runInAction } from 'mobx';

import IStoreWithUpdateResults from '../IStoreWithUpdateResults';

interface SongLyricsRouteParams {
	lyricsId?: number;
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<SongLyricsRouteParams> = require('./SongLyricsRouteParams.schema');
const validate = ajv.compile(schema);

export default class SongLyricsStore
	implements IStoreWithUpdateResults<SongLyricsRouteParams> {
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
			lyricsId: this.selectedLyricsId,
		};
	}
	public set routeParams(value: SongLyricsRouteParams) {
		this.selectedLyricsId = value.lyricsId || this.lyricsId;
	}

	public validateRouteParams = (data: any): data is SongLyricsRouteParams =>
		validate(data);

	public clearResultsByQueryKeys: (keyof SongLyricsRouteParams)[] = [];

	private pauseNotifications = false;

	public updateResults = (clearResults: boolean): void => {
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		this.songRepo
			.getLyrics({
				lyricsId: this.selectedLyricsId,
				songVersion: this.songVersion,
			})
			.then((lyrics) => {
				this.pauseNotifications = false;

				runInAction(() => {
					this.selectedLyrics = lyrics;
				});
			});
	};
}
