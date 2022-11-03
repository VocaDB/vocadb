import { DiscussionFolderContract } from '@/DataContracts/Discussion/DiscussionFolderContract';
import { DiscussionTopicContract } from '@/DataContracts/Discussion/DiscussionTopicContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { LoginManager } from '@/Models/LoginManager';
import { makeObservable, observable } from 'mobx';

export class DiscussionTopicEditStore {
	readonly author: UserApiContract;
	@observable content = '';
	@observable folderId?: number = undefined;
	@observable locked = false;
	@observable name = '';

	constructor(
		loginManager: LoginManager,
		readonly folders: DiscussionFolderContract[],
		contract?: DiscussionTopicContract,
	) {
		makeObservable(this);

		this.author = { id: loginManager.loggedUserId, name: '' };

		if (contract) {
			this.author = contract.author;
			this.content = contract.content;
			this.folderId = contract.folderId;
			this.locked = contract.locked;
			this.name = contract.name;
		}
	}

	toContract = (): DiscussionTopicContract => {
		return JSON.parse(JSON.stringify(this)) as DiscussionTopicContract;
	};
}
