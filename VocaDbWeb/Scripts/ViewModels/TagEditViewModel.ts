/// <reference path="../typings/knockout/knockout.d.ts" />

module vdb.viewModels {

	import dc = vdb.dataContracts;

	export class TagEditViewModel {

		constructor(contract: dc.TagApiContract) {

			this.aliasedToName = ko.observable(contract.aliasedToName);
			this.categoryName = ko.observable(contract.categoryName);
			this.description = ko.observable(contract.description);
			this.name = contract.name;
			this.parentName = ko.observable(contract.parentName);

			this.validationError_needDescription = ko.computed(() => !this.description());

			this.aliasedTo = ko.computed({
				read: () => { return this.aliasedToName() ? { name: this.aliasedToName(), id: null } : null },
				write: (val: dc.TagBaseContract) => this.aliasedToName(val ? val.name : null)
			});

			this.parent = ko.computed({
				read: () => { return this.parentName() ? { name: this.parentName(), id: null } : null },
				write: (val: dc.TagBaseContract) => this.parentName(val ? val.name : null)
			});

			this.hasValidationErrors = ko.computed(() =>
				this.validationError_needDescription()
			);

		}

		public aliasedTo: KnockoutComputed<dc.TagBaseContract>;
		public aliasedToName: KnockoutObservable<string>;
		public categoryName: KnockoutObservable<string>;
		public description: KnockoutObservable<string>;
		public hasValidationErrors: KnockoutComputed<boolean>;
		public name: string;
		public parent: KnockoutComputed<dc.TagBaseContract>;
		public parentName: KnockoutObservable<string>;
		public submitting = ko.observable(false);
		public validationExpanded = ko.observable(false);
		public validationError_needDescription: KnockoutComputed<boolean>;

		denySelf = (tag: dc.TagBaseContract) => (tag && tag.name !== this.name);

		public submit = () => {
			this.submitting(true);
			return true;
		}

	}

}