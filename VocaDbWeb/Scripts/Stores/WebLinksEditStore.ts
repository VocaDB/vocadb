import WebLinkContract from '@/DataContracts/WebLinkContract';
import WebLinkCategory from '@/Models/WebLinkCategory';
import { makeObservable } from 'mobx';

import BasicListEditStore from './BasicListEditStore';
import WebLinkEditStore from './WebLinkEditStore';

export default class WebLinksEditStore extends BasicListEditStore<
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
