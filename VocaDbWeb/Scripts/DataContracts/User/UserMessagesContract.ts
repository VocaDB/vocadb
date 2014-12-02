/// <reference path="UserMessageSummaryContract.ts" />

module vdb.dataContracts {

    export interface UserMessagesContract {

        // Received messages, includes notifications.
        receivedMessages: UserMessageSummaryContract[];

        sentMessages: UserMessageSummaryContract[];

    }

}