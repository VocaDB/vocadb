
namespace vdb.dataContracts {

	// Matches ReleaseEventForApiContract
	export interface ReleaseEventContract {

		category: string;

		date?: string;

		defaultNameLanguage: string;

		id: number;

		mainPicture?: EntryThumbContract;

		name: string;

		names?: globalization.LocalizedStringWithIdContract[];

		pvs?: pvs.PVContract[];

		series?: EventSeriesContract;

		songList?: SongListBaseContract;

		webLinks: WebLinkContract[];

	}

}