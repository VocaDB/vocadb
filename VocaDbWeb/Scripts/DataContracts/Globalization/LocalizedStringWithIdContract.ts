import LocalizedStringContract from './LocalizedStringContract';

//namespace vdb.dataContracts.globalization {

	export default interface LocalizedStringWithIdContract extends LocalizedStringContract {
		id: number;
	}

//}