
interface KnockoutFilters {
	formatDate: (date: Date, format: string) => string;
}

ko.filters.formatDate = (date, format) => {

	if (!date)
		return "";

	return moment(date).format(format);

}