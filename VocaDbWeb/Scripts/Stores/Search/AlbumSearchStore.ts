import AlbumContract from '@DataContracts/Album/AlbumContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import AlbumRepository from '@Repositories/AlbumRepository';
import ArtistRepository from '@Repositories/ArtistRepository';
import GlobalValues from '@Shared/GlobalValues';
import _ from 'lodash';
import { computed, makeObservable, observable, reaction } from 'mobx';

import ArtistFilters from './ArtistFilters';
import { ICommonSearchStore } from './CommonSearchStore';
import SearchCategoryBaseStore from './SearchCategoryBaseStore';
import SearchQueryParams from './SearchQueryParams';
import { SearchType } from './SearchStore';

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

export default class AlbumSearchStore extends SearchCategoryBaseStore<AlbumContract> {
	@observable public albumType: string /* TODO: enum */;
	public readonly artistFilters: ArtistFilters;
	@observable public sort: AlbumSortRule;
	@observable public viewMode: string /* TODO: enum */;

	public constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		private readonly albumRepo: AlbumRepository,
		artistRepo: ArtistRepository,
		sort?: AlbumSortRule,
		artistId?: number[],
		childVoicebanks?: boolean,
		albumType?: string,
		viewMode?: string,
	) {
		super(commonSearchStore);

		makeObservable(this);

		reaction(
			() => this.advancedFilters.filters.map((filter) => filter.description),
			this.updateResultsWithTotalCount,
		);
		this.artistFilters = new ArtistFilters(values, artistRepo, childVoicebanks);
		this.artistFilters.selectArtists(artistId);

		this.albumType = albumType || 'Unknown';
		this.sort = sort || AlbumSortRule.Name;
		this.viewMode = viewMode || 'Details';

		reaction(() => this.sort, this.updateResultsWithTotalCount);
		reaction(() => this.albumType, this.updateResultsWithTotalCount);
		reaction(
			() => this.artistFilters.filters,
			this.updateResultsWithTotalCount,
		);
	}

	@computed public get queryParams(): SearchQueryParams {
		return {
			searchType: SearchType.Album,
			filter: this.searchTerm,
			tagId: this.tagIds,
			sort: this.sort,
			artistId: this.artistFilters.artistIds,
			childTags: this.childTags,
			childVoicebanks: this.artistFilters.childVoicebanks,
			discType: this.albumType,
			viewMode: this.viewMode,
			pageSize: this.pageSize,
		};
	}
	public set queryParams(value: SearchQueryParams) {
		if (value.searchType !== SearchType.Album) return;

		this.searchTerm = value.filter ?? '';
		this.tagIds = value.tagId ?? [];
		this.sort = value.sort ?? AlbumSortRule.Name;
		this.artistFilters.artistIds = value.artistId ?? [];
		this.childTags = value.childTags ?? false;
		this.artistFilters.childVoicebanks = value.childVoicebanks ?? false;
		this.albumType = value.discType ?? 'Unknown';
		this.viewMode = value.viewMode ?? 'Details';
		this.pageSize = value.pageSize ?? 10;
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
}
