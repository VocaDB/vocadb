const div = (num: number): number => {
	return Math.round(num / 1000);
};

export const formatFromMilliBpm = (minMilliBpm?: number, maxMilliBpm?: number): string => {
	if (minMilliBpm && maxMilliBpm && maxMilliBpm > minMilliBpm) {
		return `${div(minMilliBpm)} - ${div(maxMilliBpm)}`;
	}

	if (minMilliBpm) return `${div(minMilliBpm)}`;

	return '';
};

