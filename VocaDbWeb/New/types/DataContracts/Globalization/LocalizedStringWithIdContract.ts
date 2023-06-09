import { LocalizedStringContract } from '@/types/DataContracts/Globalization/LocalizedStringContract';

export interface LocalizedStringWithIdContract extends LocalizedStringContract {
	id: number;
}
