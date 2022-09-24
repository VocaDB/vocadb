import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { SongContract } from '@/DataContracts/Song/SongContract';
import { IEntryWithIdAndName } from '@/Models/IEntryWithIdAndName';
import { PVServiceIcons } from '@/Models/PVServiceIcons';
import { SongType } from '@/Models/Songs/SongType';
import { ISongSearchStore } from '@/Pages/Search/Partials/SongSearchList';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import {
	SongGetListQueryParams,
	SongOptionalField,
	SongRepository,
} from '@/Repositories/SongRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { UrlMapper } from '@/Shared/UrlMapper';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { PVPlayerStore } from '@/Stores/PVs/PVPlayerStore';
import { PVPlayersFactory } from '@/Stores/PVs/PVPlayersFactory';
import { AdvancedSearchFilter } from '@/Stores/Search/AdvancedSearchFilter';
import { ArtistFilters } from '@/Stores/Search/ArtistFilters';
import { ICommonSearchStore } from '@/Stores/Search/CommonSearchStore';
import { SearchCategoryBaseStore } from '@/Stores/Search/SearchCategoryBaseStore';
import { SearchType } from '@/Stores/Search/SearchStore';
import { SongBpmFilter } from '@/Stores/Search/SongBpmFilter';
import { SongLengthFilter } from '@/Stores/Search/SongLengthFilter';
import {
	ISongsAdapterStore,
	PlayListRepositoryForSongsAdapter,
} from '@/Stores/Song/PlayList/PlayListRepositoryForSongsAdapter';
import { PlayListStore } from '@/Stores/Song/PlayList/PlayListStore';
import { SongWithPreviewStore } from '@/Stores/Song/SongWithPreviewStore';
import { includesAny, StateChangeEvent } from '@vocadb/route-sphere';
import { computed, makeObservable, observable } from 'mobx';
import moment from 'moment';

export interface ISongSearchItem extends SongApiContract {
	previewStore?: SongWithPreviewStore;
}

export type SongVoteRating = 'Nothing' | 'Like' | 'Favorite';

export interface IRatedSongSearchItem extends ISongSearchItem {
	rating?: SongVoteRating;
}

// Corresponds to the SongSortRule enum in C#.
export enum SongSortRule {
	None = 'None',
	Name = 'Name',
	AdditionDate = 'AdditionDate',
	PublishDate = 'PublishDate',
	FavoritedTimes = 'FavoritedTimes',
	RatingScore = 'RatingScore',
	TagUsageCount = 'TagUsageCount',
	SongType = 'SongType',
}

export interface SongSearchRouteParams {
	advancedFilters?: AdvancedSearchFilter[];
	artistId?: number | number[];
	artistParticipationStatus?: string /* TODO: enum */;
	autoplay?: boolean;
	childTags?: boolean;
	childVoicebanks?: boolean;
	dateYear?: number;
	dateMonth?: number;
	dateDay?: number;
	draftsOnly?: boolean;
	eventId?: number;
	filter?: string;
	includeMembers?: boolean;
	maxLength?: number;
	maxMilliBpm?: number;
	minLength?: number;
	minMilliBpm?: number;
	minScore?: number;
	onlyWithPVs?: boolean;
	onlyRatedSongs?: boolean;
	page?: number;
	pageSize?: number;
	parentVersionId?: number;
	searchType?: SearchType.Song;
	shuffle?: boolean;
	since?: number;
	songType?: SongType;
	sort?: SongSortRule;
	tag?: string;
	tagId?: number | number[];
	unifyEntryTypesAndTags?: boolean;
	viewMode?: 'Details' | 'PlayList' /* TODO: enum */;
}

const clearResultsByQueryKeys: (keyof SongSearchRouteParams)[] = [
	'pageSize',
	'filter',
	'tagId',
	'childTags',
	'draftsOnly',
	'searchType',

	'advancedFilters',
	'artistId',
	'artistParticipationStatus',
	'childVoicebanks',
	'includeMembers',
	'dateYear',
	'dateMonth',
	'dateDay',
	'eventId',
	'minScore',
	'onlyRatedSongs',
	'parentVersionId',
	'onlyWithPVs',
	'since',
	'songType',
	'sort',
	'unifyEntryTypesAndTags',
	'viewMode',
	'minMilliBpm',
	'maxMilliBpm',
	'minLength',
	'maxLength',
];

