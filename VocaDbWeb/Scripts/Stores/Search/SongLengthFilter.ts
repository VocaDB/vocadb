import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { computed, makeObservable, observable } from 'mobx';

export class SongLengthFilter {
	@observable length = 0;

	constructor() {
		makeObservable(this);
	}

	@computed get lengthFormatted(): string {
		return DateTimeHelper.formatFromSeconds(this.length);
	}
	set lengthFormatted(value: string) {
		this.length = DateTimeHelper.parseToSeconds(value);
	}
}
