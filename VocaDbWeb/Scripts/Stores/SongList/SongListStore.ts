import CommentContract from '@DataContracts/CommentContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import SongInListContract from '@DataContracts/Song/SongInListContract';
import TagSelectionContract from '@DataContracts/Tag/TagSelectionContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import {
	SongOptionalField,
	SongOptionalFields,
} from '@Models/EntryOptionalFields';
import EntryType from '@Models/EntryType';
import LoginManager from '@Models/LoginManager';
import PVServiceIcons from '@Models/PVServiceIcons';
import SongType from '@Models/Songs/SongType';
import ArtistRepository from '@Repositories/ArtistRepository';
import SongListRepository from '@Repositories/SongListRepository';
import SongRepository from '@Repositories/SongRepository';
import TagRepository from '@Repositories/TagRepository';
import UserRepository from '@Repositories/UserRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import GlobalValues from '@Shared/GlobalValues';
import UrlMapper from '@Shared/UrlMapper';
import EditableCommentsStore from '@Stores/EditableCommentsStore';
import IStoreWithPaging from '@Stores/IStoreWithPaging';
import PVPlayerStore from '@Stores/PVs/PVPlayerStore';
import PVPlayersFactory from '@Stores/PVs/PVPlayersFactory';
import AdvancedSearchFilter from '@Stores/Search/AdvancedSearchFilter';
import AdvancedSearchFilters from '@Stores/Search/AdvancedSearchFilters';
import ArtistFilters from '@Stores/Search/ArtistFilters';
import { ISongSearchItem } from '@Stores/Search/SongSearchStore';
import TagFilter from '@Stores/Search/TagFilter';
import TagFilters from '@Stores/Search/TagFilters';
import ServerSidePagingStore from '@Stores/ServerSidePagingStore';
import PlayListRepositoryForSongListAdapter, {
	ISongListStore,
} from '@Stores/Song/PlayList/PlayListRepositoryForSongListAdapter';
import PlayListStore from '@Stores/Song/PlayList/PlayListStore';
import SongWithPreviewStore from '@Stores/Song/SongWithPreviewStore';
import TagListStore from '@Stores/Tag/TagListStore';
import TagsEditStore from '@Stores/Tag/TagsEditStore';
import Ajv, { JSONSchemaType } from 'ajv';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

const loginManager = new LoginManager(vdb.values);

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
	songType?: string /* TODO: enum */;
	tagId?: number[];
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<SongListRouteParams> = require('./SongListRouteParams.schema');
const validate = ajv.compile(schema);

