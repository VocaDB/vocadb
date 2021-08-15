import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import SongApiContract from '@DataContracts/Song/SongApiContract';
import SongContract from '@DataContracts/Song/SongContract';
import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';
import PVServiceIcons from '@Models/PVServiceIcons';
import SongType from '@Models/Songs/SongType';
import ArtistRepository from '@Repositories/ArtistRepository';
import ReleaseEventRepository from '@Repositories/ReleaseEventRepository';
import SongRepository from '@Repositories/SongRepository';
import UserRepository from '@Repositories/UserRepository';
import GlobalValues from '@Shared/GlobalValues';
import UrlMapper from '@Shared/UrlMapper';
import BasicEntryLinkStore from '@Stores/BasicEntryLinkStore';
import SongWithPreviewStore from '@Stores/Song/SongWithPreviewStore';
import debounceEffect from '@Stores/debounceEffect';
import _ from 'lodash';
import { computed, makeObservable, observable, reaction } from 'mobx';
import moment from 'moment';

import ArtistFilters from './ArtistFilters';
import { ICommonSearchStore } from './CommonSearchStore';
import SearchCategoryBaseStore from './SearchCategoryBaseStore';
import SongBpmFilter from './SongBpmFilter';
import SongLengthFilter from './SongLengthFilter';

// Corresponds to the SongSortRule enum in C#.
export enum SongSortRule {
	None = 'None',
	Name = 'Name',
	AdditionDate = 'AdditionDate',
	PublishDate = 'PublishDate',
	FavoritedTimes = 'FavoritedTimes',
	RatingScore = 'RatingScore',
	TagUsageCount = 'TagUsageCount',
}

interface ISongSearchItem extends SongApiContract {
	previewStore?: SongWithPreviewStore;
}

