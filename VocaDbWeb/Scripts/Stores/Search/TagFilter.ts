import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import {  makeObservable, observable } from 'mobx';

export class TagFilter {
	@observable name?: string = undefined;
	@observable excluded: boolean = false;
	@observable urlSlug?: string = undefined;

	constructor(readonly id: number, name?: string, urlSlug?: string) {
		makeObservable(this);

		this.name = name;
		this.urlSlug = urlSlug;
	}

	static fromContract = (tag: TagBaseContract): TagFilter => {
		return new TagFilter(tag.id, tag.name, tag.urlSlug);
	};
}
