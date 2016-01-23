/// <reference path="../typings/knockout/knockout.d.ts" />

module vdb.viewModels {

	import dc = vdb.dataContracts;

	export class TagEditViewModel {

		constructor(
			userRepository: vdb.repositories.UserRepository,
			contract: dc.TagApiContract) {

			this.aliasedTo = ko.observable(contract.aliasedTo);
			this.categoryName = ko.observable(contract.categoryName);
			this.defaultNameLanguage = ko.observable(contract.defaultNameLanguage);
			this.description = new globalization.EnglishTranslatedStringEditViewModel(contract.translatedDescription);
			this.id = contract.id;
			this.names = globalization.NamesEditViewModel.fromContracts(contract.names);
			this.parent = ko.observable(contract.parent);

			this.validationError_needDescription = ko.computed(() => !this.description.original());

			this.aliasedToName = ko.computed(() => this.aliasedTo() ? this.aliasedTo().name : null);
			this.parentName = ko.computed(() => this.parent() ? this.parent().name : null);

			this.hasValidationErrors = ko.computed(() =>
				this.validationError_needDescription()
			);

			window.setInterval(() => userRepository.refreshEntryEdit(models.EntryType.Tag, contract.id), 10000);

		}

		public aliasedTo: KnockoutObservable<dc.TagBaseContract>;
		public aliasedToName: KnockoutComputed<string>;
		public categoryName: KnockoutObservable<string>;
		public defaultNameLanguage: KnockoutObservable<string>;
		public description: globalization.EnglishTranslatedStringEditViewModel;
		public hasValidationErrors: KnockoutComputed<boolean>;
		private id: number;
		public names: globalization.NamesEditViewModel;
		public parent: KnockoutObservable<dc.TagBaseContract>;
		public parentName: KnockoutComputed<string>;
		public submitting = ko.observable(false);
		public validationExpanded = ko.observable(false);
		public validationError_needDescription: KnockoutComputed<boolean>;

		denySelf = (tag: dc.TagBaseContract) => (tag && tag.id !== this.id);

		public submit = () => {
			this.submitting(true);
			return true;
		}

	}

}