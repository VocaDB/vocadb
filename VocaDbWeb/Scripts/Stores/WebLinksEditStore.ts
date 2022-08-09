import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { WebLinkCategory } from '@/Models/WebLinkCategory';
import { BasicListEditStore } from '@/Stores/BasicListEditStore';
import { WebLinkEditStore } from '@/Stores/WebLinkEditStore';
import { makeObservable } from 'mobx';

export class WebLinksEditStore extends BasicListEditStore<
	WebLinkEditStore,
	WebLinkContract
> {
	public constructor(
		webLinkContracts: WebLinkContract[],
		public categories?: WebLinkCategory[],
	) {
		super(WebLinkEditStore, webLinkContracts);

		makeObservable(this);
	}
}
