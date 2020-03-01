import EnglishTranslatedStringContract from '../../DataContracts/Globalization/EnglishTranslatedStringContract';

//module vdb.viewModels.globalization {
	
	export default class EnglishTranslatedStringEditViewModel {
		
		constructor(contract: EnglishTranslatedStringContract) {
			this.english = ko.observable(contract ? contract.english : null);
			this.original = ko.observable(contract ? contract.original : null);
			this.showTranslation = ko.observable(contract && contract.english !== "");
		}

		public english: KnockoutObservable<string>;

		public original: KnockoutObservable<string>;

		public showTranslation: KnockoutObservable<boolean>;

		public toContract = () => {

			var contract: EnglishTranslatedStringContract = {
				english: this.english(),
				original: this.original()
			};

			return contract;

		}

	}

//} 