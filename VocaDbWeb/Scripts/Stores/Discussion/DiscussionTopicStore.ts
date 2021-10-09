import DiscussionFolderContract from '@DataContracts/Discussion/DiscussionFolderContract';
import DiscussionTopicContract from '@DataContracts/Discussion/DiscussionTopicContract';
import LoginManager from '@Models/LoginManager';
import DiscussionRepository from '@Repositories/DiscussionRepository';
import EditableCommentsStore from '@Stores/EditableCommentsStore';
import IStoreWithRouteParams from '@Stores/IStoreWithRouteParams';
import Ajv, { JSONSchemaType } from 'ajv';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

import DiscussionTopicEditStore from './DiscussionTopicEditStore';

interface DiscussionTopicRouteParams {
	page?: number;
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<DiscussionTopicRouteParams> = require('./DiscussionTopicRouteParams.schema');
const validate = ajv.compile(schema);

export default class DiscussionTopicStore
	implements IStoreWithRouteParams<DiscussionTopicRouteParams> {
	@observable public comments: EditableCommentsStore;
	@observable public contract: DiscussionTopicContract;
	@observable public editStore?: DiscussionTopicEditStore = undefined;

	public constructor(
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

	@computed public get isBeingEdited(): boolean {
		return !!this.editStore;
	}

	@action public beginEditTopic = (): void => {
		this.editStore = new DiscussionTopicEditStore(
			this.loginManager,
			this.folders,
			this.contract,
		);
	};

	@action public cancelEdit = (): void => {
		this.editStore = undefined;
	};

	public saveEditedTopic = (): void => {
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

	public popState = false;

	@computed.struct public get routeParams(): DiscussionTopicRouteParams {
		return {
			page: this.comments.paging.page,
		};
	}
	public set routeParams(value: DiscussionTopicRouteParams) {
		this.comments.paging.page = value.page ?? 1;
	}

	public validateRouteParams = (
		data: any,
	): data is DiscussionTopicRouteParams => validate(data);
}
