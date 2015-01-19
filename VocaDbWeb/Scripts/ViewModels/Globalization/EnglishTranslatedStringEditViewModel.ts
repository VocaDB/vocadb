
module vdb.viewModels.globalization {
	
	import dc = vdb.dataContracts;

	export class EnglishTranslatedStringEditViewModel {
		
		constructor(contract: dc.globalization.EnglishTranslatedStringContract) {
			this.english = ko.observable(contract.english);
			this.original = ko.observable(contract.original);
			this.showTranslation = ko.observable(contract.english !== "");
		}

		public english: KnockoutObservable<string>;

		public original: KnockoutObservable<string>;

		public showTranslation: KnockoutObservable<boolean>;

		public toContract = () => {

			var contract: dc.globalization.EnglishTranslatedStringContract = {
				english: this.english(),
				original: this.original()
			};

			return contract;

		}

	}

} 