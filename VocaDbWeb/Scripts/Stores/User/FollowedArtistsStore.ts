import { ArtistForUserForApiContract } from '@/DataContracts/User/ArtistForUserForApiContract';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { TagRepository } from '@/Repositories/TagRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { TagFilters } from '@/Stores/Search/TagFilters';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import {
	includesAny,
	RouteParamsChangeEvent,
	RouteParamsStore,
} from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export interface FollowedArtistsRouteParams {
	artistType?: ArtistType;
	page?: number;
	pageSize?: number;
	tagId?: number | number[];
}

const clearResultsByQueryKeys: (keyof FollowedArtistsRouteParams)[] = [
	'pageSize',
	'tagId',
	'artistType',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<FollowedArtistsRouteParams> = require('./FollowedArtistsRouteParams.schema');
const validate = ajv.compile(schema);

export class FollowedArtistsStore
	implements RouteParamsStore<FollowedArtistsRouteParams> {
	@observable public artistType = ArtistType.Unknown;
	@observable public loading = true; // Currently loading for data
	@observable public page: ArtistForUserForApiContract[] = []; // Current page of items
	public readonly paging = new ServerSidePagingStore(20); // Paging store
	public readonly tagFilters: TagFilters;

	public constructor(
		private readonly values: GlobalValues,
		private readonly userRepo: UserRepository,
		tagRepo: TagRepository,
		private readonly userId: number,
	) {
		makeObservable(this);

		this.tagFilters = new TagFilters(values, tagRepo);
	}

	@computed public get tagIds(): number[] {
		return this.tagFilters.tags.map((t) => t.id);
	}
	public set tagIds(value: number[]) {
		// OPTIMIZE
		this.tagFilters.tags = [];
		this.tagFilters.addTags(value);
	}

	public pauseNotifications = false;

	@action public updateResults = async (clearResults = true): Promise<void> => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;
		this.loading = true;

		const pagingProperties = this.paging.getPagingProperties(clearResults);

		const result = await this.userRepo.getFollowedArtistsList({
			userId: this.userId,
			paging: pagingProperties,
			lang: this.values.languagePreference,
			tagIds: this.tagFilters.tagIds,
			artistType: this.artistType,
		});

		this.pauseNotifications = false;

		runInAction(() => {
			if (pagingProperties.getTotalCount)
				this.paging.totalItems = result.totalCount;

			this.page = result.items;
			this.loading = false;
		});
	};

	@computed.struct public get routeParams(): FollowedArtistsRouteParams {
		return {
			artistType: this.artistType,
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			tagId: this.tagIds,
		};
	}
	public set routeParams(value: FollowedArtistsRouteParams) {
		this.artistType = value.artistType ?? ArtistType.Unknown;
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 20;
		this.tagIds = ([] as number[]).concat(value.tagId ?? []);
	}

	public validateRouteParams = (
		data: any,
	): data is FollowedArtistsRouteParams => {
		return validate(data);
	};

	public onRouteParamsChange = (
		event: RouteParamsChangeEvent<FollowedArtistsRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
