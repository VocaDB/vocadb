import { CommentContract } from '@/DataContracts/CommentContract';
import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { SongInListContract } from '@/DataContracts/Song/SongInListContract';
import { TagSelectionContract } from '@/DataContracts/Tag/TagSelectionContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { LoginManager } from '@/Models/LoginManager';
import { PVServiceIcons } from '@/Models/PVServiceIcons';
import { SongType } from '@/Models/Songs/SongType';
import { TagTargetType } from '@/Models/Tags/TagTargetType';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import type { SongListGetSongsQueryParams } from '@/Repositories/SongListRepository';
import { SongListRepository } from '@/Repositories/SongListRepository';
import {
	SongOptionalField,
	SongRepository,
} from '@/Repositories/SongRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { GlobalValues } from '@/Shared/GlobalValues';
import { UrlMapper } from '@/Shared/UrlMapper';
import { EditableCommentsStore } from '@/Stores/EditableCommentsStore';
import { AdvancedSearchFilter } from '@/Stores/Search/AdvancedSearchFilter';
import { AdvancedSearchFilters } from '@/Stores/Search/AdvancedSearchFilters';
import { ArtistFilters } from '@/Stores/Search/ArtistFilters';
import { ISongSearchItem } from '@/Stores/Search/SongSearchStore';
import { TagFilter } from '@/Stores/Search/TagFilter';
import { TagFilters } from '@/Stores/Search/TagFilters';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import { SongWithPreviewStore } from '@/Stores/Song/SongWithPreviewStore';
import { TagListStore } from '@/Stores/Tag/TagListStore';
import { TagsEditStore } from '@/Stores/Tag/TagsEditStore';
import {
	includesAny,
	StateChangeEvent,
	LocationStateStore,
} from '@/route-sphere';
import Ajv from 'ajv';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

import schema from './SongListRouteParams.schema.json';

interface SongListRouteParams {
	advancedFilters?: AdvancedSearchFilter[];
	artistId?: number[];
	artistParticipationStatus?: string /* TODO: enum */;
	childTags?: boolean;
	childVoicebanks?: boolean;
	page?: number;
	pageSize?: number;
	playlistMode?: boolean;
	query?: string;
	sort?: string /* TODO: enum */;
	songType?: SongType;
	tagId?: number[];
}

