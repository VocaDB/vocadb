
namespace vdb.dataContracts {

	// Matches ReleaseEventForApiContract
	export interface ReleaseEventContract {

		date?: string;

		id: number;

		name: string;

		series?: models.IEntryWithIdAndName;

		songList?: SongListBaseContract;

		webLinks: WebLinkContract[];

	}

}