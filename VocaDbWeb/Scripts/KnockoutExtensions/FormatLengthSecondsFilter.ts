
interface KnockoutFilters {
	formatLengthSeconds: (length: number) => string;
}

interface KnockoutStatic {
	filters: KnockoutFilters;
}

ko.filters.formatLengthSeconds = (length) => {
	return vdb.helpers.DateTimeHelper.formatFromSeconds(length);
}