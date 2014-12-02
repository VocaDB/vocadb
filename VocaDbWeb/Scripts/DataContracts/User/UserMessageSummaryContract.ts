/// <reference path="UserWithIconContract.ts" />

module vdb.dataContracts {

    export interface UserMessageSummaryContract {

        createdFormatted: string;

        highPriority: boolean;

        id: number;

        read: boolean;

        receiver: vdb.dataContracts.UserWithIconContract;

        sender?: vdb.dataContracts.UserWithIconContract;

        subject: string;

    }

}