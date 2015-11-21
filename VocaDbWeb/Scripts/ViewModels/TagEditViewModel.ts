/// <reference path="../typings/knockout/knockout.d.ts" />

module vdb.viewModels {

	import dc = vdb.dataContracts;

	export class TagEditViewModel {

		constructor(contract: dc.TagApiContract) {

			this.aliasedTo = ko.observable(contract.aliasedTo);
			this.categoryName = ko.observable(contract.categoryName);
			this.description = ko.observable(contract.description);
			this.id = contract.id;
			this.name = contract.name;
			this.parent = ko.observable(contract.parent);

			this.validationError_needDescription = ko.computed(() => !this.description());

			this.aliasedToName = ko.computed(() => this.aliasedTo() ? this.aliasedTo().name : null);
			this.parentName = ko.computed(() => this.parent() ? this.parent().name : null);

			this.hasValidationErrors = ko.computed(() =>
				this.validationError_needDescription()
			);

		}

		public aliasedTo: KnockoutObservable<dc.TagBaseContract>;
		public aliasedToName: KnockoutComputed<string>;
		public categoryName: KnockoutObservable<string>;
		public description: KnockoutObservable<string>;
		public hasValidationErrors: KnockoutComputed<boolean>;
		private id: number;
		public name: string;
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