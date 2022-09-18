import { SongRepository } from '@/Repositories/SongRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { UrlMapper } from '@/Shared/UrlMapper';
import { PVPlayersFactory } from '@/Stores/PVs/PVPlayersFactory';
import { CommonSearchStore } from '@/Stores/Search/CommonSearchStore';
import { SongSearchStore, SongSortRule } from '@/Stores/Search/SongSearchStore';
import {
	includesAny,
	RouteParamsChangeEvent,
	StoreWithUpdateResults,
} from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';

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
const schema: JSONSchemaType<ArtistSongsRouteParams> = require('./ArtistSongsRouteParams.schema');
const validate = ajv.compile(schema);

export class ArtistSongsStore
	extends SongSearchStore
	implements StoreWithUpdateResults<ArtistSongsRouteParams> {
	public constructor(
		values: GlobalValues,
		urlMapper: UrlMapper,
		songRepo: SongRepository,
		userRepo: UserRepository,
		pvPlayersFactory: PVPlayersFactory,
	) {
		super(
			new CommonSearchStore(values, undefined!),
			values,
			urlMapper,
			songRepo,
			userRepo,
			undefined!,
			undefined!,
			pvPlayersFactory,
		);
	}

	public get routeParams(): ArtistSongsRouteParams {
		return {
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			sort: this.sort,
			viewMode: this.viewMode,
		};
	}
	public set routeParams(value: ArtistSongsRouteParams) {
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 10;
		this.sort = value.sort ?? SongSortRule.RatingScore;
		this.viewMode = value.viewMode ?? 'Details';
	}

	public validateRouteParams = (data: any): data is ArtistSongsRouteParams => {
		return validate(data);
	};

	public onRouteParamsChange = (
		event: RouteParamsChangeEvent<ArtistSongsRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
