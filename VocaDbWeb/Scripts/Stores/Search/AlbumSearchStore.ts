import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { AlbumType } from '@/Models/Albums/AlbumType';
import {
	AlbumOptionalField,
	AlbumRepository,
} from '@/Repositories/AlbumRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { AdvancedSearchFilter } from '@/Stores/Search/AdvancedSearchFilter';
import { ArtistFilters } from '@/Stores/Search/ArtistFilters';
import { ICommonSearchStore } from '@/Stores/Search/CommonSearchStore';
import { SearchCategoryBaseStore } from '@/Stores/Search/SearchCategoryBaseStore';
import { SearchType } from '@/Stores/Search/SearchStore';
import { includesAny, StateChangeEvent } from '@/route-sphere';
import { computed, makeObservable, observable } from 'mobx';

// Corresponds to the AlbumSortRule enum in C#.
export enum AlbumSortRule {
	None = 'None',
	Name = 'Name',
	ReleaseDate = 'ReleaseDate',
	ReleaseDateWithNulls = 'ReleaseDateWithNulls',
	AdditionDate = 'AdditionDate',
	RatingAverage = 'RatingAverage',
	RatingTotal = 'RatingTotal',
	NameThenReleaseDate = 'NameThenReleaseDate',
	CollectionCount = 'CollectionCount',
}

export interface AlbumSearchRouteParams {
	advancedFilters?: AdvancedSearchFilter[];
	artistId?: number | number[];
	artistParticipationStatus?: string /* TODO: enum */;
	childTags?: boolean;
	childVoicebanks?: boolean;
	discType?: AlbumType;
	draftsOnly?: boolean;
	filter?: string;
	includeMembers?: boolean;
	page?: number;
	pageSize?: number;
	searchType?: SearchType.Album;
	sort?: AlbumSortRule;
	tag?: string;
	tagId?: number | number[];
	viewMode?: string /* TODO: enum */;
}

const clearResultsByQueryKeys: (keyof AlbumSearchRouteParams)[] = [
	'pageSize',
	'filter',
	'tagId',
	'childTags',
	'draftsOnly',
	'searchType',

	'advancedFilters',
	'sort',
	'discType',
	'artistId',
	'artistParticipationStatus',
	'childVoicebanks',
	'includeMembers',
];

export class AlbumSearchStore extends SearchCategoryBaseStore<
	AlbumSearchRouteParams,
	AlbumContract
> {
	@observable albumType = AlbumType.Unknown;
	readonly artistFilters: ArtistFilters;
	@observable sort = AlbumSortRule.Name;
	@observable viewMode = 'Details' /* TODO: enum */;

	constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		private readonly albumRepo: AlbumRepository,
		artistRepo: ArtistRepository,
	) {
		super(commonSearchStore);

		makeObservable(this);

		this.artistFilters = new ArtistFilters(values, artistRepo);
	}

	@computed get fields(): AlbumOptionalField[] {
		return this.showTags
			? [
					AlbumOptionalField.AdditionalNames,
					AlbumOptionalField.MainPicture,
					AlbumOptionalField.ReleaseEvent,
					AlbumOptionalField.Tags,
			  ]
			: [
					AlbumOptionalField.AdditionalNames,
					AlbumOptionalField.MainPicture,
					AlbumOptionalField.ReleaseEvent,
			  ];
	}

	loadResults = (
		pagingProperties: PagingProperties,
	): Promise<PartialFindResultContract<AlbumContract>> => {
		return this.albumRepo.getList({
			paging: pagingProperties,
			lang: this.values.languagePreference,
			query: this.searchTerm,
			sort: this.sort,
			discTypes: [this.albumType],
			tags: this.tagIds,
			childTags: this.childTags,
			artistIds: this.artistFilters.artistIds,
			artistParticipationStatus: this.artistFilters.artistParticipationStatus,
			childVoicebanks: this.artistFilters.childVoicebanks,
			includeMembers: this.artistFilters.includeMembers,
			fields: this.fields,
			status: this.draftsOnly ? 'Draft' : undefined,
			deleted: false,
			advancedFilters: this.advancedFilters.filters,
		});
	};

	ratingStars = (album: AlbumContract): { enabled: boolean }[] => {
		if (!album) return [];

		const ratings = [1, 2, 3, 4, 5].map((rating) => ({
			enabled: Math.round(album.ratingAverage) >= rating,
		}));
		return ratings;
	};

	@computed.struct get locationState(): AlbumSearchRouteParams {
		return {
			searchType: SearchType.Album,
			advancedFilters: this.advancedFilters.filters.map((filter) => ({
				description: filter.description,
				filterType: filter.filterType,
				negate: filter.negate,
				param: filter.param,
			})),
			artistId: this.artistFilters.artistIds,
			artistParticipationStatus: this.artistFilters.artistParticipationStatus,
			childTags: this.childTags,
			childVoicebanks: this.artistFilters.childVoicebanks,
			discType: this.albumType,
			draftsOnly: this.draftsOnly,
			filter: this.searchTerm,
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			sort: this.sort,
			tagId: this.tagIds,
			viewMode: this.viewMode,
		};
	}
	set locationState(value: AlbumSearchRouteParams) {
		this.advancedFilters.filters = value.advancedFilters ?? [];
		this.artistFilters.artistIds = ([] as number[]).concat(
			value.artistId ?? [],
		);
		this.artistFilters.artistParticipationStatus =
			value.artistParticipationStatus ?? 'Everything';
		this.childTags = value.childTags ?? false;
		this.artistFilters.childVoicebanks = value.childVoicebanks ?? false;
		this.albumType = value.discType ?? AlbumType.Unknown;
		this.draftsOnly = value.draftsOnly ?? false;
		this.searchTerm = value.filter ?? '';
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 10;
		this.sort = value.sort ?? AlbumSortRule.Name;
		this.tagIds = ([] as number[]).concat(value.tagId ?? []);
		this.viewMode = value.viewMode ?? 'Details';
	}

	onLocationStateChange = (
		event: StateChangeEvent<AlbumSearchRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
