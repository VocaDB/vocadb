import DateTimeHelper from '@Helpers/DateTimeHelper';

declare global {
	interface KnockoutPunchesFilters {
		formatLengthSeconds: (length: number) => string;
	}
}

ko.filters.formatLengthSeconds = (length): string => {
	return DateTimeHelper.formatFromSeconds(length);
};
