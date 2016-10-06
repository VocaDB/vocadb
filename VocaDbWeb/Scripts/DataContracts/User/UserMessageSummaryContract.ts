
module vdb.dataContracts {

    export interface UserMessageSummaryContract {

		/* 
			Message body, optional field. 
			This field is encoded with Markdown.
		*/
		body?: string;

        createdFormatted: string;

        highPriority: boolean;

		id: number;

		inbox: string;

        read: boolean;

        receiver: vdb.dataContracts.user.UserApiContract;

        sender?: vdb.dataContracts.user.UserApiContract;

        subject: string;

    }

}