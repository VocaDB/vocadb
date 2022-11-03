import { EnglishTranslatedStringContract } from '@/DataContracts/Globalization/EnglishTranslatedStringContract';
import { makeObservable, observable } from 'mobx';

export class EnglishTranslatedStringEditStore {
	@observable english: string;
	@observable original: string;
	@observable showTranslation: boolean;

	constructor(contract: EnglishTranslatedStringContract) {
		makeObservable(this);

		this.english = contract.english;
		this.original = contract.original;
		this.showTranslation = contract.english !== '';
	}

	toContract = (): EnglishTranslatedStringContract => {
		const contract: EnglishTranslatedStringContract = {
			english: this.english,
			original: this.original,
		};

		return contract;
	};
}
