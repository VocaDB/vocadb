import EnglishTranslatedStringContract from '@DataContracts/Globalization/EnglishTranslatedStringContract';
import ko, { Observable } from 'knockout';

export default class EnglishTranslatedStringEditViewModel {
	constructor(contract: EnglishTranslatedStringContract) {
		this.english = ko.observable(contract ? contract.english : null!);
		this.original = ko.observable(contract ? contract.original : null!);
		this.showTranslation = ko.observable(contract && contract.english !== '');
	}

	public english: Observable<string>;

	public original: Observable<string>;

	public showTranslation: Observable<boolean>;

	public toContract = (): EnglishTranslatedStringContract => {
		var contract: EnglishTranslatedStringContract = {
			english: this.english(),
			original: this.original(),
		};

		return contract;
	};
}
