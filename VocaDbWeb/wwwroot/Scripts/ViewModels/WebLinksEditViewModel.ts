import BasicListEditViewModel from './BasicListEditViewModel';
import TranslatedEnumField from '@DataContracts/TranslatedEnumField';
import WebLinkContract from '@DataContracts/WebLinkContract';
import WebLinkEditViewModel from './WebLinkEditViewModel';

export default class WebLinksEditViewModel extends BasicListEditViewModel<
  WebLinkEditViewModel,
  WebLinkContract
> {
  constructor(
    webLinkContracts: WebLinkContract[],
    public categories?: TranslatedEnumField[],
  ) {
    super(WebLinkEditViewModel, webLinkContracts);
  }
}
