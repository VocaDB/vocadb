
module vdb.viewModels.search {

	export class TagFilter {

		public static fromContract = (tag: dc.TagBaseContract) => {
			return new TagFilter(tag.id, tag.name, tag.urlSlug);
		}

		constructor(public id: number, name?: string, urlSlug?: string) {
			this.name(name);
			this.urlSlug(urlSlug);
		}

		public name = ko.observable<string>(null);

		public urlSlug = ko.observable<string>(null);

	}

} 