import Decimal from 'decimal.js-light';

export class BpmHelper {
	static formatFromMilliBpm = (
		minMilliBpm?: number,
		maxMilliBpm?: number,
	): string => {
		if (minMilliBpm && maxMilliBpm && maxMilliBpm > minMilliBpm) {
			return `${new Decimal(minMilliBpm).div(1000)} - ${new Decimal(
				maxMilliBpm,
			).div(1000)}`;
		}

		if (minMilliBpm) return `${new Decimal(minMilliBpm).div(1000)}`;

		return '';
	};
}