export default class SongListStore
	implements ISongListStore, IStoreWithPaging<SongListRouteParams> {
	public readonly advancedFilters = new AdvancedSearchFilters();
	public readonly artistFilters: ArtistFilters;
	public readonly comments: EditableCommentsStore;
	@observable public loading = true; // Currently loading for data
	@observable public page: (SongInListContract & {
		song: ISongSearchItem;
	})[] = []; // Current page of items
	public readonly paging = new ServerSidePagingStore(20); // Paging view model
	public pauseNotifications = false;
	@observable public playlistMode = false;
	public readonly playlistStore: PlayListStore;
	public readonly pvPlayerStore: PVPlayerStore;
	public readonly pvServiceIcons: PVServiceIcons;
	@observable public query = '';
	@observable public showAdvancedFilters = false;
	@observable public showTags = false;
	@observable public sort = '' /* TODO: enum */;
	@observable public songType = SongType[SongType.Unspecified] /* TODO: enum */;
	public readonly tagsEditStore: TagsEditStore;
	public readonly tagFilters: TagFilters;
	public readonly tagUsages: TagListStore;

	public constructor(
		private readonly values: GlobalValues,
		urlMapper: UrlMapper,
		private readonly songListRepo: SongListRepository,
		private readonly songRepo: SongRepository,
		tagRepo: TagRepository,
		private readonly userRepo: UserRepository,
		artistRepo: ArtistRepository,
		latestComments: CommentContract[],
		private readonly listId: number,
		tagUsages: TagUsageForApiContract[],
		pvPlayersFactory: PVPlayersFactory,
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
		this.pvPlayerStore = new PVPlayerStore(
			values,
			songRepo,
			userRepo,
			pvPlayersFactory,
		);
		const playListRepoAdapter = new PlayListRepositoryForSongListAdapter(
			songListRepo,
			listId,
			this,
		);
		this.playlistStore = new PlayListStore(
			values,
			urlMapper,
			playListRepoAdapter,
			this.pvPlayerStore,
		);
		this.pvServiceIcons = new PVServiceIcons(urlMapper);

		this.tagsEditStore = new TagsEditStore(
			{
				getTagSelections: (): Promise<TagSelectionContract[]> =>
					userRepo.getSongListTagSelections({ songListId: this.listId }),
				saveTagSelections: (tags): Promise<void> =>
					userRepo
						.updateSongListTags({ songListId: this.listId, tags: tags })
						.then(this.tagUsages.updateTagUsages),
			},
			EntryType.SongList,
		);

		this.tagFilters = new TagFilters(values, tagRepo);

		this.tagUsages = new TagListStore(tagUsages);

		reaction(() => this.showTags, this.updateResultsWithTotalCount);
	}

	@computed public get childTags(): boolean {
		return this.tagFilters.childTags;
	}
	public set childTags(value: boolean) {
		this.tagFilters.childTags = value;
	}

	@computed public get tags(): TagFilter[] {
		return this.tagFilters.tags;
	}
	public set tags(value: TagFilter[]) {
		this.tagFilters.tags = value;
	}

	@computed public get tagIds(): number[] {
		return _.map(this.tags, (t) => t.id);
	}
	public set tagIds(value: number[]) {
		// OPTIMIZE
		this.tagFilters.tags = [];
		this.tagFilters.addTags(value);
	}

	public mapTagUrl = (tagUsage: TagUsageForApiContract): string => {
		return EntryUrlMapper.details_tag(tagUsage.tag.id, tagUsage.tag.urlSlug);
	};

	public popState = false;

	public clearResultsByQueryKeys: (keyof SongListRouteParams)[] = [
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

	@computed.struct public get routeParams(): SongListRouteParams {
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
	public set routeParams(value: SongListRouteParams) {
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
		this.songType = value.songType ?? SongType[SongType.Unspecified];
		this.tagIds = ([] as number[]).concat(value.tagId ?? []);
	}

	public validateRouteParams = (data: any): data is SongListRouteParams =>
		validate(data);

	private loadResults = (
		pagingProperties: PagingProperties,
	): Promise<PartialFindResultContract<SongInListContract>> => {
		if (this.playlistMode) {
			this.playlistStore.updateResultsWithTotalCount();
			return Promise.resolve({ items: [], totalCount: 0 });
		} else {
			const fields = [
				SongOptionalField.AdditionalNames,
				SongOptionalField.ThumbUrl,
			];

			if (this.showTags) fields.push(SongOptionalField.Tags);

			return this.songListRepo.getSongs({
				listId: this.listId,
				query: this.query,
				songTypes:
					this.songType !== SongType[SongType.Unspecified]
						? this.songType
						: undefined,
				tagIds: this.tagIds,
				childTags: this.childTags,
				artistIds: this.artistFilters.artistIds,
				artistParticipationStatus: this.artistFilters.artistParticipationStatus,
				childVoicebanks: this.artistFilters.childVoicebanks,
				advancedFilters: this.advancedFilters.filters,
				pvServices: undefined,
				paging: pagingProperties,
				fields: new SongOptionalFields(fields),
				sort: this.sort,
				lang: this.values.languagePreference,
			});
		}
	};

	@action public updateResults = (clearResults: boolean): void => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;
		this.loading = true;

		const pagingProperties = this.paging.getPagingProperties(clearResults);

		this.loadResults(pagingProperties).then((result) => {
			_.each(result.items, (item) => {
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
			});

			this.pauseNotifications = false;

			runInAction(() => {
				if (pagingProperties.getTotalCount)
					this.paging.totalItems = result.totalCount;

				this.page = result.items;
				this.loading = false;
			});
		});
	};

	public updateResultsWithTotalCount = (): void => this.updateResults(true);
	public updateResultsWithoutTotalCount = (): void => this.updateResults(false);
}
