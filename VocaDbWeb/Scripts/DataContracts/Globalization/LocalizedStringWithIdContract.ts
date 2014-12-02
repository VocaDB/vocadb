
module vdb.dataContracts.globalization {

	import cls = vdb.models;

	export interface LocalizedStringWithIdContract {

		id: number;
		language: string;
		value: string;


	}

}