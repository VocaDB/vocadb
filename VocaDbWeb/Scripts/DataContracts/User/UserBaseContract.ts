import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';

export default interface UserBaseContract extends IEntryWithIdAndName {
	id: number;

	name?: string;
}
