import { CommentContract } from '@/DataContracts/CommentContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { SongListContract } from '@/DataContracts/Song/SongListContract';
import { HighchartsHelper } from '@/Helpers/HighchartsHelper';
import { LoginManager } from '@/Models/LoginManager';
import { UserEventRelationshipType } from '@/Models/Users/UserEventRelationshipType';
import { AdminRepository } from '@/Repositories/AdminRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import { EditableCommentsStore } from '@/Stores/EditableCommentsStore';
import {
	SongListsBaseStore,
	SongListSortRule,
} from '@/Stores/SongList/SongListsBaseStore';
import { AlbumCollectionStore } from '@/Stores/User/AlbumCollectionStore';
import { FollowedArtistsStore } from '@/Stores/User/FollowedArtistsStore';
import { RatedSongsSearchStore } from '@/Stores/User/RatedSongsSearchStore';
import {
	includesAny,
	StateChangeEvent,
	LocationStateStore,
} from '@/route-sphere';
import Ajv from 'ajv';
import type { Options } from 'highcharts';
import { makeObservable, observable, reaction, runInAction } from 'mobx';

import schema from './UserSongListsRouteParams.schema.json';

interface UserSongListsRouteParams {
	filter?: string;
	sort?: SongListSortRule;
	tagId?: number | number[];
}

const clearResultsByQueryKeys: (keyof UserSongListsRouteParams)[] = [
	'filter',
	'sort',
	'tagId',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<UserSongListsRouteParams>(schema);

export class UserSongListsStore
	extends SongListsBaseStore
	implements LocationStateStore<UserSongListsRouteParams>
{
	constructor(
		values: GlobalValues,
		private readonly userId: number,
		private readonly userRepo: UserRepository,
		tagRepo: TagRepository,
	) {
		super(values, tagRepo, [], true);
	}

	loadMoreItems = (): Promise<PartialFindResultContract<SongListContract>> => {
		return this.userRepo.getSongLists({
			userId: this.userId,
			query: this.query,
			paging: { start: this.start, maxEntries: 50, getTotalCount: true },
			tagIds: this.tagFilters.tagIds,
			sort: this.sort,
			fields: this.fields,
		});
	};

	validateLocationState = (data: any): data is UserSongListsRouteParams => {
		return validate(data);
	};

	private pauseNotifications = false;

	updateResults = async (clearResults: boolean): Promise<void> => {
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		await this.clear();

		this.pauseNotifications = false;
	};

	onLocationStateChange = (
		event: StateChangeEvent<UserSongListsRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		this.updateResults(clearResults);
	};
}

export class SfsCheckStore {
	@observable html?: string;
	@observable dialogVisible = false;

	constructor(
		private readonly lastLoginAddress: string,
		private readonly adminRepo: AdminRepository,
	) {
		makeObservable(this);
	}

	checkSFS = async (): Promise<void> => {
		const html = await this.adminRepo.checkSFS({ ip: this.lastLoginAddress });

		runInAction(() => {
			this.html = html;
			this.dialogVisible = true;
		});
	};
}

export class UserDetailsStore {
	readonly comments: EditableCommentsStore;
	private eventsLoaded = false;
	@observable events: ReleaseEventContract[] = [];
	@observable eventsType =
		UserEventRelationshipType[UserEventRelationshipType.Attending];
	readonly limitedUserStore: DeleteEntryStore;
	readonly reportUserStore: DeleteEntryStore;
	@observable ratingsByGenreChart?: Options;
	readonly songLists: UserSongListsStore;
	readonly sfsCheckDialog: SfsCheckStore;

	constructor(
		values: GlobalValues,
		loginManager: LoginManager,
		private readonly userId: number,
		private readonly lastLoginAddress: string,
		canEditAllComments: boolean,
		private readonly userRepo: UserRepository,
		private readonly adminRepo: AdminRepository,
		tagRepo: TagRepository,
		readonly followedArtistsStore: FollowedArtistsStore,
		readonly albumCollectionStore: AlbumCollectionStore,
		readonly ratedSongsStore: RatedSongsSearchStore,
		latestComments: CommentContract[],
		commentsLocked: boolean,
	) {
		makeObservable(this);

		this.limitedUserStore = new DeleteEntryStore((notes) =>
			userRepo.postStatusLimited({ id: userId, notes: notes }),
		);

		this.reportUserStore = new DeleteEntryStore(
			(notes) =>
				userRepo.postReport({
					id: userId,
					notes: notes,
				}),
			true,
		);

		this.comments = new EditableCommentsStore(
			loginManager,
			userRepo,
			userId,
			userId === loginManager.loggedUserId,
			canEditAllComments,
			false,
			latestComments,
			true,
			commentsLocked,
		);
		this.songLists = new UserSongListsStore(values, userId, userRepo, tagRepo);
		this.sfsCheckDialog = new SfsCheckStore(lastLoginAddress, this.adminRepo);

		// TODO: Use useStoreWithUpdateResults.
		reaction(() => this.eventsType, this.loadEvents);
	}

	addBan = ({ name }: { name: string }): Promise<boolean> => {
		return this.adminRepo.addIpToBanList({
			rule: { address: this.lastLoginAddress, notes: name },
		});
	};

	loadHighcharts = async (): Promise<void> => {
		const data = await this.userRepo.getRatingsByGenre({ userId: this.userId });

		runInAction(() => {
			this.ratingsByGenreChart = HighchartsHelper.simplePieChart(
				null!,
				'Songs' /* LOC */,
				data,
			);
		});
	};

	private loadEvents = (): Promise<void> => {
		return this.userRepo
			.getEvents({
				userId: this.userId,
				relationshipType:
					UserEventRelationshipType[
						this.eventsType as keyof typeof UserEventRelationshipType
					],
			})
			.then((events) =>
				runInAction(() => {
					this.events = events;
				}),
			);
	};

	initEvents = (): void => {
		if (this.eventsLoaded) return;

		this.loadEvents();
		this.eventsLoaded = true;
	};
}
