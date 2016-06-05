
module vdb.dataContracts {
	
	export interface CommentContract {

		author: user.UserApiContract;

		authorName?: string;

		canBeDeleted?: boolean;

		canBeEdited?: boolean;

		created?: Date;

		id?: number;

		message: string;

	}

}