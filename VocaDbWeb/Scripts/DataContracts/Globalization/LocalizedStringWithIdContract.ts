import { LocalizedStringContract } from '@/DataContracts/Globalization/LocalizedStringContract';

export interface LocalizedStringWithIdContract extends LocalizedStringContract {
	id: number;
}
