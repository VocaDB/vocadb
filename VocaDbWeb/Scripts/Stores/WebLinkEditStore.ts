import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { WebLinkCategory } from '@/Models/WebLinkCategory';
import { WebLinkMatcher } from '@vocadb/web-link-matcher';
import { makeObservable, observable, reaction } from 'mobx';

export class WebLinkEditStore {
	@observable public category: WebLinkCategory;
	@observable public description: string;
	@observable public disabled: boolean;
	public readonly id: number;
	@observable public url: string;

	public constructor(data?: WebLinkContract) {
		makeObservable(this);

		if (data) {
			this.category = data.category;
			this.description = data.description;
			this.disabled = data.disabled;
			this.id = data.id;
			this.url = data.url;
		} else {
			this.category = WebLinkCategory.Other;
			this.description = '';
			this.disabled = false;
			this.id = 0;
			this.url = '';
		}

		reaction(
			() => this.url,
			(url) => {
				if (this.description) return;

				const matcher = WebLinkMatcher.matchWebLink(url);

				if (matcher) {
					this.description = matcher.desc;
					this.category = matcher.cat;
				}
			},
		);
	}
}
