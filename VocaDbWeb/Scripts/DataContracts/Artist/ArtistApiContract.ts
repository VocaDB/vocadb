
module vdb.dataContracts {

	export interface ArtistApiContract extends EntryWithTagUsagesContract {

		additionalNames: string;

		id: number;

		mainPicture: EntryThumbContract;

		name: string;

	}

} 