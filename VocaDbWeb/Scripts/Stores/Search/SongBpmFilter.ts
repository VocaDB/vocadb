import Decimal from 'decimal.js-light';
import { computed, makeObservable, observable } from 'mobx';

export class SongBpmFilter {
	@observable milliBpm?: number;

	constructor() {
		makeObservable(this);
	}

	@computed get bpmAsString(): string | undefined {
		return this.milliBpm
			? new Decimal(this.milliBpm).div(1000).toString()
			: undefined;
	}
	set bpmAsString(value: string | undefined) {
		this.milliBpm = value
			? new Decimal(value).mul(1000).toInteger().toNumber()
			: undefined;
	}
}
