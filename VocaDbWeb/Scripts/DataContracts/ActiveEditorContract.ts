import { EntryContract } from './EntryContract';
import { UserApiContract } from './User/UserApiContract';

// C# class: ActiveEditorForApiContract
export interface ActiveEditorContract {
	user: UserApiContract;
	time: string;
	entry: EntryContract;
}
