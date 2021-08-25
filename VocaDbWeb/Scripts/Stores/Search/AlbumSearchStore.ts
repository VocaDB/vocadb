import AlbumContract from '@DataContracts/Album/AlbumContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import AlbumRepository from '@Repositories/AlbumRepository';
import ArtistRepository from '@Repositories/ArtistRepository';
import GlobalValues from '@Shared/GlobalValues';
import _ from 'lodash';
import { computed, makeObservable, observable } from 'mobx';

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
	artistId?: number[];
	childTags?: boolean;
	childVoicebanks?: boolean;
	discType?: string /* TODO: enum */;
	draftsOnly?: boolean;
	filter?: string;
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

	@computed public get routeParams(): SearchRouteParams {
		return {
			searchType: SearchType.Album,
			artistId: this.artistFilters.artistIds,
			childTags: this.childTags || undefined,
			childVoicebanks: this.artistFilters.childVoicebanks || undefined,
			discType: this.albumType,
			draftsOnly: this.draftsOnly || undefined,
			filter: this.searchTerm || undefined,
			page: this.paging.page,
			pageSize: this.pageSize,
			sort: this.sort,
			tagId: this.tagIds,
			viewMode: this.viewMode,
		};
	}
	public set routeParams(value: SearchRouteParams) {
		if (value.searchType !== SearchType.Album) return;

		this.artistFilters.artistIds = value.artistId ?? [];
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

	public shouldClearResults = (value: SearchRouteParams): boolean => {
		if (value.searchType !== SearchType.Album) return true;

		const routeParams = this.routeParams;
		if (routeParams.searchType !== SearchType.Album) return true;

		if (!_.isEqual(value.tagId, routeParams.tagId)) return true;
		if (value.draftsOnly !== routeParams.draftsOnly) return true;

		// TODO: advancedFilters
		if (value.sort !== routeParams.sort) return true;
		if (value.discType !== routeParams.discType) return true;
		if (!_.isEqual(value.artistId, routeParams.artistId)) return true;

		return false;
	};
}
