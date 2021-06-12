import TranslatedEnumField from '@DataContracts/TranslatedEnumField';
import WebLinkContract from '@DataContracts/WebLinkContract';

import BasicListEditViewModel from './BasicListEditViewModel';
import WebLinkEditViewModel from './WebLinkEditViewModel';

export default class WebLinksEditViewModel extends BasicListEditViewModel<
	WebLinkEditViewModel,
	WebLinkContract
> {
	public constructor(
		webLinkContracts: WebLinkContract[],
		public categories?: TranslatedEnumField[],
	) {
		super(WebLinkEditViewModel, webLinkContracts);
	}
}
