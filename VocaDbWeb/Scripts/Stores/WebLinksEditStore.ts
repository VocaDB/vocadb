import TranslatedEnumField from '@DataContracts/TranslatedEnumField';
import WebLinkContract from '@DataContracts/WebLinkContract';
import { makeObservable } from 'mobx';

import BasicListEditStore from './BasicListEditStore';
import WebLinkEditStore from './WebLinkEditStore';

export default class WebLinksEditStore extends BasicListEditStore<
	WebLinkEditStore,
	WebLinkContract
> {
	public constructor(
		webLinkContracts: WebLinkContract[],
		public categories?: TranslatedEnumField[],
	) {
		super(WebLinkEditStore, webLinkContracts);

		makeObservable(this);
	}
}
