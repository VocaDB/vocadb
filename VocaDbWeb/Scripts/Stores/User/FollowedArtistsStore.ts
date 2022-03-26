import ArtistForUserForApiContract from '@DataContracts/User/ArtistForUserForApiContract';
import TagRepository from '@Repositories/TagRepository';
import UserRepository from '@Repositories/UserRepository';
import GlobalValues from '@Shared/GlobalValues';
import Ajv, { JSONSchemaType } from 'ajv';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

import PartialFindResultContract from '../../DataContracts/PartialFindResultContract';
import ArtistType from '../../Models/Artists/ArtistType';
import IStoreWithPaging from '../IStoreWithPaging';
import TagFilters from '../Search/TagFilters';
import ServerSidePagingStore from '../ServerSidePagingStore';

export interface FollowedArtistsRouteParams {
	artistType?: string /* TODO: enum */;
	page?: number;
	pageSize?: number;
	tagId?: number | number[];
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<FollowedArtistsRouteParams> = require('./FollowedArtistsRouteParams.schema');
const validate = ajv.compile(schema);

export default class FollowedArtistsStore
	implements IStoreWithPaging<FollowedArtistsRouteParams> {
	@observable public artistType =
		ArtistType[ArtistType.Unknown] /* TODO: enum */;
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
		return _.map(this.tagFilters.tags, (t) => t.id);
	}
	public set tagIds(value: number[]) {
		// OPTIMIZE
		this.tagFilters.tags = [];
		this.tagFilters.addTags(value);
	}

	public pauseNotifications = false;

	@action public updateResults = (clearResults = true): void => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;
		this.loading = true;

		const pagingProperties = this.paging.getPagingProperties(clearResults);

		this.userRepo
			.getFollowedArtistsList({
				userId: this.userId,
				paging: pagingProperties,
				lang: this.values.languagePreference,
				tagIds: this.tagFilters.tagIds,
				artistType: this.artistType,
			})
			.then(
				(result: PartialFindResultContract<ArtistForUserForApiContract>) => {
					this.pauseNotifications = false;

					runInAction(() => {
						if (pagingProperties.getTotalCount)
							this.paging.totalItems = result.totalCount;

						this.page = result.items;
						this.loading = false;
					});
				},
			);
	};

	public popState = false;

	public readonly clearResultsByQueryKeys: (keyof FollowedArtistsRouteParams)[] = [
		'pageSize',
		'tagId',
		'artistType',
	];

	@computed.struct public get routeParams(): FollowedArtistsRouteParams {
		return {
			artistType: this.artistType,
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			tagId: this.tagIds,
		};
	}
	public set routeParams(value: FollowedArtistsRouteParams) {
		this.artistType = value.artistType ?? ArtistType[ArtistType.Unknown];
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 20;
		this.tagIds = ([] as number[]).concat(value.tagId ?? []);
	}

	public validateRouteParams = (
		data: any,
	): data is FollowedArtistsRouteParams => validate(data);
}
