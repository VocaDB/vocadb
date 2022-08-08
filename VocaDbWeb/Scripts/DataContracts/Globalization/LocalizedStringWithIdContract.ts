import LocalizedStringContract from '@/DataContracts/Globalization/LocalizedStringContract';

export default interface LocalizedStringWithIdContract
	extends LocalizedStringContract {
	id: number;
}