export class SongSearchStore
	extends SearchCategoryBaseStore<SongSearchRouteParams, ISongSearchItem>
	implements ISongSearchStore, ISongsAdapterStore {
	public readonly artistFilters: ArtistFilters;
	@observable public dateDay?: number = undefined;
	@observable public dateMonth?: number = undefined;
	@observable public dateYear?: number = undefined;
	public readonly releaseEvent: BasicEntryLinkStore<IEntryWithIdAndName>;
	@observable public minScore?: number;
	@observable public onlyRatedSongs = false;
	public readonly parentVersion: BasicEntryLinkStore<SongContract>;
	public readonly playListStore: PlayListStore;
	public readonly pvPlayerStore: PVPlayerStore;
	@observable public pvsOnly = false;
	private readonly pvServiceIcons: PVServiceIcons;
	@observable public since?: number;
	@observable public songType = SongType.Unspecified;
	@observable public sort = SongSortRule.RatingScore;
	@observable public unifyEntryTypesAndTags = false;
	@observable public viewMode: 'Details' | 'PlayList' =
		'Details' /* TODO: enum */;
	public readonly minBpmFilter = new SongBpmFilter();
	public readonly maxBpmFilter = new SongBpmFilter();
	public readonly minLengthFilter = new SongLengthFilter();
	public readonly maxLengthFilter = new SongLengthFilter();

	public constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		urlMapper: UrlMapper,
		private readonly songRepo: SongRepository,
		private readonly userRepo: UserRepository,
		eventRepo: ReleaseEventRepository,
		artistRepo: ArtistRepository,
		pvPlayersFactory: PVPlayersFactory,
	) {
		super(commonSearchStore);

		makeObservable(this);

		this.pvServiceIcons = new PVServiceIcons(urlMapper);

		this.artistFilters = new ArtistFilters(values, artistRepo);

		this.releaseEvent = new BasicEntryLinkStore<IEntryWithIdAndName>(
			(entryId) =>
				eventRepo
					? eventRepo.getOne({ id: entryId })
					: Promise.resolve(undefined),
		);

		this.parentVersion = new BasicEntryLinkStore<SongContract>((entryId) =>
			songRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);

		this.pvPlayerStore = new PVPlayerStore(
			values,
			songRepo,
			userRepo,
			pvPlayersFactory,
		);

		const songsRepoAdapter = new PlayListRepositoryForSongsAdapter(
			values,
			songRepo,
			this,
		);

		this.playListStore = new PlayListStore(
			values,
			urlMapper,
			songsRepoAdapter,
			this.pvPlayerStore,
		);
	}

	// Remember, JavaScript months start from 0 (who came up with that??)
	// The answer song: https://stackoverflow.com/questions/2552483/why-does-the-month-argument-range-from-0-to-11-in-javascripts-date-constructor/41992352#41992352
	private toDateOrUndefined = (mom: moment.Moment): Date | undefined =>
		mom.isValid() ? mom.toDate() : undefined;

	@computed public get afterDate(): Date | undefined {
		return this.dateYear
			? this.toDateOrUndefined(
					moment.utc([
						this.dateYear,
						(this.dateMonth || 1) - 1,
						this.dateDay || 1,
					]),
			  )
			: undefined;
	}

	@computed public get beforeDate(): Date | undefined {
		if (!this.dateYear) return undefined;

		const mom = moment.utc([
			this.dateYear,
			(this.dateMonth || 12) - 1,
			this.dateDay || 1,
		]);

		return this.toDateOrUndefined(
			this.dateMonth && this.dateDay ? mom.add(1, 'd') : mom.add(1, 'M'),
		);
	}

	@computed public get fields(): SongOptionalField[] {
		return this.showTags
			? [
					SongOptionalField.AdditionalNames,
					SongOptionalField.MainPicture,
					SongOptionalField.Tags,
			  ]
			: [SongOptionalField.AdditionalNames, SongOptionalField.MainPicture];
	}

	@computed public get showUnifyEntryTypesAndTags(): boolean {
		return (
			this.songType !== SongType.Unspecified &&
			this.songType !== SongType.Original
		);
	}

	@computed public get queryParams(): SongGetListQueryParams {
		return {
			query: this.searchTerm,
			sort: this.sort,
			songTypes:
				this.songType !== SongType.Unspecified ? [this.songType] : undefined,
			afterDate: this.afterDate,
			beforeDate: this.beforeDate,
			tagIds: this.tagIds,
			childTags: this.childTags,
			unifyTypesAndTags: this.unifyEntryTypesAndTags,
			artistIds: this.artistFilters.artistIds,
			artistParticipationStatus: this.artistFilters.artistParticipationStatus,
			childVoicebanks: this.artistFilters.childVoicebanks,
			includeMembers: this.artistFilters.includeMembers,
			eventId: this.releaseEvent.id,
			onlyWithPvs: this.pvsOnly,
			since: this.since,
			minScore: this.minScore,
			userCollectionId: this.onlyRatedSongs
				? this.values.loggedUserId
				: undefined,
			parentSongId: this.parentVersion.id,
			status: this.draftsOnly ? 'Draft' : undefined,
			advancedFilters: this.advancedFilters.filters,
			minMilliBpm: this.minBpmFilter.milliBpm,
			maxMilliBpm: this.maxBpmFilter.milliBpm,
			minLength: this.minLengthFilter.length
				? this.minLengthFilter.length
				: undefined,
			maxLength: this.maxLengthFilter.length
				? this.maxLengthFilter.length
				: undefined,
		};
	}

	public loadResults = async (
		pagingProperties: PagingProperties,
	): Promise<PartialFindResultContract<ISongSearchItem>> => {
		if (this.viewMode === 'PlayList') {
			this.playListStore.updateResultsWithTotalCount();
			return { items: [], totalCount: 0 };
		} else {
			const result = await this.songRepo.getList({
				fields: this.fields,
				lang: this.values.languagePreference,
				paging: pagingProperties,
				pvServices: undefined,
				queryParams: this.queryParams,
			});

			for (const song of result.items as ISongSearchItem[]) {
				if (song.pvServices && song.pvServices !== 'Nothing') {
					song.previewStore = new SongWithPreviewStore(
						this.songRepo,
						this.userRepo,
						song.id,
						song.pvServices,
					);
					// TODO: song.previewStore.ratingComplete = ui.showThankYouForRatingMessage;
				} else {
					song.previewStore = undefined;
				}
			}

			return result;
		}
	};

	public getPVServiceIcons = (
		services: string,
	): { service: string; url: string }[] => {
		return this.pvServiceIcons.getIconUrls(services);
	};

	@computed.struct public get locationState(): SongSearchRouteParams {
		return {
			searchType: SearchType.Song,
			advancedFilters: this.advancedFilters.filters.map((filter) => ({
				description: filter.description,
				filterType: filter.filterType,
				negate: filter.negate,
				param: filter.param,
			})),
			artistId: this.artistFilters.artistIds,
			artistParticipationStatus: this.artistFilters.artistParticipationStatus,
			// TODO: autoplay
			childTags: this.childTags,
			childVoicebanks: this.artistFilters.childVoicebanks,
			dateDay: this.dateDay,
			dateMonth: this.dateMonth,
			dateYear: this.dateYear,
			draftsOnly: this.draftsOnly,
			eventId: this.releaseEvent.id,
			filter: this.searchTerm,
			// TODO: includeMembers
			maxLength: this.maxLengthFilter.length,
			maxMilliBpm: this.maxBpmFilter.milliBpm,
			minLength: this.minLengthFilter.length,
			minMilliBpm: this.minBpmFilter.milliBpm,
			minScore: this.minScore,
			onlyRatedSongs: this.onlyRatedSongs,
			onlyWithPVs: this.pvsOnly,
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			parentVersionId: this.parentVersion.id,
			// TODO: shuffle
			since: this.since,
			songType: this.songType,
			sort: this.sort,
			tagId: this.tagIds,
			unifyEntryTypesAndTags: this.unifyEntryTypesAndTags,
			viewMode: this.viewMode,
		};
	}
	public set locationState(value: SongSearchRouteParams) {
		this.advancedFilters.filters = value.advancedFilters ?? [];
		this.artistFilters.artistIds = ([] as number[]).concat(
			value.artistId ?? [],
		);
		this.artistFilters.artistParticipationStatus =
			value.artistParticipationStatus ?? 'Everything';
		// TODO: autoplay
		this.childTags = value.childTags ?? false;
		this.artistFilters.childVoicebanks = value.childVoicebanks ?? false;
		this.dateDay = value.dateDay;
		this.dateMonth = value.dateMonth;
		this.dateYear = value.dateYear;
		this.draftsOnly = value.draftsOnly ?? false;
		this.releaseEvent.id = value.eventId;
		this.searchTerm = value.filter ?? '';
		// TODO: includeMembers
		this.maxLengthFilter.length = value.maxLength ?? 0;
		this.maxBpmFilter.milliBpm = value.maxMilliBpm;
		this.minLengthFilter.length = value.minLength ?? 0;
		this.minBpmFilter.milliBpm = value.minMilliBpm;
		this.minScore = value.minScore;
		this.onlyRatedSongs = value.onlyRatedSongs ?? false;
		this.pvsOnly = value.onlyWithPVs ?? false;
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 10;
		this.parentVersion.id = value.parentVersionId;
		// TODO: shuffle
		this.since = value.since;
		this.songType = value.songType ?? SongType.Unspecified;
		this.sort = value.sort ?? SongSortRule.RatingScore;
		this.tagIds = ([] as number[]).concat(value.tagId ?? []);
		this.unifyEntryTypesAndTags = value.unifyEntryTypesAndTags ?? false;
		this.viewMode = value.viewMode ?? 'Details';
	}

	public onLocationStateChange = (
		event: StateChangeEvent<SongSearchRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
