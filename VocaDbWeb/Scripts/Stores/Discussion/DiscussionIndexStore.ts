import type { DiscussionFolderContract } from '@/DataContracts/Discussion/DiscussionFolderContract';
import { DiscussionTopicContract } from '@/DataContracts/Discussion/DiscussionTopicContract';
import { LoginManager } from '@/Models/LoginManager';
import { DiscussionRepository } from '@/Repositories/DiscussionRepository';
import { DiscussionTopicEditStore } from '@/Stores/Discussion/DiscussionTopicEditStore';
import { DiscussionTopicStore } from '@/Stores/Discussion/DiscussionTopicStore';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import {
	includesAny,
	StateChangeEvent,
	LocationStateStore,
} from '@vocadb/route-sphere';
import Ajv from 'ajv';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

import schema from './DiscussionIndexRouteParams.schema.json';

interface DiscussionIndexRouteParams {
	page?: number;
}

const clearResultsByQueryKeys: (keyof DiscussionIndexRouteParams)[] = [];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<DiscussionIndexRouteParams>(schema);

export class DiscussionIndexStore
	implements LocationStateStore<DiscussionIndexRouteParams> {
	@observable folders: DiscussionFolderContract[] = [];
	@observable newTopic: DiscussionTopicEditStore;
	readonly paging = new ServerSidePagingStore(30); // Paging store
	@observable recentTopics: DiscussionTopicContract[] = [];
	@observable selectedFolder?: DiscussionFolderContract = undefined;
	@observable selectedTopic?: DiscussionTopicStore = undefined;
	@observable showCreateNewTopic: boolean = false;
	@observable topics: DiscussionTopicContract[] = [];

	constructor(
		readonly loginManager: LoginManager,
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
		return this.folders.find((f) => f.id === folderId);
	};

	@action selectFolderById = (folderId: number): void => {
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

	selectTopicById = (topicId?: number): void => {
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

	createNewTopic = (): Promise<DiscussionTopicContract> => {
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

	deleteTopic = (topic: DiscussionTopicContract): Promise<void> => {
		return this.discussionRepo.deleteTopic({ topicId: topic.id });
	};

	@computed.struct get locationState(): DiscussionIndexRouteParams {
		return {
			page: this.paging.page,
		};
	}
	set locationState(value: DiscussionIndexRouteParams) {
		this.paging.page = value.page ?? 1;
	}

	validateLocationState = (data: any): data is DiscussionIndexRouteParams => {
		return validate(data);
	};

	private pauseNotifications = false;

	updateResults = async (clearResults: boolean): Promise<void> => {
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		await this.loadTopicsForCurrentFolder();

		this.pauseNotifications = false;
	};

	onLocationStateChange = (
		event: StateChangeEvent<DiscussionIndexRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
