import { makeObservable, observable } from 'mobx';

import { BasicListEditStore } from './BasicListEditStore';

export class CultureCodesEditStore extends BasicListEditStore<String, String> {
	@observable extended: boolean;
	constructor(cultureCodes: string[], extended?: boolean) {
		super(String, cultureCodes);

		this.extended =
			extended ??
			cultureCodes.filter((c) => c.length > 2 && c !== 'fil').length > 0;

		makeObservable(this);
	}
}
