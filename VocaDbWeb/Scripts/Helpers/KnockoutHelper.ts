import Decimal from 'decimal.js-light';
import ko, { Computed, Observable } from 'knockout';

import DateTimeHelper from './DateTimeHelper';

export default class KnockoutHelper {
	public static stringEnum<T>(
		observable: Observable<T>,
		enumType: any,
	): Computed<string> {
		return ko.computed({
			read: () => {
				var val: any = observable();
				return enumType[val];
			},
			write: (val: string) => observable(enumType[val]),
		});
	}

	public static bpm(observable: Observable<number>): Computed<string> {
		return ko.computed({
			read: () => {
				var val: any = observable();
				return val ? new Decimal(val).div(1000).toString() : null!;
			},
			write: (val: string) => {
				observable(
					val ? new Decimal(val).mul(1000).toInteger().toNumber() : null!,
				);
			},
		});
	}

	public static lengthFormatted(
		observable: Observable<number>,
	): Computed<string> {
		return ko.computed({
			read: () => {
				var val: any = observable();
				return DateTimeHelper.formatFromSeconds(val);
			},
			write: (val: string) => {
				observable(DateTimeHelper.parseToSeconds(val));
			},
		});
	}
}