export default class SongSearchStore extends SearchCategoryBaseStore<ISongSearchItem> {
	public readonly artistFilters: ArtistFilters;
	@observable public dateDay?: number = undefined;
	@observable public dateMonth?: number = undefined;
	@observable public dateYear?: number = undefined;
	public readonly releaseEvent: BasicEntryLinkStore<IEntryWithIdAndName>;
	@observable public minScore?: number;
	@observable public onlyRatedSongs = false;
	public readonly parentVersion: BasicEntryLinkStore<SongContract>;
	// TODO: public readonly playListStore: PlayListStore;
	// TODO: public readonly pvPlayerStore: PVPlayerStore;
	@observable public pvsOnly = false;
	private readonly pvServiceIcons: PVServiceIcons;
	@observable public since?: number;
	@observable public songType = SongType[SongType.Unspecified] /* TODO: enum */;
	@observable public sort = SongSortRule.Name;
	@observable public unifyEntryTypesAndTags = false;
	@observable public viewMode: string /* TODO: enum */;
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
		private readonly eventRepo: ReleaseEventRepository,
		artistRepo: ArtistRepository,
		// TODO: pvPlayersFactory
		sort?: SongSortRule,
		artistId?: number[],
		childVoicebanks?: boolean,
		songType?: string,
		eventId?: number,
		onlyWithPVs?: boolean,
		onlyRatedSongs?: boolean,
		since?: number,
		minScore?: number,
		viewMode?: string,
		autoplay?: boolean,
		shuffle?: boolean,
	) {
		super(commonSearchStore);

		makeObservable(this);

		this.pvServiceIcons = new PVServiceIcons(urlMapper);

		this.artistFilters = new ArtistFilters(values, artistRepo, childVoicebanks);
		this.artistFilters.selectArtists(artistId);

		this.releaseEvent = new BasicEntryLinkStore<IEntryWithIdAndName>(
			{ id: eventId!, name: undefined },
			(entryId) =>
				this.eventRepo
					? eventRepo.getOne({ id: entryId })
					: Promise.resolve(undefined),
		);

		if (eventId) this.releaseEvent.id = eventId;

		if (sort) this.sort = sort;

		if (songType) this.songType = songType;

		if (onlyWithPVs) this.pvsOnly = onlyWithPVs;

		if (onlyRatedSongs) this.onlyRatedSongs = onlyRatedSongs;

		this.minScore = minScore || undefined;
		this.since = since;
		this.viewMode = viewMode || 'Details';

		this.parentVersion = new BasicEntryLinkStore<SongContract>(
			undefined,
			(entryId) =>
				songRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);

		reaction(
			() => this.advancedFilters.filters.map((filter) => filter.description),
			this.updateResultsWithTotalCount,
		);
		reaction(
			() => this.artistFilters.filters,
			this.updateResultsWithTotalCount,
		);
		reaction(
			() => this.afterDate,
			debounceEffect(this.updateResultsWithTotalCount, 300),
		);
		reaction(() => this.releaseEvent.id, this.updateResultsWithTotalCount);
		reaction(
			() => this.minScore,
			debounceEffect(this.updateResultsWithTotalCount, 300),
		);
		reaction(() => this.onlyRatedSongs, this.updateResultsWithTotalCount);
		reaction(() => this.parentVersion.id, this.updateResultsWithTotalCount);
		// TODO: this.pvPlayerStore = new PVPlayerStore();
		reaction(() => this.pvsOnly, this.updateResultsWithTotalCount);
		reaction(() => this.since, this.updateResultsWithTotalCount);
		reaction(() => this.songType, this.updateResultsWithTotalCount);
		reaction(() => this.sort, this.updateResultsWithTotalCount);
		reaction(
			() => this.unifyEntryTypesAndTags,
			this.updateResultsWithTotalCount,
		);
		reaction(() => this.viewMode, this.updateResultsWithTotalCount);
		reaction(
			() => this.minBpmFilter.milliBpm,
			debounceEffect(this.updateResultsWithTotalCount, 300),
		);
		reaction(
			() => this.maxBpmFilter.milliBpm,
			debounceEffect(this.updateResultsWithTotalCount, 300),
		);
		reaction(
			() => this.minLengthFilter.length,
			debounceEffect(this.updateResultsWithTotalCount, 300),
		);
		reaction(
			() => this.maxLengthFilter.length,
			debounceEffect(this.updateResultsWithTotalCount, 300),
		);

		// TODO: this.playListStore = ;
	}

	// Remember, JavaScript months start from 0 (who came up with that??)
	// The answer song: https://stackoverflow.com/questions/2552483/why-does-the-month-argument-range-from-0-to-11-in-javascripts-date-constructor/41992352#41992352
	private toDateOrUndefined = (mom: moment.Moment): Date | undefined =>
		mom.isValid() ? mom.toDate() : undefined;

	@computed private get afterDate(): Date | undefined {
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

	@computed private get beforeDate(): Date | undefined {
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

	@computed public get fields(): string {
		return this.showTags
			? 'AdditionalNames,ThumbUrl,Tags'
			: 'AdditionalNames,ThumbUrl';
	}

	@computed public get showUnifyEntryTypesAndTags(): boolean {
		return (
			this.songType !== SongType[SongType.Unspecified] &&
			this.songType !== SongType[SongType.Original]
		);
	}

	public loadResults = (
		pagingProperties: PagingProperties,
		searchTerm: string,
		tags: number[],
		childTags: boolean,
		status?: string,
	): Promise<PartialFindResultContract<ISongSearchItem>> => {
		if (this.viewMode === 'PlayList') {
			// TODO: this.playListStore.updateResultsWithTotalCount();
			return Promise.resolve({ items: [], totalCount: 0 });
		} else {
			return this.songRepo
				.getList({
					paging: pagingProperties,
					lang: this.values.languagePreference,
					query: searchTerm,
					sort: this.sort,
					songTypes:
						this.songType !== SongType[SongType.Unspecified]
							? this.songType
							: undefined,
					afterDate: this.afterDate,
					beforeDate: this.beforeDate,
					tagIds: tags,
					childTags: childTags,
					unifyTypesAndTags: this.unifyEntryTypesAndTags,
					artistIds: this.artistFilters.artistIds,
					artistParticipationStatus: this.artistFilters
						.artistParticipationStatus,
					childVoicebanks: this.artistFilters.childVoicebanks,
					includeMembers: this.artistFilters.includeMembers,
					eventId: this.releaseEvent.id,
					onlyWithPvs: this.pvsOnly,
					pvServices: undefined,
					since: this.since,
					minScore: this.minScore,
					userCollectionId: this.onlyRatedSongs
						? this.values.loggedUserId
						: undefined,
					parentSongId: this.parentVersion.id,
					fields: this.fields,
					status: status,
					advancedFilters: this.advancedFilters.filters,
					minMilliBpm: this.minBpmFilter.milliBpm,
					maxMilliBpm: this.maxBpmFilter.milliBpm,
					minLength: this.minLengthFilter.length
						? this.minLengthFilter.length
						: undefined,
					maxLength: this.maxLengthFilter.length
						? this.maxLengthFilter.length
						: undefined,
				})
				.then((result) => {
					_.each(result.items, (song: ISongSearchItem) => {
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
					});

					return result;
				});
		}
	};

	public getPVServiceIcons = (
		services: string,
	): { service: string; url: string }[] => {
		return this.pvServiceIcons.getIconUrls(services);
	};
}
