
interface KnockoutFilters {
	formatLengthSeconds: (length: number) => string;
}

ko.filters.formatLengthSeconds = (length) => {
	return vdb.helpers.DateTimeHelper.formatFromSeconds(length);
}