import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { WebLinkCategory } from '@/Models/WebLinkCategory';
import { BasicListEditStore } from '@/Stores/BasicListEditStore';
import { WebLinkEditStore } from '@/Stores/WebLinkEditStore';
import { makeObservable } from 'mobx';

export class WebLinksEditStore extends BasicListEditStore<
	WebLinkEditStore,
	WebLinkContract
> {
	constructor(
		webLinkContracts: WebLinkContract[],
		readonly categories?: WebLinkCategory[],
	) {
		super(WebLinkEditStore, webLinkContracts);

		makeObservable(this);
	}
}
