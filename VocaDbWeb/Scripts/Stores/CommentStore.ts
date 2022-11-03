import { CommentContract } from '@/DataContracts/CommentContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { action, makeObservable, observable } from 'mobx';

export class CommentStore {
	readonly author: UserApiContract;
	readonly authorName?: string;
	readonly created: string;
	@observable editedMessage?: string = undefined;
	id?: number;
	@observable message: string;

	constructor(
		contract: CommentContract,
		readonly canBeDeleted: boolean,
		readonly canBeEdited: boolean,
	) {
		makeObservable(this);

		this.author = contract.author;
		this.authorName = contract.authorName;
		this.created = contract.created;
		this.id = contract.id;
		this.message = contract.message;
	}

	@action beginEdit = (): void => {
		this.editedMessage = this.message;
	};

	@action saveChanges = (): void => {
		this.message = this.editedMessage!;
	};

	toContract = (): CommentContract => {
		return {
			created: this.created,
			id: this.id,
			message: this.message,
			author: this.author,
		};
	};
}
