import { DiscussionFolderContract } from '@/DataContracts/Discussion/DiscussionFolderContract';
import type { DiscussionTopicContract } from '@/DataContracts/Discussion/DiscussionTopicContract';
import { LoginManager } from '@/Models/LoginManager';
import { DiscussionRepository } from '@/Repositories/DiscussionRepository';
import { DiscussionTopicEditStore } from '@/Stores/Discussion/DiscussionTopicEditStore';
import { EditableCommentsStore } from '@/Stores/EditableCommentsStore';
import { LocationStateStore } from '@/route-sphere';
import Ajv from 'ajv';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

import schema from './DiscussionTopicRouteParams.schema.json';

interface DiscussionTopicRouteParams {
	page?: number;
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<DiscussionTopicRouteParams>(schema);

export class DiscussionTopicStore
	implements LocationStateStore<DiscussionTopicRouteParams> {
	@observable comments: EditableCommentsStore;
	@observable contract: DiscussionTopicContract;
	@observable editStore?: DiscussionTopicEditStore = undefined;

	constructor(
		private readonly loginManager: LoginManager,
		private readonly discussionRepo: DiscussionRepository,
		canDeleteAllComments: boolean,
		contract: DiscussionTopicContract,
		private readonly folders: DiscussionFolderContract[],
	) {
		makeObservable(this);

		this.contract = contract;

		this.comments = new EditableCommentsStore(
			loginManager,
			discussionRepo,
			contract.id,
			canDeleteAllComments,
			canDeleteAllComments,
			true,
			contract.comments,
		);
	}

	@computed get isBeingEdited(): boolean {
		return !!this.editStore;
	}

	@action beginEditTopic = (): void => {
		this.editStore = new DiscussionTopicEditStore(
			this.loginManager,
			this.folders,
			this.contract,
		);
	};

	@action cancelEdit = (): void => {
		this.editStore = undefined;
	};

	saveEditedTopic = (): void => {
		if (!this.isBeingEdited) return;

		const editedContract = this.editStore!.toContract();

		this.discussionRepo
			.updateTopic({
				topicId: this.contract.id,
				contract: editedContract,
			})
			.then(() => {
				editedContract.id = this.contract.id;
				editedContract.created = this.contract.created;
				editedContract.canBeDeleted = this.contract.canBeDeleted;
				editedContract.canBeEdited = this.contract.canBeEdited;

				runInAction(() => {
					this.contract = editedContract;
					this.editStore = undefined;
				});
			});
	};

	@computed.struct get locationState(): DiscussionTopicRouteParams {
		return {
			page: this.comments.paging.page,
		};
	}
	set locationState(value: DiscussionTopicRouteParams) {
		this.comments.paging.page = value.page ?? 1;
	}

	validateLocationState = (data: any): data is DiscussionTopicRouteParams => {
		return validate(data);
	};
}
