import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import {
	ReleaseEventOptionalField,
	ReleaseEventRepository,
} from '@/Repositories/ReleaseEventRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { ArtistFilters } from '@/Stores/Search/ArtistFilters';
import { ICommonSearchStore } from '@/Stores/Search/CommonSearchStore';
import { SearchCategoryBaseStore } from '@/Stores/Search/SearchCategoryBaseStore';
import { SearchType } from '@/Stores/Search/SearchStore';
import { includesAny, StateChangeEvent } from '@/route-sphere';
import { computed, makeObservable, observable } from 'mobx';

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

const clearResultsByQueryKeys: (keyof EventSearchRouteParams)[] = [
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

export class EventSearchStore extends SearchCategoryBaseStore<
	EventSearchRouteParams,
	ReleaseEventContract
> {
	@observable afterDate?: Date = undefined;
	@observable allowAliases = false;
	readonly artistFilters: ArtistFilters;
	@observable beforeDate?: Date = undefined;
	@observable category = '';
	@observable onlyMyEvents = false;
	@observable sort = EventSortRule.Name;

	constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		private readonly eventRepo: ReleaseEventRepository,
		artistRepo: ArtistRepository,
	) {
		super(commonSearchStore);

		makeObservable(this);

		this.artistFilters = new ArtistFilters(values, artistRepo);
	}

	@computed get fields(): ReleaseEventOptionalField[] {
		return this.showTags
			? [
					ReleaseEventOptionalField.AdditionalNames,
					ReleaseEventOptionalField.MainPicture,
					ReleaseEventOptionalField.Series,
					ReleaseEventOptionalField.Venue,
					ReleaseEventOptionalField.Tags,
			  ]
			: [
					ReleaseEventOptionalField.AdditionalNames,
					ReleaseEventOptionalField.MainPicture,
					ReleaseEventOptionalField.Series,
					ReleaseEventOptionalField.Venue,
			  ];
	}

	loadResults = (
		pagingProperties: PagingProperties,
	): Promise<PartialFindResultContract<ReleaseEventContract>> => {
		return this.eventRepo.getList({
			queryParams: {
				start: pagingProperties.start,
				maxResults: pagingProperties.maxEntries,
				getTotalCount: pagingProperties.getTotalCount,
				lang: this.values.languagePreference,
				query: this.searchTerm,
				sort: this.sort,
				category: this.category === 'Unspecified' ? undefined : this.category,
				tagIds: this.tagIds,
				childTags: this.childTags,
				userCollectionId: this.onlyMyEvents
					? this.values.loggedUserId
					: undefined,
				artistId: this.artistFilters.artistIds,
				childVoicebanks: this.artistFilters.childVoicebanks,
				includeMembers: this.artistFilters.includeMembers,
				afterDate: this.afterDate,
				beforeDate: this.beforeDate,
				status: this.draftsOnly ? 'Draft' : undefined,
				fields: this.fields,
			},
		});
	};

	@computed.struct get locationState(): EventSearchRouteParams {
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
	set locationState(value: EventSearchRouteParams) {
		this.afterDate = value.afterDate ? new Date(value.afterDate) : undefined;
		this.artistFilters.artistIds = ([] as number[]).concat(
			value.artistId ?? [],
		);
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
		this.tagIds = ([] as number[]).concat(value.tagId ?? []);
	}

	onLocationStateChange = (
		event: StateChangeEvent<EventSearchRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
