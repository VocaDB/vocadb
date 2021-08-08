import Decimal from 'decimal.js-light';
import { computed, makeObservable, observable } from 'mobx';

export default class SongBpmFilter {
	@observable public milliBpm?: number;

	public constructor() {
		makeObservable(this);
	}

	@computed public get bpmAsString(): string | undefined {
		return this.milliBpm
			? new Decimal(this.milliBpm).div(1000).toString()
			: undefined;
	}
	public set bpmAsString(value: string | undefined) {
		this.milliBpm = value
			? new Decimal(value).mul(1000).toInteger().toNumber()
			: undefined;
	}
}
