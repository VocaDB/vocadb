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
}