const clearResultsByQueryKeys: (keyof SongListRouteParams)[] = [
	'advancedFilters',
	'artistId',
	'artistParticipationStatus',
	'childTags',
	'childVoicebanks',
	'pageSize',
	'songType',
	'tagId',

	'sort',
	'playlistMode',
	'query',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<SongListRouteParams>(schema);

export class SongListStore implements LocationStateStore<SongListRouteParams> {
	readonly advancedFilters = new AdvancedSearchFilters();
	readonly artistFilters: ArtistFilters;
	readonly comments: EditableCommentsStore;
	@observable loading = true; // Currently loading for data
	@observable page: (SongInListContract & {
		song: ISongSearchItem;
	})[] = []; // Current page of items
	readonly paging = new ServerSidePagingStore(20); // Paging view model
	pauseNotifications = false;
	@observable playlistMode = false;
	readonly pvServiceIcons: PVServiceIcons;
	@observable query = '';
	@observable showAdvancedFilters = false;
	@observable showTags = false;
	@observable sort = '' /* TODO: enum */;
	@observable songType = SongType.Unspecified;
	readonly tagsEditStore: TagsEditStore;
	readonly tagFilters: TagFilters;
	readonly tagUsages: TagListStore;

	constructor(
		private readonly values: GlobalValues,
		loginManager: LoginManager,
		urlMapper: UrlMapper,
		private readonly songListRepo: SongListRepository,
		private readonly songRepo: SongRepository,
		tagRepo: TagRepository,
		private readonly userRepo: UserRepository,
		artistRepo: ArtistRepository,
		latestComments: CommentContract[],
		private readonly listId: number,
		tagUsages: TagUsageForApiContract[],
		canDeleteAllComments: boolean,
	) {
		makeObservable(this);

		this.artistFilters = new ArtistFilters(values, artistRepo);
		this.comments = new EditableCommentsStore(
			loginManager,
			songListRepo.getComments({}),
			listId,
			canDeleteAllComments,
			canDeleteAllComments,
			false,
			latestComments,
			true,
		);

		// TODO
		this.pvServiceIcons = new PVServiceIcons(urlMapper);

		this.tagsEditStore = new TagsEditStore(
			{
				getTagSelections: (): Promise<TagSelectionContract[]> =>
					userRepo.getSongListTagSelections({ songListId: this.listId }),
				saveTagSelections: async (tags): Promise<void> => {
					const usages = await userRepo.updateSongListTags({
						songListId: this.listId,
						tags: tags,
					});

					this.tagUsages.updateTagUsages(usages);
				},
			},
			TagTargetType.SongList,
		);

		this.tagFilters = new TagFilters(values, tagRepo);

		this.tagUsages = new TagListStore(tagUsages);

		reaction(() => this.showTags, this.updateResultsWithTotalCount);
	}

	@computed get childTags(): boolean {
		return this.tagFilters.childTags;
	}
	set childTags(value: boolean) {
		this.tagFilters.childTags = value;
	}

	@computed get tags(): TagFilter[] {
		return this.tagFilters.tags;
	}
	set tags(value: TagFilter[]) {
		this.tagFilters.tags = value;
	}

	@computed get tagIds(): number[] {
		return this.tags.map((t) => t.id);
	}
	set tagIds(value: number[]) {
		// OPTIMIZE
		this.tagFilters.tags = [];
		this.tagFilters.addTags(value);
	}

	mapTagUrl = (tagUsage: TagUsageForApiContract): string => {
		return EntryUrlMapper.details_tag(tagUsage.tag.id, tagUsage.tag.urlSlug);
	};

	@computed.struct get locationState(): SongListRouteParams {
		return {
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
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			playlistMode: this.playlistMode,
			query: this.query,
			sort: this.sort,
			songType: this.songType,
			tagId: this.tagIds,
		};
	}
	set locationState(value: SongListRouteParams) {
		this.advancedFilters.filters = value.advancedFilters ?? [];
		this.artistFilters.artistIds = ([] as number[]).concat(
			value.artistId ?? [],
		);
		this.artistFilters.artistParticipationStatus =
			value.artistParticipationStatus ?? 'Everything';
		this.childTags = value.childTags ?? false;
		this.artistFilters.childVoicebanks = value.childVoicebanks ?? false;
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 20;
		this.playlistMode = value.playlistMode ?? false;
		this.query = value.query ?? '';
		this.sort = value.sort ?? '';
		this.songType = value.songType ?? SongType.Unspecified;
		this.tagIds = ([] as number[]).concat(value.tagId ?? []);
	}

	validateLocationState = (data: any): data is SongListRouteParams => {
		return validate(data);
	};

	@computed get queryParams(): SongListGetSongsQueryParams {
		return {
			listId: this.listId,
			query: this.query,
			songTypes:
				this.songType !== SongType.Unspecified ? [this.songType] : undefined,
			tagIds: this.tagIds,
			childTags: this.childTags,
			artistIds: this.artistFilters.artistIds,
			artistParticipationStatus: this.artistFilters.artistParticipationStatus,
			childVoicebanks: this.artistFilters.childVoicebanks,
			advancedFilters: this.advancedFilters.filters,
			sort: this.sort,
		};
	}

	private loadResults = (
		pagingProperties: PagingProperties,
	): Promise<PartialFindResultContract<SongInListContract>> => {
		if (this.playlistMode) {
			return Promise.resolve({ items: [], totalCount: 0 });
		} else {
			return this.songListRepo.getSongs({
				fields: [
					SongOptionalField.AdditionalNames,
					SongOptionalField.MainPicture,
					...(this.showTags ? [SongOptionalField.Tags] : []),
				],
				lang: this.values.languagePreference,
				paging: pagingProperties,
				pvServices: undefined,
				queryParams: this.queryParams,
			});
		}
	};

	@action updateResults = async (clearResults: boolean): Promise<void> => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;
		this.loading = true;

		const pagingProperties = this.paging.getPagingProperties(clearResults);

		const result = await this.loadResults(pagingProperties);

		for (const item of result.items) {
			const song = item.song;
			const songAny: any = song;

			if (song.pvServices && song.pvServices !== 'Nothing') {
				songAny.previewStore = new SongWithPreviewStore(
					this.songRepo,
					this.userRepo,
					song.id,
					song.pvServices,
				);
				// TODO: songAny.previewViewModel.ratingComplete = ui.showThankYouForRatingMessage;
			} else {
				songAny.previewStore = undefined;
			}
		}

		this.pauseNotifications = false;

		runInAction(() => {
			if (pagingProperties.getTotalCount)
				this.paging.totalItems = result.totalCount;

			this.page = result.items;
			this.loading = false;
		});
	};

	updateResultsWithTotalCount = (): Promise<void> => {
		return this.updateResults(true);
	};

	updateResultsWithoutTotalCount = (): Promise<void> => {
		return this.updateResults(false);
	};

	onLocationStateChange = (
		event: StateChangeEvent<SongListRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
