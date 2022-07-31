import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';

export interface ArchivedEntryPictureFileContract {
	author: ObjectRefContract;
	created: string;
	id: number;
	mime: string;
	name: string;
}
