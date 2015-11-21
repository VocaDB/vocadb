
module vdb.dataContracts {

	export interface TagApiContract {

		aliasedTo: TagBaseContract;

		categoryName: string;

		description: string;

		id: number;

		mainPicture: EntryThumbContract;

		name: string;

		parent: TagBaseContract;

		status: string;

	}

} 