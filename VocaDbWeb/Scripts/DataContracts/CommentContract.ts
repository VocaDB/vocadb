
module vdb.dataContracts {
	
	export interface CommentContract {

		author: UserWithIconContract;

		authorName: string;

		created: Date;

		id: number;

		message: string;

	}

}