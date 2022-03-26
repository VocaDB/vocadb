import DateTimeHelper from '@Helpers/DateTimeHelper';
import { computed, makeObservable, observable } from 'mobx';

export default class SongLengthFilter {
	@observable public length = 0;

	public constructor() {
		makeObservable(this);
	}

	@computed public get lengthFormatted(): string {
		return DateTimeHelper.formatFromSeconds(this.length);
	}
	public set lengthFormatted(value: string) {
		this.length = DateTimeHelper.parseToSeconds(value);
	}
}
