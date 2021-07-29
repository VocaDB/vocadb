import DiscussionFolderContract from '@DataContracts/Discussion/DiscussionFolderContract';
import DiscussionTopicContract from '@DataContracts/Discussion/DiscussionTopicContract';
import LoginManager from '@Models/LoginManager';
import DiscussionRepository from '@Repositories/DiscussionRepository';
import ServerSidePagingStore from '@Stores/ServerSidePagingStore';
import _ from 'lodash';
import {
	action,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

import DiscussionTopicEditStore from './DiscussionTopicEditStore';
import DiscussionTopicStore from './DiscussionTopicStore';

export default class DiscussionIndexStore {
	@observable public folders: DiscussionFolderContract[] = [];
	@observable public newTopic: DiscussionTopicEditStore;
	public readonly paging = new ServerSidePagingStore(30); // Paging store
	@observable public recentTopics: DiscussionTopicContract[] = [];
	@observable public selectedFolder?: DiscussionFolderContract = undefined;
	@observable public selectedTopic?: DiscussionTopicStore = undefined;
	@observable public showCreateNewTopic: boolean = false;
	@observable public topics: DiscussionTopicContract[] = [];

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
			(folder) => {
				this.showCreateNewTopic = false;
				this.selectedTopic = undefined;
				this.paging.goToFirstPage();

				this.loadTopics(folder);
			},
		);

		reaction(() => this.paging.page, this.loadTopicsForCurrentFolder);
		reaction(() => this.paging.pageSize, this.loadTopicsForCurrentFolder);
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

	private loadTopicsForCurrentFolder = (): void => {
		this.loadTopics(this.selectedFolder);
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
}
