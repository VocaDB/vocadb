
module vdb.dataContracts {
	
	export interface CommentContract {

		author: UserWithIconContract;

		authorName: string;

		canBeDeleted: boolean;

		created: Date;

		id: number;

		message: string;

	}

}