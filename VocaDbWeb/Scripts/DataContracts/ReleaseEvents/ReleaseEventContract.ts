
namespace vdb.dataContracts {

	// Matches ReleaseEventForApiContract
	export interface ReleaseEventContract {

		date?: string;

		defaultNameLanguage: string;

		id: number;

		mainPicture?: EntryThumbContract;

		name: string;

		names?: globalization.LocalizedStringWithIdContract[];

		series?: models.IEntryWithIdAndName;

		songList?: SongListBaseContract;

		webLinks: WebLinkContract[];

	}

}