import UserApiContract from './UserApiContract';

export default interface UserMessageSummaryContract {
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

	receiver: UserApiContract;

	sender?: UserApiContract;

	subject: string;
}
