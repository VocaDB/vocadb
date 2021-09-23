import ArtistContract from '@DataContracts/Artist/ArtistContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import ArtistHelper from '@Helpers/ArtistHelper';
import ArtistType from '@Models/Artists/ArtistType';
import ArtistRepository from '@Repositories/ArtistRepository';
import GlobalValues from '@Shared/GlobalValues';
import { computed, makeObservable, observable } from 'mobx';

import AdvancedSearchFilter from './AdvancedSearchFilter';
import { ICommonSearchStore } from './CommonSearchStore';
import SearchCategoryBaseStore from './SearchCategoryBaseStore';
import { SearchType } from './SearchStore';

// Corresponds to the ArtistSortRule enum in C#.
export enum ArtistSortRule {
	None = 'None',
	Name = 'Name',
	AdditionDate = 'AdditionDate',
	AdditionDateAsc = 'AdditionDateAsc',
	ReleaseDate = 'ReleaseDate',
	SongCount = 'SongCount',
	SongRating = 'SongRating',
	FollowerCount = 'FollowerCount',
}

export interface ArtistSearchRouteParams {
	advancedFilters?: AdvancedSearchFilter[];
	artistType?: string /* TODO: enum */;
	childTags?: boolean;
	draftsOnly?: boolean;
	filter?: string;
	onlyFollowedByMe?: boolean;
	page?: number;
	pageSize?: number;
	searchType?: SearchType.Artist;
	sort?: ArtistSortRule;
	tag?: string;
	tagId?: number | number[];
}

export default class ArtistSearchStore extends SearchCategoryBaseStore<ArtistContract> {
	@observable public artistType = 'Unknown' /* TODO: enum */;
	@observable public onlyFollowedByMe = false;
	@observable public onlyRootVoicebanks = false;
	@observable public sort = ArtistSortRule.Name;

	public constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		private readonly artistRepo: ArtistRepository,
	) {
		super(commonSearchStore);

		makeObservable(this);
	}

	@computed public get fields(): string {
		return this.showTags
			? 'AdditionalNames,MainPicture,Tags'
			: 'AdditionalNames,MainPicture';
	}

	public loadResults = (
		pagingProperties: PagingProperties,
		searchTerm: string,
		tags: number[],
		childTags: boolean,
		status?: string,
	): Promise<PartialFindResultContract<ArtistContract>> => {
		return this.artistRepo.getList({
			paging: pagingProperties,
			lang: this.values.languagePreference,
			query: searchTerm,
			sort: this.sort,
			artistTypes:
				this.artistType !== ArtistType[ArtistType.Unknown]
					? this.artistType
					: undefined,
			allowBaseVoicebanks: !this.onlyRootVoicebanks,
			tags: tags,
			childTags: childTags,
			followedByUserId: this.onlyFollowedByMe
				? this.values.loggedUserId
				: undefined,
			fields: this.fields,
			status: status,
			advancedFilters: this.advancedFilters.filters,
		});
	};

	@computed public get canHaveChildVoicebanks(): boolean {
		return ArtistHelper.canHaveChildVoicebanks(
			ArtistType[this.artistType as keyof typeof ArtistType],
		);
	}

	public readonly clearResultsByQueryKeys: (keyof ArtistSearchRouteParams)[] = [
		'pageSize',
		'filter',
		'tagId',
		'childTags',
		'draftsOnly',
		'searchType',

		'advancedFilters',
		'sort',
		'artistType',
		'onlyFollowedByMe',
		// TODO: onlyRootVoicebanks
	];

	@computed.struct public get routeParams(): ArtistSearchRouteParams {
		return {
			searchType: SearchType.Artist,
			advancedFilters: this.advancedFilters.filters.map((filter) => ({
				description: filter.description,
				filterType: filter.filterType,
				negate: filter.negate,
				param: filter.param,
			})),
			artistType: this.artistType,
			childTags: this.childTags,
			draftsOnly: this.draftsOnly,
			filter: this.searchTerm,
			onlyFollowedByMe: this.onlyFollowedByMe,
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			sort: this.sort,
			tagId: this.tagIds,
		};
	}
	public set routeParams(value: ArtistSearchRouteParams) {
		this.advancedFilters.filters = value.advancedFilters ?? [];
		this.artistType = value.artistType ?? 'Unknown';
		this.childTags = value.childTags ?? false;
		this.draftsOnly = value.draftsOnly ?? false;
		this.searchTerm = value.filter ?? '';
		this.onlyFollowedByMe = value.onlyFollowedByMe ?? false;
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 10;
		this.sort = value.sort ?? ArtistSortRule.Name;
		this.tagIds = value.tagId ? ([] as number[]).concat(value.tagId) : [];
	}
}
