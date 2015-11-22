
module vdb.viewModels.search {

	export class TagFilter {

		constructor(public id: number, name?: string, urlSlug?: string) {
			this.name(name);
			this.urlSlug(urlSlug);
		}

		public name = ko.observable<string>(null);

		public urlSlug = ko.observable<string>(null);

	}

} 