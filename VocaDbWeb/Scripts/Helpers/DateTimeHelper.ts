import dayjs from 'dayjs';
import UTC from 'dayjs/plugin/utc';

dayjs.extend(UTC);

export class DateTimeHelper {
	private static addLeadingZero(val: any): any {
		return val < 10 ? '0' + val : val;
	}

	static formatComponentDate(
		year?: number,
		month?: number,
		date?: number,
	): string {
		if (date && month && year) {
			return dayjs()
				.year(year)
				.month(month - 1)
				.date(date)
				.format('ll');
		}

		if (month && year) {
			return dayjs()
				.year(year)
				.month(month - 1)
				.format('MMM YYYY');
		}

		if (year) {
			return dayjs().year(year).format('YYYY');
		}

		return '';
	}

	static convertToLocal(utcDate: Date): Date | null {
		if (utcDate == null) return null;
		const momentDate = dayjs.utc(utcDate);
		return new Date(momentDate.year(), momentDate.month(), momentDate.date());
		//return new Date(utcDate.getFullYear(), utcDate.getMonth(), utcDate.getDate());
	}

	static convertToUtc(localDate: Date): Date | null {
		if (localDate == null) return null;
		return dayjs
			.utc()
			.year(localDate.getFullYear())
			.month(localDate.getMonth())
			.date(localDate.getDate())
			.toDate();
	}

	// Formats seconds as minutes and seconds, for example 12:34
	static formatFromSeconds = (seconds: number): string => {
		seconds = Math.max(seconds, 0);
		var mins = Math.floor(seconds / 60);
		return mins + ':' + DateTimeHelper.addLeadingZero(seconds % 60);
	};

	static parseToSeconds = (formatted: string): number => {
		const clamp = (value: number, min: number, max: number): number =>
			Math.min(Math.max(value, min), max);
		const parseToSecondsInternal = (formatted: string): number => {
			var parts = formatted.split(':');
			switch (parts.length) {
				case 1: {
					var seconds = parseInt(parts[0]) || 0;
					return seconds;
				}
				case 2: {
					var mins = parseInt(parts[0]) || 0;
					// eslint-disable-next-line @typescript-eslint/no-redeclare
					var seconds = parseInt(parts[1]) || 0;
					return mins * 60 + seconds;
				}
				default: {
					return 0;
				}
			}
		};
		return clamp(parseToSecondsInternal(formatted), 0, 60 * 3939);
	};
}
