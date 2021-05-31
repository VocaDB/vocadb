import WebLinkContract from '@DataContracts/WebLinkContract';
import WebLinkCategory from '@Models/WebLinkCategory';
import WebLinkMatcher from '@Shared/WebLinkMatcher';
import ko, { Observable } from 'knockout';

export default class WebLinkEditViewModel {
	public category: Observable<string>;

	public description: Observable<string>;

	public disabled: Observable<boolean>;

	public id: number;

	public url: Observable<string>;

	constructor(data?: WebLinkContract) {
		if (data) {
			this.category = ko.observable(data.category);
			this.description = ko.observable(data.description);
			this.disabled = ko.observable(data.disabled);
			this.id = data.id;
			this.url = ko.observable(data.url);
		} else {
			this.category = ko.observable(WebLinkCategory[WebLinkCategory.Other]);
			this.description = ko.observable('');
			this.disabled = ko.observable(false);
			this.id = 0;
			this.url = ko.observable('');
		}

		this.url.subscribe((url) => {
			if (!this.description()) {
				var matcher = WebLinkMatcher.matchWebLink(url);

				if (matcher) {
					this.description(matcher.desc);
					this.category(WebLinkCategory[matcher.cat]);
				}
			}
		});
	}
}
