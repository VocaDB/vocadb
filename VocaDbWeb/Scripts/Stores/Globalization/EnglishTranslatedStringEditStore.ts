import EnglishTranslatedStringContract from '@DataContracts/Globalization/EnglishTranslatedStringContract';
import { makeObservable, observable } from 'mobx';

export default class EnglishTranslatedStringEditStore {
	@observable public english: string;
	@observable public original: string;
	@observable public showTranslation: boolean;

	public constructor(contract: EnglishTranslatedStringContract) {
		makeObservable(this);

		this.english = contract.english;
		this.original = contract.original;
		this.showTranslation = contract.english !== '';
	}

	public toContract = (): EnglishTranslatedStringContract => {
		const contract: EnglishTranslatedStringContract = {
			english: this.english,
			original: this.original,
		};

		return contract;
	};
}
