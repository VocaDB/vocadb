
import TranslatedEnumField from '../DataContracts/TranslatedEnumField';
import WebLinkContract from '../DataContracts/WebLinkContract';
import WebLinkEditViewModel from './WebLinkEditViewModel';

//module vdb.viewModels {

    export default class WebLinksEditViewModel {

        public add: () => void;

        public remove: (webLink) => void;

		public toContracts: () => WebLinkContract[] = () => {
			return ko.toJS(this.webLinks);
		}

        public webLinks: KnockoutObservableArray<WebLinkEditViewModel>;

        constructor(webLinkContracts: WebLinkContract[], public categories?: TranslatedEnumField[]) {
            
            this.webLinks = ko.observableArray(_.map(webLinkContracts, contract => new WebLinkEditViewModel(contract)));
            
            this.add = () => {
                this.webLinks.push(new WebLinkEditViewModel());
            };

            this.remove = (webLink) => {
                this.webLinks.remove(webLink);
            }

        }

    }

//}

