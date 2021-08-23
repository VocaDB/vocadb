import ArtistContract from '@DataContracts/Artist/ArtistContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import ArtistHelper from '@Helpers/ArtistHelper';
import ArtistType from '@Models/Artists/ArtistType';
import ArtistRepository from '@Repositories/ArtistRepository';
import GlobalValues from '@Shared/GlobalValues';
import { computed, makeObservable, observable, reaction } from 'mobx';

import { ICommonSearchStore } from './CommonSearchStore';
import SearchCategoryBaseStore from './SearchCategoryBaseStore';
import { SearchRouteParams, SearchType } from './SearchStore';

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
	artistType?: string /* TODO: enum */;
	childTags?: boolean;
	draftsOnly?: boolean;
	filter?: string;
	page?: number;
	pageSize?: number;
	searchType: SearchType.Artist;
	sort?: ArtistSortRule;
	tag?: string;
	tagId?: number[];
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

		reaction(
			() => this.advancedFilters.filters.map((filter) => filter.description),
			this.updateResultsWithTotalCount,
		);
		reaction(() => this.sort, this.updateResultsWithTotalCount);
		reaction(() => this.artistType, this.updateResultsWithTotalCount);
		reaction(() => this.onlyFollowedByMe, this.updateResultsWithTotalCount);
		reaction(() => this.onlyRootVoicebanks, this.updateResultsWithTotalCount);
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

	@computed public get routeParams(): SearchRouteParams {
		return {};
	}
	public set routeParams(value: SearchRouteParams) {}
}
