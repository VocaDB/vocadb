import AlbumContract from '@/DataContracts/Album/AlbumContract';
import PagingProperties from '@/DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@/DataContracts/PartialFindResultContract';
import AlbumType from '@/Models/Albums/AlbumType';
import AlbumRepository from '@/Repositories/AlbumRepository';
import ArtistRepository from '@/Repositories/ArtistRepository';
import GlobalValues from '@/Shared/GlobalValues';
import AdvancedSearchFilter from '@/Stores/Search/AdvancedSearchFilter';
import ArtistFilters from '@/Stores/Search/ArtistFilters';
import { ICommonSearchStore } from '@/Stores/Search/CommonSearchStore';
import SearchCategoryBaseStore from '@/Stores/Search/SearchCategoryBaseStore';
import { SearchType } from '@/Stores/Search/SearchStore';
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

export default class AlbumSearchStore extends SearchCategoryBaseStore<
	AlbumSearchRouteParams,
	AlbumContract
> {
	@observable public albumType = AlbumType.Unknown;
	public readonly artistFilters: ArtistFilters;
	@observable public sort = AlbumSortRule.Name;
	@observable public viewMode = 'Details' /* TODO: enum */;

	public constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		private readonly albumRepo: AlbumRepository,
		artistRepo: ArtistRepository,
	) {
		super(commonSearchStore);

		makeObservable(this);

		this.artistFilters = new ArtistFilters(values, artistRepo);
	}

	@computed public get fields(): string {
		return this.showTags
			? 'AdditionalNames,MainPicture,ReleaseEvent,Tags'
			: 'AdditionalNames,MainPicture,ReleaseEvent';
	}

	public loadResults = (
		pagingProperties: PagingProperties,
		searchTerm: string,
		tags: number[],
		childTags: boolean,
		status?: string,
	): Promise<PartialFindResultContract<AlbumContract>> => {
		const artistIds = this.artistFilters.artistIds;

		return this.albumRepo.getList({
			paging: pagingProperties,
			lang: this.values.languagePreference,
			query: searchTerm,
			sort: this.sort,
			discTypes: [this.albumType],
			tags: tags,
			childTags: childTags,
			artistIds: artistIds,
			artistParticipationStatus: this.artistFilters.artistParticipationStatus,
			childVoicebanks: this.artistFilters.childVoicebanks,
			includeMembers: this.artistFilters.includeMembers,
			fields: this.fields,
			status: status,
			deleted: false,
			advancedFilters: this.advancedFilters.filters,
		});
	};

	public ratingStars = (album: AlbumContract): { enabled: boolean }[] => {
		if (!album) return [];

		const ratings = [1, 2, 3, 4, 5].map((rating) => ({
			enabled: Math.round(album.ratingAverage) >= rating,
		}));
		return ratings;
	};

	public readonly clearResultsByQueryKeys: (keyof AlbumSearchRouteParams)[] = [
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

	@computed.struct public get routeParams(): AlbumSearchRouteParams {
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
	public set routeParams(value: AlbumSearchRouteParams) {
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
}
