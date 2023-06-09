import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ArtistHelper } from '@/Helpers/ArtistHelper';
import { ArtistType } from '@/Models/Artists/ArtistType';
import {
	ArtistOptionalField,
	ArtistRepository,
} from '@/Repositories/ArtistRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { AdvancedSearchFilter } from '@/Stores/Search/AdvancedSearchFilter';
import { ICommonSearchStore } from '@/Stores/Search/CommonSearchStore';
import { SearchCategoryBaseStore } from '@/Stores/Search/SearchCategoryBaseStore';
import { SearchType } from '@/Stores/Search/SearchStore';
import { includesAny, StateChangeEvent } from '@/route-sphere';
import { computed, makeObservable, observable } from 'mobx';

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
	ArtistType = 'ArtistType',
}

export interface ArtistSearchRouteParams {
	advancedFilters?: AdvancedSearchFilter[];
	artistType?: ArtistType;
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
	languages?: string[];
}

const clearResultsByQueryKeys: (keyof ArtistSearchRouteParams)[] = [
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
	'languages',
	// TODO: onlyRootVoicebanks
];

export class ArtistSearchStore extends SearchCategoryBaseStore<
	ArtistSearchRouteParams,
	ArtistContract
> {
	@observable artistType = ArtistType.Unknown;
	@observable onlyFollowedByMe = false;
	@observable onlyRootVoicebanks = false;
	@observable sort = ArtistSortRule.Name;
	@observable languages?: string[] = undefined;
	@observable languagesExtended: boolean = false;

	constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		private readonly artistRepo: ArtistRepository,
	) {
		super(commonSearchStore);

		makeObservable(this);
	}

	@computed get fields(): ArtistOptionalField[] {
		return this.showTags
			? [
					ArtistOptionalField.AdditionalNames,
					ArtistOptionalField.MainPicture,
					ArtistOptionalField.Tags,
			  ]
			: [ArtistOptionalField.AdditionalNames, ArtistOptionalField.MainPicture];
	}

	loadResults = (
		pagingProperties: PagingProperties,
	): Promise<PartialFindResultContract<ArtistContract>> => {
		return this.artistRepo.getList({
			paging: pagingProperties,
			lang: this.values.languagePreference,
			query: this.searchTerm,
			sort: this.sort,
			artistTypes:
				this.artistType !== ArtistType.Unknown ? [this.artistType] : undefined,
			allowBaseVoicebanks: !this.onlyRootVoicebanks,
			tags: this.tagIds,
			childTags: this.childTags,
			followedByUserId: this.onlyFollowedByMe
				? this.values.loggedUserId
				: undefined,
			fields: this.fields,
			status: this.draftsOnly ? 'Draft' : undefined,
			advancedFilters: this.advancedFilters.filters,
			languages: this.languages,
		});
	};

	@computed get canHaveChildVoicebanks(): boolean {
		return ArtistHelper.canHaveChildVoicebanks(this.artistType);
	}

	@computed.struct get locationState(): ArtistSearchRouteParams {
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
			languages: this.languages,
		};
	}
	set locationState(value: ArtistSearchRouteParams) {
		this.advancedFilters.filters = value.advancedFilters ?? [];
		this.artistType = value.artistType ?? ArtistType.Unknown;
		this.childTags = value.childTags ?? false;
		this.draftsOnly = value.draftsOnly ?? false;
		this.searchTerm = value.filter ?? '';
		this.onlyFollowedByMe = value.onlyFollowedByMe ?? false;
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 10;
		this.sort = value.sort ?? ArtistSortRule.Name;
		this.tagIds = ([] as number[]).concat(value.tagId ?? []);
		this.languages = value.languages;
	}

	onLocationStateChange = (
		event: StateChangeEvent<ArtistSearchRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) {
			this.paging.goToFirstPage();
		} else {
			this.languagesExtended = false;
		}

		this.updateResults(clearResults);
	};
}
