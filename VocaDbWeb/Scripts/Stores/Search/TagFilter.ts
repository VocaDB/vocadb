import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import { makeObservable, observable } from 'mobx';

export default class TagFilter {
	@observable public name?: string = undefined;
	@observable public urlSlug?: string = undefined;

	public constructor(
		public readonly id: number,
		name?: string,
		urlSlug?: string,
	) {
		makeObservable(this);

		this.name = name;
		this.urlSlug = urlSlug;
	}

	public static fromContract = (tag: TagBaseContract): TagFilter => {
		return new TagFilter(tag.id, tag.name, tag.urlSlug);
	};
}
