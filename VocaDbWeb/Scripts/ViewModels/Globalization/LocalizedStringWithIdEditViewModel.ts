
module vdb.viewModels.globalization {

	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class LocalizedStringWithIdEditViewModel {

		public id: number;
		public language = ko.observable(cls.globalization.ContentLanguageSelection.Unspecified); //: KnockoutObservable<cls.globalization.ContentLanguageSelection>;
		public languageStr = vdb.helpers.KnockoutHelper.stringEnum(this.language, cls.globalization.ContentLanguageSelection);
		public value: KnockoutObservable<string>;

		public static fromContract(contract: dc.globalization.LocalizedStringWithIdContract) {
			return new LocalizedStringWithIdEditViewModel(cls.globalization.ContentLanguageSelection[contract.language], contract.value, contract.id);
		}

		constructor(language: cls.globalization.ContentLanguageSelection = cls.globalization.ContentLanguageSelection.Unspecified, value: string = null, id: number = 0) {

			this.language(language);
			this.value = ko.observable(value);
			this.id = id;

		}

	}

}