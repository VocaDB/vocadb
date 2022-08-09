import { TranslatedEnumField } from '@/DataContracts/TranslatedEnumField';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { BasicListEditViewModel } from '@/ViewModels/BasicListEditViewModel';
import { WebLinkEditViewModel } from '@/ViewModels/WebLinkEditViewModel';

export class WebLinksEditViewModel extends BasicListEditViewModel<
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
