/// <reference path="../typings/knockout/knockout.d.ts" />

module vdb.viewModels {

	export class TagEditViewModel {

		public description: KnockoutObservable<string>;
		public parent: KnockoutObservable<string>;

		constructor(private name: string, parent: string, description: string) {

			this.description = ko.observable(description);
			this.parent = ko.observable(parent);

		}

		denySelf = (tagName: string) => (tagName != this.name);

	}

}