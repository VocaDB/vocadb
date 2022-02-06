import CommentContract from '@DataContracts/CommentContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import SongListContract from '@DataContracts/Song/SongListContract';
import LoginManager from '@Models/LoginManager';
import UserEventRelationshipType from '@Models/Users/UserEventRelationshipType';
import AdminRepository from '@Repositories/AdminRepository';
import TagRepository from '@Repositories/TagRepository';
import UserRepository from '@Repositories/UserRepository';
import GlobalValues from '@Shared/GlobalValues';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import Ajv, { JSONSchemaType } from 'ajv';
import { Options } from 'highcharts';
import { makeObservable, observable, reaction, runInAction } from 'mobx';

import HighchartsHelper from '../../Helpers/HighchartsHelper';
import DeleteEntryStore from '../DeleteEntryStore';
import EditableCommentsStore from '../EditableCommentsStore';
import IStoreWithUpdateResults from '../IStoreWithUpdateResults';
import SongListsBaseStore, {
	SongListSortRule,
} from '../SongList/SongListsBaseStore';
import AlbumCollectionStore from './AlbumCollectionStore';
import FollowedArtistsStore from './FollowedArtistsStore';
import RatedSongsSearchStore from './RatedSongsSearchStore';

interface UserSongListsRouteParams {
	filter?: string;
	sort?: SongListSortRule;
	tagId?: number | number[];
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<UserSongListsRouteParams> = require('./UserSongListsRouteParams.schema');
const validate = ajv.compile(schema);

export class UserSongListsStore
	extends SongListsBaseStore
	implements IStoreWithUpdateResults<UserSongListsRouteParams> {
	public constructor(
		values: GlobalValues,
		private readonly userId: number,
		private readonly userRepo: UserRepository,
		tagRepo: TagRepository,
	) {
		super(values, tagRepo, [], true);
	}

	public loadMoreItems = (): Promise<
		PartialFindResultContract<SongListContract>
	> => {
		return this.userRepo.getSongLists({
			userId: this.userId,
			query: this.query,
			paging: { start: this.start, maxEntries: 50, getTotalCount: true },
			tagIds: this.tagFilters.tagIds,
			sort: this.sort,
			fields: this.fields,
		});
	};

	public popState = false;

	public clearResultsByQueryKeys: (keyof UserSongListsRouteParams)[] = [
		'filter',
		'sort',
		'tagId',
	];

	public validateRouteParams = (data: any): data is UserSongListsRouteParams =>
		validate(data);

	private pauseNotifications = false;

	public updateResults = (clearResults: boolean): void => {
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		this.clear().then(() => {
			this.pauseNotifications = false;
		});
	};
}

export class SfsCheckStore {
	@observable public html?: string;
	@observable public dialogVisible = false;

	public constructor(
		private readonly lastLoginAddress: string,
		private readonly adminRepo: AdminRepository,
	) {
		makeObservable(this);
	}

	public checkSFS = async (): Promise<void> => {
		const html = await this.adminRepo.checkSFS({ ip: this.lastLoginAddress });

		runInAction(() => {
			this.html = html;
			this.dialogVisible = true;
		});
	};
}

export default class UserDetailsStore {
	public readonly comments: EditableCommentsStore;
	private eventsLoaded = false;
	@observable public events: ReleaseEventContract[] = [];
	@observable public eventsType =
		UserEventRelationshipType[UserEventRelationshipType.Attending];

	public readonly limitedUserStore = new DeleteEntryStore((notes) => {
		return this.httpClient
			.post<void>(
				this.urlMapper.mapRelative(`/api/users/${this.userId}/status-limited`),
				{ reason: notes, createReport: true },
			)
			.then(() => {
				window.location.reload();
			});
	});

	public readonly reportUserStore = new DeleteEntryStore((notes) => {
		return this.httpClient
			.post<boolean>(
				this.urlMapper.mapRelative(`/api/users/${this.userId}/reports`),
				{ reason: notes, reportType: 'Spamming' },
			)
			.then(() => {
				// TODO: showSuccessMessage
				runInAction(() => {
					this.reportUserStore.notes = '';
				});
			});
	}, true);

	@observable public ratingsByGenreChart?: Options;
	public readonly songLists: UserSongListsStore;
	public readonly sfsCheckDialog: SfsCheckStore;

	public constructor(
		values: GlobalValues,
		loginManager: LoginManager,
		private readonly userId: number,
		private readonly lastLoginAddress: string,
		canEditAllComments: boolean,
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
		private readonly userRepo: UserRepository,
		private readonly adminRepo: AdminRepository,
		tagRepo: TagRepository,
		public readonly followedArtistsStore: FollowedArtistsStore,
		public readonly albumCollectionStore: AlbumCollectionStore,
		public readonly ratedSongsStore: RatedSongsSearchStore,
		latestComments: CommentContract[],
	) {
		makeObservable(this);

		this.comments = new EditableCommentsStore(
			loginManager,
			userRepo,
			userId,
			userId === loginManager.loggedUserId,
			canEditAllComments,
			false,
			latestComments,
			true,
		);
		this.songLists = new UserSongListsStore(values, userId, userRepo, tagRepo);
		this.sfsCheckDialog = new SfsCheckStore(lastLoginAddress, this.adminRepo);

		// TODO: Use useStoreWithUpdateResults.
		reaction(() => this.eventsType, this.loadEvents);
	}

	public addBan = ({ name }: { name: string }): Promise<boolean> => {
		return this.adminRepo.addIpToBanList({
			rule: { address: this.lastLoginAddress, notes: name },
		});
	};

	public loadHighcharts = async (): Promise<void> => {
		const data = await this.userRepo.getRatingsByGenre({ userId: this.userId });

		runInAction(() => {
			this.ratingsByGenreChart = HighchartsHelper.simplePieChart(
				null!,
				'Songs' /* TODO: localize */,
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

	public initEvents = (): void => {
		if (this.eventsLoaded) return;

		this.loadEvents();
		this.eventsLoaded = true;
	};
}
