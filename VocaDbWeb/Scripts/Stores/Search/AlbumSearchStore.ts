import AlbumContract from '@DataContracts/Album/AlbumContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import AlbumRepository from '@Repositories/AlbumRepository';
import ArtistRepository from '@Repositories/ArtistRepository';
import GlobalValues from '@Shared/GlobalValues';
import _ from 'lodash';
import { computed, makeObservable, observable, reaction } from 'mobx';

import AdvancedSearchFilter from './AdvancedSearchFilter';
import ArtistFilters from './ArtistFilters';
import { ICommonSearchStore } from './CommonSearchStore';
import SearchCategoryBaseStore from './SearchCategoryBaseStore';
import { SearchRouteParams, SearchType } from './SearchStore';

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
	artistId?: number[];
	artistParticipationStatus?: string /* TODO: enum */;
	childTags?: boolean;
	childVoicebanks?: boolean;
	discType?: string /* TODO: enum */;
	draftsOnly?: boolean;
	filter?: string;
	includeMembers?: boolean;
	page?: number;
	pageSize?: number;
	searchType?: SearchType.Album;
	sort?: AlbumSortRule;
	tag?: string;
	tagId?: number[];
	viewMode?: string /* TODO: enum */;
}

export default class AlbumSearchStore extends SearchCategoryBaseStore<AlbumContract> {
	@observable public albumType = 'Unknown' /* TODO: enum */;
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

		reaction(
			() => this.advancedFilters.filters.map((filter) => filter.description),
			this.updateResultsWithTotalCount,
		);
		this.artistFilters = new ArtistFilters(values, artistRepo);

		reaction(() => this.sort, this.updateResultsWithTotalCount);
		reaction(() => this.albumType, this.updateResultsWithTotalCount);
		reaction(
			() => this.artistFilters.filters,
			this.updateResultsWithTotalCount,
		);
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
			discTypes: this.albumType,
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

		const ratings = _.map([1, 2, 3, 4, 5], (rating) => {
			return {
				enabled: Math.round(album.ratingAverage) >= rating,
			};
		});
		return ratings;
	};

	@computed.struct public get routeParams(): SearchRouteParams {
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
			pageSize: this.pageSize,
			sort: this.sort,
			tagId: this.tagIds,
			viewMode: this.viewMode,
		};
	}
	public set routeParams(value: SearchRouteParams) {
		if (value.searchType !== SearchType.Album) return;

		this.advancedFilters.filters = value.advancedFilters ?? [];
		this.artistFilters.artistIds = value.artistId ?? [];
		this.artistFilters.artistParticipationStatus =
			value.artistParticipationStatus ?? 'Everything';
		this.childTags = value.childTags ?? false;
		this.artistFilters.childVoicebanks = value.childVoicebanks ?? false;
		this.albumType = value.discType ?? 'Unknown';
		this.draftsOnly = value.draftsOnly ?? false;
		this.searchTerm = value.filter ?? '';
		this.paging.page = value.page ?? 1;
		this.pageSize = value.pageSize ?? 10;
		this.sort = value.sort ?? AlbumSortRule.Name;
		this.tagIds = value.tagId ?? [];
		this.viewMode = value.viewMode ?? 'Details';
	}
}
