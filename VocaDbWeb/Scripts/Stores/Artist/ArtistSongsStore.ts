import { SongRepository } from '@/Repositories/SongRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { UrlMapper } from '@/Shared/UrlMapper';
import { CommonSearchStore } from '@/Stores/Search/CommonSearchStore';
import { SongSearchStore, SongSortRule } from '@/Stores/Search/SongSearchStore';
import {
	includesAny,
	StateChangeEvent,
	LocationStateStore,
} from '@vocadb/route-sphere';
import Ajv from 'ajv';

import schema from './ArtistSongsRouteParams.schema.json';

export interface ArtistSongsRouteParams {
	page?: number;
	pageSize?: number;
	sort?: SongSortRule;
	viewMode?: 'Details' | 'PlayList' /* TODO: enum */;
}

const clearResultsByQueryKeys: (keyof ArtistSongsRouteParams)[] = [
	'pageSize',

	'sort',
	'viewMode',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<ArtistSongsRouteParams>(schema);

export class ArtistSongsStore
	extends SongSearchStore
	implements LocationStateStore<ArtistSongsRouteParams> {
	constructor(
		values: GlobalValues,
		urlMapper: UrlMapper,
		songRepo: SongRepository,
		userRepo: UserRepository,
	) {
		super(
			new CommonSearchStore(values, undefined!),
			values,
			urlMapper,
			songRepo,
			userRepo,
			undefined!,
			undefined!,
		);
	}

	get locationState(): ArtistSongsRouteParams {
		return {
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			sort: this.sort,
			viewMode: this.viewMode,
		};
	}
	set locationState(value: ArtistSongsRouteParams) {
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 10;
		this.sort = value.sort ?? SongSortRule.RatingScore;
		this.viewMode = value.viewMode ?? 'Details';
	}

	validateLocationState = (data: any): data is ArtistSongsRouteParams => {
		return validate(data);
	};

	onLocationStateChange = (
		event: StateChangeEvent<ArtistSongsRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
