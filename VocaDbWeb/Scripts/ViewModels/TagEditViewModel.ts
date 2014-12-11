/// <reference path="../typings/knockout/knockout.d.ts" />

module vdb.viewModels {

	import dc = vdb.dataContracts;

	export class TagEditViewModel {

		constructor(contract: dc.TagApiContract) {

			this.aliasedToName = ko.observable(contract.aliasedToName);
			this.description = ko.observable(contract.description);
			this.name = contract.name;
			this.parentName = ko.observable(contract.parentName);

		}

		public aliasedToName: KnockoutObservable<string>;
		public description: KnockoutObservable<string>;
		public name: string;
		public parentName: KnockoutObservable<string>;

		denySelf = (tagName: string) => (tagName != this.name);

	}

}