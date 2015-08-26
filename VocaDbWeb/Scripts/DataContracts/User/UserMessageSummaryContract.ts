/// <reference path="UserWithIconContract.ts" />

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

        read: boolean;

        receiver: vdb.dataContracts.UserWithIconContract;

        sender?: vdb.dataContracts.UserWithIconContract;

        subject: string;

    }

}