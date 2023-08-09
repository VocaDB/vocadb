import dayjs from 'dayjs';
import LocalizedFormat from 'dayjs/plugin/localizedFormat';

dayjs.extend(LocalizedFormat);

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

interface DateStat {
	date: string;
	count: number;
}

export const sumDatesInOneDay = (dateStrings: string[]): DateStat[] => {
	const summedDates: Map<string, DateStat> = new Map();

	for (const dateString of dateStrings) {
		const date = new Date(dateString);
		date.setHours(0, 0, 0, 0);

		const formattedDate = date.toISOString().slice(0, 10);

		if (summedDates.has(formattedDate)) {
			const count = summedDates.get(formattedDate)!.count;
			summedDates.set(formattedDate, {
				count: count + 1,
				date: formattedDate,
			});
		} else {
			summedDates.set(formattedDate, {
				count: 1,
				date: formattedDate,
			});
		}
	}

	const result = Array.from(summedDates.keys())
		.map((dateString) => {
			const count = summedDates.get(dateString)!.count;
			return { date: summedDates.get(dateString)?.date!, count };
		})
		.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());

	console.log(result);
	return result;
};

