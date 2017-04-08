
namespace vdb.dataContracts {

	// Matches ReleaseEventForApiContract
	export interface ReleaseEventContract {

		date?: string;

		id: number;

		mainPicture?: EntryThumbContract;

		name: string;

		series?: models.IEntryWithIdAndName;

		songList?: SongListBaseContract;

		webLinks: WebLinkContract[];

	}

}