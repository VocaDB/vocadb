import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import ArtistRepository from '@Repositories/ArtistRepository';
import ReleaseEventRepository from '@Repositories/ReleaseEventRepository';
import GlobalValues from '@Shared/GlobalValues';
import { computed, makeObservable, observable } from 'mobx';

import ArtistFilters from './ArtistFilters';
import { ICommonSearchStore } from './CommonSearchStore';
import SearchCategoryBaseStore from './SearchCategoryBaseStore';
import { SearchType } from './SearchStore';

// Corresponds to the EventSortRule enum in C#.
export enum EventSortRule {
	None = 'None',
	Name = 'Name',
	Date = 'Date',
	AdditionDate = 'AdditionDate',
	SeriesName = 'SeriesName',
	VenueName = 'VenueName',
}

export interface EventSearchRouteParams {
	afterDate?: string /* TODO: use Date */;
	artistId?: number | number[];
	beforeDate?: string /* TODO: use Date */;
	childTags?: boolean;
	childVoicebanks?: boolean;
	draftsOnly?: boolean;
	eventCategory?: string;
	filter?: string;
	includeMembers?: boolean;
	onlyMyEvents?: boolean;
	page?: number;
	pageSize?: number;
	searchType?: SearchType.ReleaseEvent;
	sort?: EventSortRule;
	tag?: string;
	tagId?: number | number[];
}

export default class EventSearchStore extends SearchCategoryBaseStore<ReleaseEventContract> {
	@observable public afterDate?: Date = undefined;
	@observable public allowAliases = false;
	public readonly artistFilters: ArtistFilters;
	@observable public beforeDate?: Date = undefined;
	@observable public category = '';
	@observable public onlyMyEvents = false;
	@observable public sort = EventSortRule.Name;

	public constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		private readonly eventRepo: ReleaseEventRepository,
		artistRepo: ArtistRepository,
	) {
		super(commonSearchStore);

		makeObservable(this);

		this.artistFilters = new ArtistFilters(values, artistRepo);
	}

	@computed public get fields(): string {
		return this.showTags
			? 'AdditionalNames,MainPicture,Series,Venue,Tags'
			: 'AdditionalNames,MainPicture,Series,Venue';
	}

	public loadResults = (
		pagingProperties: PagingProperties,
		searchTerm: string,
		tags: number[],
		childTags: boolean,
		status?: string,
	): Promise<PartialFindResultContract<ReleaseEventContract>> => {
		return this.eventRepo.getList({
			queryParams: {
				start: pagingProperties.start,
				maxResults: pagingProperties.maxEntries,
				getTotalCount: pagingProperties.getTotalCount,
				lang: this.values.languagePreference,
				query: searchTerm,
				sort: this.sort,
				category: this.category === 'Unspecified' ? undefined : this.category,
				childTags: childTags,
				tagIds: tags,
				userCollectionId: this.onlyMyEvents
					? this.values.loggedUserId
					: undefined,
				artistId: this.artistFilters.artistIds,
				childVoicebanks: this.artistFilters.childVoicebanks,
				includeMembers: this.artistFilters.includeMembers,
				afterDate: this.afterDate,
				beforeDate: this.beforeDate,
				status: status,
				fields: this.fields,
			},
		});
	};

	public readonly clearResultsByQueryKeys: (keyof EventSearchRouteParams)[] = [
		'pageSize',
		'filter',
		'tagId',
		'childTags',
		'draftsOnly',
		'searchType',

		'afterDate',
		'beforeDate',
		'artistId',
		'childVoicebanks',
		'includeMembers',
		'eventCategory',
		'onlyMyEvents',
		'sort',
	];

	@computed.struct public get routeParams(): EventSearchRouteParams {
		return {
			searchType: SearchType.ReleaseEvent,
			afterDate: this.afterDate?.toISOString(),
			artistId: this.artistFilters.artistIds,
			beforeDate: this.beforeDate?.toISOString(),
			childTags: this.childTags,
			childVoicebanks: this.artistFilters.childVoicebanks,
			draftsOnly: this.draftsOnly,
			eventCategory: this.category,
			filter: this.searchTerm,
			onlyMyEvents: this.onlyMyEvents,
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			sort: this.sort,
			tagId: this.tagIds,
		};
	}
	public set routeParams(value: EventSearchRouteParams) {
		this.afterDate = value.afterDate ? new Date(value.afterDate) : undefined;
		this.artistFilters.artistIds = value.artistId
			? ([] as number[]).concat(value.artistId)
			: [];
		this.beforeDate = value.beforeDate ? new Date(value.beforeDate) : undefined;
		this.childTags = value.childTags ?? false;
		this.artistFilters.childVoicebanks = value.childVoicebanks ?? false;
		this.draftsOnly = value.draftsOnly ?? false;
		this.category = value.eventCategory ?? '';
		this.searchTerm = value.filter ?? '';
		this.onlyMyEvents = value.onlyMyEvents ?? false;
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 10;
		this.sort = value.sort ?? EventSortRule.Name;
		this.tagIds = value.tagId ? ([] as number[]).concat(value.tagId) : [];
	}
}
