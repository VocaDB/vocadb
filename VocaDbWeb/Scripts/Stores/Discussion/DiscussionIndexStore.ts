import DiscussionFolderContract from '@DataContracts/Discussion/DiscussionFolderContract';
import DiscussionTopicContract from '@DataContracts/Discussion/DiscussionTopicContract';
import LoginManager from '@Models/LoginManager';
import DiscussionRepository from '@Repositories/DiscussionRepository';
import IStoreWithPaging from '@Stores/IStoreWithPaging';
import ServerSidePagingStore from '@Stores/ServerSidePagingStore';
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

import DiscussionTopicEditStore from './DiscussionTopicEditStore';
import DiscussionTopicStore from './DiscussionTopicStore';

interface DiscussionIndexRouteParams {
	page?: number;
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<DiscussionIndexRouteParams> = require('@Stores/Discussion/DiscussionIndexRouteParams.schema');
const validate = ajv.compile(schema);

export default class DiscussionIndexStore
	implements IStoreWithPaging<DiscussionIndexRouteParams> {
	@observable public folders: DiscussionFolderContract[] = [];
	@observable public newTopic: DiscussionTopicEditStore;
	public readonly paging = new ServerSidePagingStore(30); // Paging store
	@observable public recentTopics: DiscussionTopicContract[] = [];
	@observable public selectedFolder?: DiscussionFolderContract = undefined;
	@observable public selectedTopic?: DiscussionTopicStore = undefined;
	@observable public showCreateNewTopic: boolean = false;
	@observable public topics: DiscussionTopicContract[] = [];
	private pauseNotifications = false;

	public constructor(
		public readonly loginManager: LoginManager,
		private readonly discussionRepo: DiscussionRepository,
		private readonly canDeleteAllComments: boolean,
	) {
		makeObservable(this);

		this.newTopic = new DiscussionTopicEditStore(loginManager, this.folders);

		discussionRepo.getFolders({}).then((folders) => {
			runInAction(() => {
				this.folders = folders;
			});
		});

		discussionRepo.getTopics({}).then((result) => {
			runInAction(() => {
				this.recentTopics = result.items;
			});
		});

		reaction(
			() => this.selectedFolder,
			() => {
				this.showCreateNewTopic = false;
				this.selectedTopic = undefined;
			},
		);
	}

	private getFolder = (
		folderId: number,
	): DiscussionFolderContract | undefined => {
		return _.find(this.folders, (f) => f.id === folderId);
	};

	@action public selectFolderById = (folderId: number): void => {
		this.selectedFolder = this.getFolder(folderId);
	};

	@action private loadTopics = (
		folder?: DiscussionFolderContract,
	): Promise<void> => {
		if (!folder) {
			this.topics = [];

			return Promise.resolve();
		}

		const paging = this.paging.getPagingProperties(true);
		return this.discussionRepo
			.getTopicsForFolder({ folderId: folder.id, paging: paging })
			.then((result) => {
				runInAction(() => {
					this.topics = result.items;

					if (paging.getTotalCount) this.paging.totalItems = result.totalCount;
				});
			});
	};

	private loadTopicsForCurrentFolder = (): Promise<void> => {
		return this.loadTopics(this.selectedFolder);
	};

	private canDeleteTopic = (topic: DiscussionTopicContract): boolean => {
		return (
			this.canDeleteAllComments ||
			topic.author?.id === this.loginManager.loggedUserId
		);
	};

	private canEditTopic = (topic: DiscussionTopicContract): boolean => {
		return (
			this.canDeleteAllComments ||
			topic.author?.id === this.loginManager.loggedUserId
		);
	};

	public selectTopicById = (topicId?: number): void => {
		if (!topicId) {
			this.loadTopics(this.selectedFolder).then(() => {
				runInAction(() => {
					this.selectedTopic = undefined;
				});
			});
			return;
		}

		this.discussionRepo.getTopic({ topicId: topicId }).then((contract) => {
			contract.canBeDeleted = this.canDeleteTopic(contract);
			contract.canBeEdited = this.canEditTopic(contract);

			this.selectFolderById(contract.folderId);
			runInAction(() => {
				this.selectedTopic = new DiscussionTopicStore(
					this.loginManager,
					this.discussionRepo,
					this.canDeleteAllComments,
					contract,
					this.folders,
				);
			});
		});
	};

	public createNewTopic = (): Promise<DiscussionTopicContract> => {
		const folder = this.selectedFolder;
		return this.discussionRepo
			.createTopic({
				folderId: folder!.id,
				contract: this.newTopic.toContract(),
			})
			.then((topic) => {
				topic.canBeDeleted = false;
				runInAction(() => {
					this.newTopic = new DiscussionTopicEditStore(
						this.loginManager,
						this.folders,
					);
					this.showCreateNewTopic = false;
					this.topics.unshift(topic);
				});
				return topic;
			});
	};

	public deleteTopic = (topic: DiscussionTopicContract): Promise<void> => {
		return this.discussionRepo.deleteTopic({ topicId: topic.id });
	};

	public popState = false;

	public readonly clearResultsByQueryKeys: (keyof DiscussionIndexRouteParams)[] = [];

	@computed.struct public get routeParams(): DiscussionIndexRouteParams {
		return {
			page: this.paging.page,
		};
	}
	public set routeParams(value: DiscussionIndexRouteParams) {
		this.paging.page = value.page ?? 1;
	}

	public validateRouteParams = (
		data: any,
	): data is DiscussionIndexRouteParams => validate(data);

	public updateResults = (clearResults: boolean): void => {
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		this.loadTopicsForCurrentFolder().then(() => {
			this.pauseNotifications = false;
		});
	};
}
