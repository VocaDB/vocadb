import dayjs from 'dayjs';

// Formats 160 to 2:40
export const formatNumberToTime = (number: number): string => {
	const rounded = Math.round(number);
	const minutes = Math.floor(rounded / 60);
	const remaining = rounded - 60 * minutes;
	return `${minutes}:${(remaining < 10 ? '0' : '') + remaining}`;
};

export const formatComponentDate = (year?: number, month?: number, date?: number): string => {
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
};

