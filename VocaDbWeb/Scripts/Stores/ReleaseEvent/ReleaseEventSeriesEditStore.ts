import _ from 'lodash';
import { action, makeObservable, observable } from 'mobx';

import NamesEditStore from '../Globalization/NamesEditStore';
import WebLinksEditStore from '../WebLinksEditStore';

export default class ReleaseEventSeriesEditStore {
	@observable public defaultNameLanguage: string;
	@observable public description: string;
	@observable public duplicateName: string;
	public readonly names: NamesEditStore;
	@observable public submitting = false;
	public readonly webLinks: WebLinksEditStore;

	public constructor(private readonly id: number, defaultNameLanguage: string) {
		makeObservable(this);

		this.defaultNameLanguage = defaultNameLanguage;
		this.names = NamesEditStore.fromContracts(names);
		this.webLinks = new WebLinksEditStore(this.webLinks);

		if (!this.isNew) {
			// TODO
		} else {
			// TODO
		}
	}

	private isNew = (): boolean => {
		return !this.id;
	};

	@action public submit = (): boolean => {
		this.submitting = true;
		return true;
	};
}
